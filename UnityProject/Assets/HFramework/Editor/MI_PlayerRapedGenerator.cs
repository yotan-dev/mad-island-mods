#nullable enable

using HFramework.ScriptGenerator;
using HFramework.ScriptNodes;
using HFramework.ScriptNodes.PrefabSetup;
using HFramework.SexScripts;
using YotanModCore.Consts;

namespace HFramework
{
	public class MI_PlayerRapedGenerator : BaseSexScriptGenerator
	{
		private class Config
		{
			public string UniqueID = "NoID";
			public string Name = "Forced";
			public string Description = "Original scene";
			public int MaleNpcID;
			public int ObjIndex;
			public AppearanceSetter? AppearanceSetter;
			public bool RequiresDlc = false;
			public string MinVersion = "0.0.0";
			public string AnimationPrefix = "A_";
		}

		private PlayerRapedScript SetupYona(Config config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.PlayerRaped.Yona.asset",
				$"Assets/HFramework/Generated/PlayerRaped/Yona/{config.UniqueID}.asset"
			) as PlayerRapedScript;
			if (script == null) {
				throw new System.Exception("Failed to create PlayerRapedScript");
			}

			script.Info.Npcs[0].NpcID = config.MaleNpcID;

			script.UniqueID = config.UniqueID;
			script.Name = config.Name;
			script.Description = config.Description;
			script.MinimalGameVersion = config.MinVersion;
			script.RequiresDLC = config.RequiresDlc;

			var setupNode = script.MustFindNodeById<MakeSexPrefab>("MakeSexPrefab");
			var instantiator = setupNode.Instantiator as SexObjPrefab;
			if (instantiator == null) {
				throw new System.Exception("SexObjPrefab not found");
			}

			instantiator.ObjIndex = config.MaleNpcID;

			if (config.AppearanceSetter != null) {
				setupNode.AppearanceSetter = config.AppearanceSetter;
			}

			return script;
		}

		public override void Generate() {
			/**
			 * Follow the cases from decompiled code, so it is easier to compare.
			 * As of v0.5.9, the game puts the smallest NPC ID first, so things like that may happen:
			 * - Male Native case (10), has a entry with Female Native
			 * - Female Native case (15) will have its own entry for other Female Native (15) or Young Boy (89)
			 *
			 * While this is messy when trying to look at the overall, it is probably easier to follow in code.
			 */

			var yonaMaleNpcIds = new int[]{
				NpcID.MaleNative, // 10
				NpcID.BigNative, // 11
			};
			foreach (var npcId in yonaMaleNpcIds) {
				this.SetupYona(new Config {
					UniqueID = $"HF.PlayerRaped.Yona_{npcId}",
					MaleNpcID = npcId,
					ObjIndex = npcId,
					AppearanceSetter = new MaleFemaleRandCharSetter(0, 1),
				});
			}

			// These only sets Female character appearance, with Male as null
			yonaMaleNpcIds = new int[]{
				NpcID.Bigfoot, // 25
				NpcID.Werewolf, // 35
				NpcID.OldGuy, // 100
				NpcID.Spike, // 101
				NpcID.Planton, // 103
				// NpcID.BossNative, // 104
			};
			foreach (var npcId in yonaMaleNpcIds) {
				this.SetupYona(new Config {
					UniqueID = $"HF.PlayerRaped.Yona_{npcId}",
					MaleNpcID = npcId,
					ObjIndex = npcId,
					AppearanceSetter = new FemaleRandCharSetter(1),
				});
			}

			this.SetupYona(new Config {
				UniqueID = "HF.PlayerRaped.Yona_104",
				MaleNpcID = NpcID.BossNative,
				ObjIndex = NpcID.BossNative,
				AppearanceSetter = new BossNativeCharSetter(0, 1),
			});

			// this.SetupYona(new Config {
			// 	UniqueID = "HF.PlayerRaped.Man_8-0",
			// 	MaleNpcID = NpcID.FemaleLargeNative, // 17
			// 	// @TODO: Battle + Sex animations
			// });
			// this.SetupYona(new Config {
			// 	UniqueID = "HF.PlayerRaped.Man_1-6",
			// 	MaleNpcID = NpcID.Mummy, // 42
			// 	// @TODO: Battle + Sex animations / Handjob
			// });

			this.Save();
		}
	}
}
