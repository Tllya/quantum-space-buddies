﻿using QSB.Menus;
using QSB.Messaging;
using QSB.Utility;
using QuantumUNET;
using System.Linq;
using QuantumUNET.Transport;

namespace QSB.Player.Events
{
	// sent by the server only
	internal class PlayerKickMessage : QSBEnumMessage<KickReason>
	{
		private uint PlayerId;

		public PlayerKickMessage(uint playerId, KickReason reason)
		{
			PlayerId = playerId;
			Value = reason;
		}

		public PlayerKickMessage() { }

		public override void Serialize(QNetworkWriter writer)
		{
			base.Serialize(writer);
			writer.Write(PlayerId);
		}

		public override void Deserialize(QNetworkReader reader)
		{
			base.Deserialize(reader);
			PlayerId = reader.ReadUInt32();
		}

		public override void OnReceiveLocal()
		{
			if (!QSBCore.IsHost)
			{
				return;
			}

			QSBCore.UnityEvents.FireInNUpdates(KickPlayer, 10);
		}

		private void KickPlayer()
			=> QNetworkServer.connections.First(x => PlayerId == x.GetPlayerId()).Disconnect();

		public override void OnReceiveRemote()
		{
			if (PlayerId != QSBPlayerManager.LocalPlayerId)
			{
				if (QSBPlayerManager.PlayerExists(PlayerId))
				{
					DebugLog.ToAll($"{QSBPlayerManager.GetPlayer(PlayerId).Name} was kicked.");
					return;
				}

				DebugLog.ToAll($"Player id:{PlayerId} was kicked.");
				return;
			}

			DebugLog.ToAll($"Kicked from server. Reason : {Value}");
			MenuManager.Instance.OnKicked(Value);
		}
	}
}
