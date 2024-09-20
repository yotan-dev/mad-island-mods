using System.Collections.Generic;
using System.IO;

namespace DisassembleItems
{
	[System.Serializable]
	public class DisassembleItem {
		public string item;
		
		public int count;
	}

	public static class DisassembleTable {
		public static Dictionary<string, DisassembleItem[]> Table;

		public static void Init() {
			var json = File.ReadAllText("BepInEx/plugins/DisassembleItems/DisassembleTable.json");
			var fileTable = SimpleJSON.JSON.Parse(json);

			Table = new Dictionary<string, DisassembleItem[]>();
			foreach (var item in fileTable.AsArray) {
				var sourceId = item.Value.AsObject["itemId"].Value;
				var items = item.Value.AsObject["items"].AsArray;
				Table[sourceId] = new DisassembleItem[items.Count];

				for (var i = 0; i < items.Count; i++) {
					Table[sourceId][i] = new DisassembleItem {
						item = items[i].AsObject["item"].Value,
						count = items[i].AsObject["count"].AsInt
					};
				}
			}
			
			PLogger.LogInfo("Disassemble table loaded");
		}
	}
}