using System.Collections.Generic;

namespace EnhancedIsland.DisassembleItems
{
	[System.Serializable]
	public class DisassembleItem {
		public string item;

		public int count;

		public DisassembleItem(string item, int count) {
			this.item = item;
			this.count = count;
		}
	}

	public static class DisassembleTable {
		public static Dictionary<string, DisassembleItem[]> Table = [];
	}
}
