using UnityEngine;

namespace YotanModCore.Items
{
	public class CustomItemData : ItemData
	{
		[Header("Custom Item Data")]
		[Tooltip("Item's internal identifier. Must be unique in the game. Used for get and save files.")]
		public string ItemKey;

		/* Proxies to original one */

		public void ShallowCopy(ItemData original)
		{
			if (original is CustomItemData itd) {
				this.ItemKey = itd.ItemKey;
			}

			// Copy fields from item data -- @TODO: Maybe use a code generator, or add some sort of validator
			this.itemType = original.itemType;
			this.subType = original.subType;
			this.itemName = original.itemName;
			this.localizeName = original.localizeName;
			this.maxStack = original.maxStack;
			this.itemSprite = original.itemSprite;
			this.itemObj = original.itemObj;
			this.toolType = original.toolType;
			this.deco = original.deco;
			this.onWater = original.onWater;
			this.onEvery = original.onEvery;
			this.horizonFit = original.horizonFit;
			this.itemLife = original.itemLife;
			this.attack = original.attack;
			this.TipsText = original.TipsText;
			this.localizedTipsText = original.localizedTipsText;
		}
	}
}
