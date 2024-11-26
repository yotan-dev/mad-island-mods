using HarmonyLib;
using YotanModCore;
using UnityEngine;

namespace DisassembleItems
{
	public class DisassembleItemsPatch
	{
		[HarmonyPatch(typeof(ItemManager), "DisassembleAnimal")]
		[HarmonyPrefix]
		private static void Pre_ItemManager_DisassembleAnimal(ItemManager __instance, int slotID, ref bool __runOriginal)
		{
			__runOriginal = true;

			if (Managers.mn.inventory.itemSlot[slotID].toolType == ItemData.ToolType.Animal)
				return;

			if (slotID >= Managers.mn.inventory.baseCount)
			{
				__instance.StartCoroutine(Managers.mn.eventMN.GoCaution(45)); /* Cannot disassemble NPCs in chest. */
				return;
			}

			ItemData itemData = __instance.FindItem(Managers.mn.inventory.itemSlot[slotID].itemKey);
			DisassembleTable.Table.TryGetValue(itemData.name, out DisassembleItem[] items);

			if (items == null)
			{
				__instance.StartCoroutine(Managers.mn.eventMN.GoCautionSt("Can't disassemble"));
				return;
			}

			__runOriginal = false;

			Managers.mn.inventory.ConsumeItem(slotID, 1);
			foreach (var item in items)
			{
				var itemInfo = Managers.mn.itemMN.FindItem(item.item);
				if (!itemInfo)
				{
					__instance.StartCoroutine(Managers.mn.eventMN.GoCautionSt($"Item {item.item} not found?? (BUG)"));
					continue;
				}

				Managers.mn.itemMN.GetItem(itemInfo, item.count);

				PLogger.LogInfo($"Disassembled {itemData.GetItemName()}");
			}

			return;
		}
	}
}