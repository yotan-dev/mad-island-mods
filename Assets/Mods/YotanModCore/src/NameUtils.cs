using System.Collections.Generic;

namespace YotanModCore
{
	public class NameUtils
	{
		private static readonly Dictionary<int, string> CraftStationNames = new Dictionary<int, string>()
		{
			{ 0, "Handmade" },
			{ 24, "Work Gloves" },
		};

		public static string CraftIdToEnglishName(int id)
		{
			var name = CraftStationNames.GetValueOrDefault(id, "");

			if (name == "")
			{
				PLogger.LogWarning($"Unknown Craft ID: {id}");
				return $"Unknown (ID: {id})";
			}

			return name;
		}
	}
}
