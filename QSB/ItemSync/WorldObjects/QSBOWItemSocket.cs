﻿using OWML.Utils;
using QSB.WorldSync;
using UnityEngine;

namespace QSB.ItemSync.WorldObjects
{
	internal class QSBOWItemSocket<T> : WorldObject<T>, IQSBOWItemSocket
		where T : MonoBehaviour
	{
		protected IQSBOWItem _socketedItem
		{
			get => ItemManager.GetObject(AttachedObject.GetValue<OWItem>("_socketedItem"));
			set => AttachedObject.SetValue("_socketedItem", (value as IWorldObject)?.ReturnObject());
		}

		private IQSBOWItem _removedItem
		{
			get => ItemManager.GetObject(AttachedObject.GetValue<OWItem>("_removedItem"));
			set => AttachedObject.SetValue("_removedItem", (value as IWorldObject).ReturnObject());
		}

		private Transform _socketTransform
		{
			get => AttachedObject.GetValue<Transform>("_socketedTransform");
			set => AttachedObject.SetValue("_socketedTransform", value);
		}

		private Sector _sector
		{
			get => AttachedObject.GetValue<Sector>("_sector");
			set => AttachedObject.SetValue("_sector", value);
		}

		public override void Init(T attachedObject, int id) { }

		public virtual bool AcceptsItem(IQSBOWItem item)
		{
			var itemType = item.GetItemType();
			var acceptableType = AttachedObject.GetValue<ItemType>("_acceptableType");
			return (itemType & acceptableType) == itemType;
		}

		public virtual bool PlaceIntoSocket(IQSBOWItem item)
		{
			if (!AcceptsItem(item) || _socketedItem != default)
			{
				return false;
			}
			_socketedItem = item;
			_socketedItem.SocketItem(_socketTransform, _sector);
			_socketedItem.PlaySocketAnimation();
			AttachedObject.enabled = true;
			return true;
		}

		public virtual IQSBOWItem RemoveFromSocket()
		{
			_removedItem = _socketedItem;
			_socketedItem = default;
			_removedItem.PlayUnsocketAnimation();
			_removedItem.SetColliderActivation(true);
			AttachedObject.enabled = true;
			return _removedItem;
		}
	}
}