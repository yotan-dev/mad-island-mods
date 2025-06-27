using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace YotanModCore.Items
{
	public class ItemDB : ScriptableObject
	{
		public static ItemDB Instance { get; private set; }

		public readonly Dictionary<string, CustomItemData> Items = [];

		internal static void Init()
		{
			Assert.IsNull(Instance, "ItemDB already exists!");

			ItemDB.Instance = ScriptableObject.CreateInstance<ItemDB>();
			PLogger.LogInfo("ItemDB Initialized");
		}

		public CustomItemData GetItem(string name)
		{
			PLogger.LogDebug($"Getting item {name}");
			if (name == null)
			{
				return null;
			}

			var item = Items.GetValueOrDefault(name, null);

			return item;
		}

		public void RegisterItem(CustomItemData item)
		{
			if (this.Items.ContainsKey(item.ItemKey))
			{
				PLogger.LogError($"Item with key {item.ItemKey} already exists!");
				return;
			}

			PLogger.LogDebug($"Registered item {item.ItemKey} as {item} (Item is null: {item == null})");
			this.Items.Add(item.ItemKey, item);
		}

		private void LoadCraftInfo()
		{
			/*
			foreach (var item in this.Items.Values)
			{
				var customCraftInfo = item.GetComponent<CustomCraftInfo>();
				if (customCraftInfo == null)
					continue;

				if (customCraftInfo.shopTrade.Length > 0)
				{
					craftInfo.shopTrade = new CraftInfo.Required[customCraftInfo.shopTrade.Length];

					for (int i = 0; i < customCraftInfo.shopTrade.Length; i++)
					{
						craftInfo.shopTrade[i] = new CraftInfo.Required
						{
							itemData = Managers.mn.itemMN.FindItem(customCraftInfo.shopTrade[i].itemCode),
							count = craftInfo.shopTrade[i].count
						};
					}
				}

				GameObject.Destroy(customCraftInfo);
			}
			*/
		}
	}
}
