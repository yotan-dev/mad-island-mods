using System.Collections.Generic;
using System.IO;

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

		public static void Init() {
			var json = File.ReadAllText("BepInEx/plugins/DisassembleItems/DisassembleTable.json");
			var fileTable = SimpleJSON.JSON.Parse(json);

			Table = [];
			foreach (var item in fileTable.AsArray) {
				var sourceId = item.Value.AsObject["itemId"].Value;
				var items = item.Value.AsObject["items"].AsArray;
				Table[sourceId] = new DisassembleItem[items.Count];

				for (var i = 0; i < items.Count; i++) {
					Table[sourceId][i] = new DisassembleItem(
						items[i].AsObject["item"].Value,
						items[i].AsObject["count"].AsInt
					);
				}
			}
			
			PLogger.LogInfo("Disassemble table loaded");
		}
	}
}