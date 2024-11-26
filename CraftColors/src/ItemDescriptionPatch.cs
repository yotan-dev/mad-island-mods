using YotanModCore;
using HarmonyLib;
using UnityEngine.UI;
using TMPro;

namespace CraftColors
{
	public class ItemDescriptionPatch
	{
		[HarmonyPatch(typeof(UIManager), "ItemDescOpen")]
		[HarmonyPostfix]
		private static void Post_UIManager_ItemDescOpen(int slotID)
		{
			bool isCraft = slotID >= 100 && slotID < 1000;
			bool isShop = slotID >= 1000;

			CraftInfo.Required[] requiredList;
			if (isCraft) {
				requiredList = Managers.mn.craftMN.craftData[Managers.mn.inventory.craftPanelID].craftInfo[slotID - 100].required;

				var itemSlot = Managers.mn.inventory.itemSlot[slotID];
				if (itemSlot.GetComponent<Button>().interactable) {
					// The validation already found that this item is fine. No need to check
					return;
				}
			} else if (isShop) {
				requiredList = Managers.mn.shop.shops[Managers.mn.shop.activeShopID].shopItems[slotID - 1000].GetComponent<CraftInfo>().shopTrade;

				// Don't know an equivalent to CraftableCheckSlot ; this could allow us to disable the button like in craft
			} else {
				return;
			}

			var reqs = new CraftRequirements();
			for (int i = 0; i < requiredList.Length; i++) {
				var req = new CraftRequirements.Requirement() {
					ItemKey = requiredList[i].itemData.name,
					Required = requiredList[i].count < 1f ? 1 : (int)requiredList[i].count,
				};
				reqs.AddRequirement(req);
			}

			reqs.CheckRequirements();
			if (!reqs.MissingRequirement) {
				return; // All requirements are met (shouldn't happen, but just in case)
			}

			var requirementList = reqs.GetRequirementList();
			for (int i = 0; i < requirementList.Count; i++) {
				var item = requirementList[i];
				ItemData reqItem = Managers.mn.itemMN.FindItem(item.ItemKey);
				ItemSlot reqSlot = Managers.mn.inventory.needSlot[i];

				var itemName = reqSlot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = reqItem.GetItemName();
				if (item.Current < item.Required) {
					reqSlot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = $"<color=\"red\">{itemName}</color>";
				} else {
					reqSlot.transform.Find("Text").GetComponent<TextMeshProUGUI>().text = itemName;
				}
			}
		}
	}
}