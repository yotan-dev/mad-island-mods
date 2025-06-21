using UnityEngine;

namespace YotanModCore.Items
{
	public class CustomItemData : ItemData
	{
		[Header("Custom Item Data")]
		[Tooltip("Item's internal identifier. Must be unique in the game. Used for get and save files.")]
		public string ItemKey;

		/* Proxies to original one */
	}
}
