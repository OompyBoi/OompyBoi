﻿using Microsoft.Extensions.Logging;
using Server.Base.Network;
using Thrift.Protocol;

namespace Server.Reawakened.Thrift.Abstractions;

public abstract class ThriftHandler
{
    public delegate void ProcessFunction(ThriftProtocol protocol, NetState netState);

    private readonly Microsoft.Extensions.Logging.ILogger _logger;

    public readonly Dictionary<string, ProcessFunction> ProcessMap;

    protected ThriftHandler(Microsoft.Extensions.Logging.ILogger logger)
    {
        ProcessMap = [];
        _logger = logger;
    }

    public abstract void AddProcesses(Dictionary<string, ProcessFunction> processes);

    public void Process(ThriftProtocol protocol, NetState netState)
    {
        ProcessMap.TryGetValue(protocol.Name, out var process);

        if (process == null)
        {
            TProtocolUtil.Skip(protocol.InProtocol, TType.Struct);
            protocol.InProtocol.ReadMessageEnd();
            _logger.LogError("Invalid Thrift method: '{Message}'.", protocol.Name);
        }
        else
        {
            process(protocol, netState);
        }
    }
}
