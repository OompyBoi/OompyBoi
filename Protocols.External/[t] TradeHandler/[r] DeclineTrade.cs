﻿using Microsoft.Extensions.Logging;
using Server.Reawakened.Network.Extensions;
using Server.Reawakened.Network.Protocols;
using Server.Reawakened.Players.Extensions;
using Server.Reawakened.Players.Helpers;
using Server.Reawakened.Players.Models.Trade;

namespace Protocols.External._t__TradeHandler;

public class DeclineTrade : ExternalProtocol
{
    public override string ProtocolName => "tr";

    public ILogger<DeclineType> Logger { get; set; }

    public PlayerHandler PlayerHandler { get; set; }

    public override void Run(string[] message)
    {
        var tradeModel = Player.TempData.TradeModel;

        if (tradeModel == null)
            return;

        var traderName = message[5];
        var tradedPlayer = PlayerHandler.GetPlayerByName(traderName);

        if (tradedPlayer == null)
            return;

        if (tradedPlayer != tradeModel.TradingPlayer)
            return;

        var status = (DeclineType) int.Parse(message[6]);

        switch (status)
        {
            case DeclineType.InviteeRejection:
                if (tradeModel.AcceptedTrade)
                    return;

                tradedPlayer?.SendXt("tc", Player.CharacterName);
                break;
            case DeclineType.PlayerBusy or DeclineType.PlayerDnD:
                tradedPlayer?.SendXt("tr", Player.CharacterName, (int)status);
                break;
            default:
                Logger.LogError("Unknown decline type: {DeclineType}", status);
                break;
        }

        Player.RemoveTrade();
    }
}

