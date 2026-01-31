using System.Collections.Generic;
using HarmonyLib;
using YotanModCore;

namespace EnhancedIsland.DisassembleItems
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
			PLogger.LogVerbose($"Disassembling {itemData.GetItemName()}...");
			foreach (var item in items)
			{
				var itemInfo = Managers.mn.itemMN.FindItem(item.item);
				if (!itemInfo)
				{
					__instance.StartCoroutine(Managers.mn.eventMN.GoCautionSt($"Item {item.item} not found?? (BUG)"));
					PLogger.LogWarning($"Disassembling {itemData.GetItemName()} into {item.item} failed - item not found. (BUG?)");
					continue;
				}

				Managers.mn.itemMN.GetItem(itemInfo, item.count);
				PLogger.LogVerbose($"Disassembling {itemData.GetItemName()} into {itemInfo.GetItemName()} x{item.count}");
			}

			return;
		}

		[HarmonyPatch(typeof(CraftManager), "Awake")]
		[HarmonyPostfix]
		private static void Post_CraftManager_Awake(CraftManager __instance)
		{
			Dictionary<string, DisassembleItem[]> newTable = [];

			PLogger.LogDebug("Recreating disassemble table...");

			// Skips 0, as it is "Handmade" (already covered by bench_hand)
			for (int i = 1; i < __instance.craftData.Length; i++)
			{
				var benchData = __instance.craftData[i];

				foreach (var craftInfo in benchData.craftInfo)
				{
					var craftResultKey = craftInfo.name;
					if (newTable.ContainsKey(craftResultKey)) {
						PLogger.LogVerbose($"Skipping {craftResultKey} because it already exists");
						continue;
					}

					var items = new List<DisassembleItem>();
					var materials = craftInfo.required;
					foreach (var material in materials)
					{
						// We don't disassemble back to body parts (causes conflicts in item that are made from different parts)
						if (material.itemData.name.StartsWith("parts_")) {
							PLogger.LogVerbose($"Skipping {craftResultKey} requirement \"{material.itemData.name}\" because it is a body part");
							continue;
						}

						items.Add(new DisassembleItem(material.itemData.name, (int)material.count));
					}

					newTable[craftResultKey] = [.. items];
				}
			}

			PLogger.LogDebug($"Recreated disassemble table with {newTable.Count} items");
			DisassembleTable.Table = newTable;
		}
	}
}
