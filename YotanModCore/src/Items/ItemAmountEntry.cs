#nullable enable

using System;

namespace YotanModCore.Items
{
	[Experimental]
	[Serializable]
	public class ItemAmountEntry
	{
		public string ItemCode { get; set; } = "";

		public float Count { get; set; } = 0f;
	}
}
