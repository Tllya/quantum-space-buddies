﻿using OWML.Common;
using QSB.ItemSync.WorldObjects.Items;
using QSB.ItemSync.WorldObjects.Sockets;
using QSB.Utility;
using QSB.WorldSync;

namespace QSB.ItemSync
{
	internal class ItemManager : WorldObjectManager
	{
		public override WorldObjectType WorldObjectType => WorldObjectType.Both;

		public override void BuildWorldObjects(OWScene scene)
		{
			DebugLog.DebugWrite("Building OWItems...", MessageType.Info);

			// Items
			QSBWorldSync.Init<QSBScrollItem, ScrollItem>();
			QSBWorldSync.Init<QSBSharedStone, SharedStone>();
			QSBWorldSync.Init<QSBWarpCoreItem, WarpCoreItem>();
			QSBWorldSync.Init<QSBNomaiConversationStone, NomaiConversationStone>();
			QSBWorldSync.Init<QSBSimpleLanternItem, SimpleLanternItem>();
			QSBWorldSync.Init<QSBSlideReelItem, SlideReelItem>();

			// Sockets
			QSBWorldSync.Init<QSBItemSocket, OWItemSocket>();
		}
	}
}
