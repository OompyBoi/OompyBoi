﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Server.Base.Core.Abstractions;
using Server.Base.Core.Extensions;
using Server.Reawakened.XMLs.Abstractions;
using Server.Web.Abstractions;
using System.Runtime.Serialization;
using Web.AssetBundles.Events;

namespace Web.AssetBundles;

public class AssetBundles : WebModule
{
    public override string[] Contributors { get; } = { "Ferox", "Prefare" };

    public AssetBundles(ILogger<AssetBundles> logger) : base(logger)
    {
    }

    public override void AddServices(IServiceCollection services, Module[] modules)
    {
        services.AddSingleton<AssetEventSink>();

        Logger.LogDebug("Loading bundles");

        foreach (var xml in modules.GetServices<IBundledXml>())
        {
            Logger.LogTrace("   Loaded: {ServiceName}", xml.Name);
            services.AddSingleton(xml, FormatterServices.GetUninitializedObject(xml));
        }

        Logger.LogInformation("Loaded bundles");
    }
}
