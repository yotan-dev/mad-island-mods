using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace EnhancedIsland.ItemColorInSlot
{
	public class ItemSlotPatch
	{
		private static void SetupSlotTracking(ItemSlot slot)
		{
			Transform itemColor = slot.transform.Find("itemColor");
			if (itemColor != null)
				return;

			itemColor = GameObject.Instantiate(Managers.uiManager.UIManager.itemDescPanel.transform.Find("NameText/itemColor"), slot.transform);
			itemColor.name = "itemColor";
			var rectTransform = itemColor.GetComponent<RectTransform>();
			itemColor.localPosition = new Vector3(-5f, 5f, 0f);
			rectTransform.localScale /= 2;

			var tracker = itemColor.gameObject.GetComponent<ColorTracker>();
			if (tracker != null)
			{
				PLogger.LogWarning("ColorTracker already exists on " + slot.itemKey);
				return;
			}

			tracker = itemColor.gameObject.AddComponent<ColorTracker>();
			tracker.Init(slot);
		}

		[HarmonyPatch(typeof(ItemManager), nameof(ItemManager.ItemToSlot))]
		[HarmonyPostfix]
		private static void Post_ItemManager_ItemToSlot(ItemSlot tmpSlot)
		{
			SetupSlotTracking(tmpSlot);
		}

		[HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.SlotClear))]
		[HarmonyPostfix]
		private static void Post_InventoryManager_SlotClear(ItemSlot tmpSlot)
		{
			SetupSlotTracking(tmpSlot);
		}

		[HarmonyPatch(typeof(InventoryManager), nameof(InventoryManager.SlotLoadSlot))]
		[HarmonyPostfix]
		private static void Post_InventoryManager_SlotLoadSlot(ItemSlot from, ItemSlot to)
		{
			SetupSlotTracking(from);
			SetupSlotTracking(to);
		}
	}
}
