using System.Collections.Generic;
using System.Linq;
using YotanModCore.Consts;

namespace YotanModCore
{
	public class CommonUtils
	{
		private static Dictionary<int, string> NPCNames = new Dictionary<int, string>();

		internal static void Init()
		{
			var fields = typeof(NpcID).GetFields();
			foreach (var field in fields) {
				field.GetValue(null);
				if (field.GetCustomAttributes(typeof(StrValAttribute), false).FirstOrDefault() is StrValAttribute attr) {
					NPCNames[(int)field.GetValue(null)] = (string)attr.StrVal;
				}
			}
		}

		/// <summary>
		/// Checks whether npcId is a playable character
		/// </summary>
		/// <param name="npcId"></param>
		/// <returns></returns>
		public static bool IsPlayer(int npcId) {
			return npcId == NpcID.Yona || npcId == NpcID.Man;
		}

		/// <summary>
		/// Checks whether character is a playable character
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public static bool IsPlayer(CommonStates character)
		{
			if (character == null)
				return false;

			return IsPlayer(character.npcID);
		}

		/// <summary>
		/// Checks if common is a friend (friend npc, etc)
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		public static bool IsFriend(CommonStates common) {
			return common.employ != CommonStates.Employ.None;
		}
		
		public static bool IsFemale(CommonStates common) {
			return Managers.mn.randChar.GetSexual(common.npcID) == Gender.Female;
		}

		public static int GetGender(CommonStates common) {
			switch (Managers.mn.randChar.GetSexual(common.npcID)) {
			case Gender.Male:
				return Gender.Male;
			case Gender.Female:
				return Gender.Female;
			case Gender.Invalid:
				return Gender.Invalid;
			default:
				return Gender.Invalid;
			}
		}


		/// <summary>
		/// Checks if common is pregnant
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		public static bool IsPregnant(CommonStates common)
		{
			return common.pregnant[0] != -1;
		}

		/// <summary>
		/// Gets a display name for npc ID. This represents the type of NPC (not their "person name")
		/// </summary>
		/// <param name="npcId"></param>
		/// <returns></returns>
		public static string GetName(int npcId) {
			var name = NPCNames.GetValueOrDefault(npcId, "");

			if (name == "")
			{
				PLogger.LogWarning($"Unknown NPC ID: {npcId}");
				return $"Unknown (ID: {npcId})";
			}

			return name;
		}

		/// <summary>
		/// Gets a display name for common. This represents the type of NPC (not their "person name")
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		public static string GetName(CommonStates common) {
			return GetName(common?.npcID ?? NpcID.None);
		}

		/// <summary>
		/// Gets a string with several information describing the NPC:
		/// ID, Type name, Person name, Employment
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		public static string DetailedString(CommonStates common) {
			var id = common?.npcID ?? -1;
			var name = common?.charaName ?? "*null*";
			var objectName = common?.name ?? "*null*";
			var employ = common?.employ.ToString() ?? "*null*";

			return $"{name}|{objectName}|{id}|{employ}";
		}

		/// <summary>
		/// Gets a string for logging common. Includes the NPC name and NPC type
		/// </summary>
		/// <param name="common"></param>
		/// <returns></returns>
		public static string LogName(CommonStates common) {
			if (common == null) {
				return "*null*";
			}

			return $"{common.charaName} ({CommonUtils.GetName(common.npcID)})";
		}
	}
}