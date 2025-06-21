using System;
using UnityEngine;

namespace YotanModCore.Items
{
	public class CustomCraftInfo : MonoBehaviour
	{
		[Serializable]
		public class ItemEntry
		{
			public string itemCode;

			public float count;
		}

		[Header("Materials")]
		public ItemEntry[] craft;

		[Header("Shop")]
		public ItemEntry[] shopTrade;
	}
}
