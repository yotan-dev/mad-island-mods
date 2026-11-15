using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using BepInEx;

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
		private const string UserTableFileName = "EnhancedIsland.DissassembleTable.user.xml";

		public static Dictionary<string, DisassembleItem[]> Table = [];

		/// <summary>
		/// Applies overrides/additions from the optional user disassemble table
		/// located in the BepInEx config folder. Entries without materials remove
		/// the item from the table entirely.
		/// </summary>
		public static void ApplyUserOverrides()
		{
			var path = Path.Combine(Paths.ConfigPath, UserTableFileName);

			if (!File.Exists(path))
			{
				PLogger.LogVerbose($"User disassemble table not found at '{path}'. Using generated table only.");
				return;
			}

			UserDisassembleTableConfig? userTable;
			try
			{
				var serializer = new XmlSerializer(typeof(UserDisassembleTableConfig));
				using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
				userTable = (UserDisassembleTableConfig?)serializer.Deserialize(stream);
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to load user disassemble table from '{path}': {e.Message}");
				PLogger.LogError(e);
				return;
			}

			if (userTable?.Entries == null)
			{
				PLogger.LogWarning($"User disassemble table at '{path}' has no entries. Using generated table only.");
				return;
			}

			var logEntries = PConfig.Instance.DisassembleItemsLogUserTableEntries.Value;
			var applied = 0;
			foreach (var entry in userTable.Entries)
			{
				if (entry == null || string.IsNullOrWhiteSpace(entry.Id))
				{
					PLogger.LogWarning("User disassemble table contains an entry with no id. Skipping.");
					continue;
				}

				var id = entry.Id;

				if (entry.Materials == null || entry.Materials.Count == 0)
				{
					if (Table.Remove(id))
					{
						if (logEntries)
							PLogger.LogInfo($"User disassemble table removed '{id}'");
						applied++;
					}
					else
					{
						PLogger.LogVerbose($"User disassemble table tried to remove '{id}', but it is not in the table");
					}
					continue;
				}

				var items = new List<DisassembleItem>();
				foreach (var material in entry.Materials)
				{
					if (material == null || string.IsNullOrWhiteSpace(material.Item))
					{
						PLogger.LogWarning($"User disassemble table entry '{id}' has a material with no item name. Skipping it.");
						continue;
					}

					if (material.Count <= 0)
					{
						PLogger.LogWarning($"User disassemble table entry '{id}' material '{material.Item}' has invalid count {material.Count}. Skipping it.");
						continue;
					}

					items.Add(new DisassembleItem(material.Item, material.Count));
				}

				if (items.Count == 0)
				{
					PLogger.LogWarning($"User disassemble table entry '{id}' has no valid materials. Skipping it.");
					continue;
				}

				var action = Table.ContainsKey(id) ? "overrode" : "added";
				Table[id] = [.. items];
				if (logEntries)
					PLogger.LogInfo($"User disassemble table {action} '{id}' with {items.Count} material(s)");
				applied++;
			}

			PLogger.LogInfo($"User disassemble table applied {applied} change(s)");
		}
	}
}
