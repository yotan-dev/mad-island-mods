#nullable enable

using System;

namespace YotanModCore.Items
{
	[Serializable]
	public class ItemAmountEntry
	{
		public string ItemCode { get; set; } = "";

		public float Count { get; set; } = 0f;
	}
}
