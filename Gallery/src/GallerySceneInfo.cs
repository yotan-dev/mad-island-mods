using System;
using System.Collections;
using YotanModCore;
using Gallery.SaveFile.Containers;
using UnityEngine;
using UnityEngine.UIElements;
using HFramework.Scenes;

namespace Gallery
{
	public class GallerySceneInfo
	{
		public class CharGroups {
			public static readonly string Yona = "Yona";
			public static readonly string Man = "Man";
			public static readonly string Giant = "Giant";
			public static readonly string Sally = "Sally";
			public static readonly string Mermaid = "Mermaid";
			public static readonly string Shino = "Shino";
			public static readonly string Merry = "Merry";
			public static readonly string NativeMale = "Native Male";
			public static readonly string BigNative = "Big Native";
			public static readonly string NativeFemale = "Native Female";
			public static readonly string FemaleLargeNative = "Female Large Native";
			public static readonly string NativeGirl = "Native Girl";
			public static readonly string OldWomanNative = "Old Woman Native";
			public static readonly string UndergroundWoman = "Underground Woman";
			public static readonly string Mummy = "Mummy";
			public static readonly string Specials = "Specials";
			public static readonly string Story = "Story";
		}

		public class SceneTypes {
			public const string AssWall = "Ass Wall (Back Toilet)";
			public const string Daruma = "Daruma";
			public const string Slave = "Slave";
			public const string PlayerRaped = "Player defeat";
			public const string ManRapes = "Player rapes";
			public const string CommonSexNpc = "NPC x NPC sex";
			public const string SleepRaes = "Sleep rapes";
			public const string CommonSexPlayer = "Player x NPC sex";
			public const string Toilet = "Player x NPC Toilet";
			public const string ToiletNPC = "Npc x NPC Toilet";
			public const string Delivery = "Delivery";
			public const string Story = "Story";
		}

		public class SceneNpc {
			public int NpcID { get; set; }
			public bool Pregnant { get; set; }
		}

		public class PlayData {
			public CommonStates NpcA { get; set; }
			public CommonStates NpcB { get; set; }
			public GameObject Prop { get; set; }
		}

		public string CharGroup { get; set; }
		public string SceneType { get; set; }
		public string Name { get; set; }
		public bool RequireDLC { get; set; } = false;
		public int MinVersion { get; set; } = 0;
		public SceneNpc NpcA { get; set; } = null;
		public SceneNpc NpcB { get; set; } = null;
		public string Prop { get; set; } = "";
		public ManRapeSleepState SleepRapeSexType { get; set; }

		public bool IsUnlocked;

		public Func<PlayData, IScene> GetScene { get; set; }
	}

}
