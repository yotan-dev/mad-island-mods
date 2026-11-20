using HFramework.ScriptGenerator;
using HFramework.ScriptNodes;
using HFramework.ScriptNodes.Animation;
using HFramework.ScriptNodes.Menu;
using HFramework.ScriptNodes.PrefabSetup;
using HFramework.SexScripts;
using HFramework.SexScripts.Info.Conditions;
using HFramework.SexScripts.Info.NpcConditions;
using YotanModCore.Consts;

namespace HFramework
{
	/// <summary>
	/// Generator for player controlling Reika sex scripts
	/// </summary>
	public class MI_CommonSexPlayer_Reika_Generator : BaseSexScriptGenerator
	{
		private class Config
		{
			public string UniqueID;
			public string Name;
			public string Description;
			public int ListIndex;
			public int ObjIndex;
			public bool RequiresDlc = false;
			public bool CanToggleMan = false;
			public string MinVersion = "0.0.0";
			public string AnimationPrefix = "A_";
		}

		private class MxFConfig : Config
		{
			public int MaleNpcID;
			public int FemaleNpcID;
			public Pregnant.PregnancyStatus Pregnancy = Pregnant.PregnancyStatus.NotPregnant;
		}

		private void SetupCommon(CommonSexPlayerScript script, Config config) {
			script.UniqueID = config.UniqueID;
			script.Name = config.Name;
			script.Description = config.Description;
			script.MinimalGameVersion = config.MinVersion;
			script.RequiresDLC = config.RequiresDlc;

			var setupNode = script.MustFindNodeById<MakeSexPrefab>("MakeSexPrefab");
			var instantiator = setupNode.Instantiator as SexListPrefab;
			if (instantiator == null) {
				throw new System.Exception("SexListPrefab not found");
			}

			instantiator.ListIndex = config.ListIndex;
			instantiator.ObjIndex = config.ObjIndex;

			if (config.AnimationPrefix != "A_") {
				var animNodes = script.Nodes.FindAll((node) => node is IAnimationNode animNode && animNode.GetAnimationName().StartsWith("A_"));
				foreach (var node in animNodes) {
					var animNode = (IAnimationNode)node;
					animNode.SetAnimationName(config.AnimationPrefix + animNode.GetAnimationName()[2..]); // removes the "A_" prefix
				}
			}

			if (!config.CanToggleMan) {
				script.Nodes
					.FindAll((node) => node is SetMenuOptions menu)
					.ForEach((node) => {
						var menu = node as SetMenuOptions;
						menu.Options.RemoveAll((opt) => opt.Id == "toggleMan");
					});
			}
		}


		private CommonSexPlayerScript CreateMxF(MxFConfig config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.CommonSexPlayer.ManxF.asset",
				$"Assets/HFramework/Generated/CommonSexPlayer/Reika/{config.UniqueID}.asset"
			) as CommonSexPlayerScript;

			this.SetupCommon(script, config);

			script.Info.Npcs[0].NpcID = config.MaleNpcID;
			script.Info.Npcs[1].NpcID = config.FemaleNpcID;
			script.Info.Npcs[1].Conditions = new NpcCondition[] {
				new Pregnant() {
					Pregnancy = config.Pregnancy
				}
			};

			return script;
		}
		public override void Generate() {
			/**
			 * Follow the cases from decompiled code, so it is easier to compare.
			 * Here we consider state = 0 / pCommon.npcID = 5 (Reika)
			 *
			 * Each region is a nCommon.npcID
			 */

			#region Case 8 - Keigo
			var keigo4_0 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.4-0",
				Name = "4-0",
				Description = "Original scene",
				AnimationPrefix = "Love_A_",
				MaleNpcID = NpcID.Keigo, // 8
				FemaleNpcID = NpcID.Reika, // 5
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 4,
				ObjIndex = 0
			});
			#endregion
			this.Save();
		}
	}
}
