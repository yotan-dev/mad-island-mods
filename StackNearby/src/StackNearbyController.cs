using System.Collections;
using System.Collections.Generic;
using YotanModCore;
using UnityEngine;

namespace StackNearby
{
	public class StackNearbyController
	{
		public static List<ItemInfo> Storages = new List<ItemInfo>();

		private static StackNearbyController Instance = new StackNearbyController();

		public static void StackNearby(Vector3 fromPos) {
			Instance.StackNearby_Sub(fromPos);
		}

		private IEnumerator StackNearby_Subsub(Vector3 fromPos) {
			if (StackNearbyController.Storages == null) {
				PLogger.LogError($"StackNearby storages are null??");
				yield return null;
			}

			foreach (var storage in StackNearbyController.Storages) {
				var storagePos = storage?.transform?.position;
				if (storagePos == null) {
					continue;
				}

				float dist = Vector3.Distance(fromPos, storage.transform.position);
				if (dist < 5) {
					// base.StartCoroutine(this.UseProp(itemData));
					var inventorySlot = storage.GetComponent<InventorySlot>();
					if (inventorySlot == null) {
						PLogger.LogError($"inventorySlot is null??");
						continue;
					}

					var itemData = Managers.mn.itemMN.FindItem(storage.itemKey);
					Managers.mn.inventory.SubInventoryLoad(inventorySlot, itemData, true); 
					Managers.mn.inventory.MoveSameItemsToSub(); 
					Managers.mn.inventory.SubInventoryVisible();
				}
			}

			yield return null;
		}

		private void StackNearby_Sub(Vector3 fromPos) {
			Managers.mn.StartCoroutine(StackNearby_Subsub(fromPos));
		}
	}
}
