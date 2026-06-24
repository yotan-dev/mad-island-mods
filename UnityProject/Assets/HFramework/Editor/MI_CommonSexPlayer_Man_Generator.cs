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
	public class MI_CommonSexPlayer_Man_Generator : BaseSexScriptGenerator
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

			var setupNode = script.MustFindNodeById<MakeSexPrefab>("MakeSexPrefab");
			var instantiator = setupNode.Instantiator as SexListPrefab;
			if (instantiator == null) {
				throw new System.Exception("SexListPrefab not found");
			}

			instantiator.ListIndex = config.ListIndex;
			instantiator.ObjIndex = config.ObjIndex;

			if (config.AnimationPrefix != "A_") {
				var animNodes = script.Nodes.FindAll((node) => node is LoopAnimationForTime && node.ID.StartsWith("Anim_"));
				foreach (var node in animNodes) {
					var animNode = (LoopAnimationForTime)node;
					animNode.AnimationName = config.AnimationPrefix + animNode.AnimationName[2..]; // removes the "A_" prefix
				}

				var animFinish = (AnimateOnce)script.Nodes.Find((node) => node is AnimateOnce && node.ID == "Anim_Finish");
				animFinish.AnimationName = config.AnimationPrefix + animFinish.AnimationName[2..]; // removes the "A_" prefix
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
				$"Assets/HFramework/Generated/CommonSexPlayer/Man/{config.UniqueID}.asset"
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

		private CommonSexPlayerScript CreateManxFemaleNative_2Pose(MxFConfig config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.CommonSexPlayer.ManxF_2Pose.asset",
				$"Assets/HFramework/Generated/CommonSexPlayer/Man/{config.UniqueID}.asset"
			) as CommonSexPlayerScript;

			this.SetupCommon(script, config);

			script.Info.Npcs[0].NpcID = config.MaleNpcID;
			script.Info.Npcs[1].NpcID = config.FemaleNpcID;
			script.Info.Npcs[1].Conditions = new NpcCondition[] {
				new Pregnant() {
					Pregnancy = config.Pregnancy
				}
			};

			// Remove Pose2 menu for Speed1
			var speed1bMenu = script.MustFindNodeById<LastChoiceEquals>("Menu_3a_speed1_b");
			var children = speed1bMenu.Children.ToArray();
			foreach (var child in children) {
				script.RemoveChild(speed1bMenu, child);
				script.DeleteNode(child);
			};

			var speed1aMenu = script.MustFindNodeById<SetMenuOptions>("Menu_speed1");
			speed1aMenu.Options.RemoveAll((opt) => opt.Id == "speed1_b");

			var menuNode = script.MustFindNodeById<MenuInteraction>("MenuInteraction");
			script.RemoveChild(menuNode, speed1bMenu);
			script.DeleteNode(speed1bMenu);

			// Fix Speed menu for Speed2. It should go back to Speed1/Pose1
			var speed2bMenu = script.MustFindNodeById<SetMenuOptions>("Menu_speed2_b");
			speed2bMenu.Options.Find((menu) => menu.Id == "speed1_b").Id = "speed1";

			// Fix Caress animation name
			script.MustFindNodeById<SetAnimation>("Anim_Contact").AnimationName = "A_Contact_01_{actors[1].tits}";
			script.MustFindNodeById<SetAnimation>("Anim_Loop02_b").AnimationName = "A_Loop_02_{actors[1].tits}";
			script.MustFindNodeById<AnimateOnce>("Anim_Finish_b").AnimationName = "A_Finish_00";
			script.MustFindNodeById<SetAnimation>("Anim_Finish_idle_b").AnimationName = "A_Finish_idle_00";

			return script;
		}

		public override void Generate() {
			/**
			 * Follow the cases from decompiled code, so it is easier to compare.
			 * Here we consider state = 0 / pCommon.npcID = 1 (Man)
			 *
			 * Each region is a nCommon.npcID
			 */

			 // @TODO: ToggleMan

			#region case 0 - Yona
			var yona0_2 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.0-2",
				Name = "0-2",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Yona, // 0
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 0,
				ObjIndex = 2
			});
			yona0_2.Info.StartConditions = new() { new(new QuestProgress("Main_Yona", 1, 4, 10)) };
			yona0_2.Info.ExecuteConditions = new() { new(new SexType(0)) };

			var yona0_3 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.0-3",
				Name = "0-3",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Yona, // 0
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 0,
				ObjIndex = 3
			});
			yona0_2.Info.StartConditions = new() { new(new QuestProgress("Main_Yona", 1, 4, 10)) };
			yona0_3.Info.ExecuteConditions = new() { new(new SexType(1)) };

			var yona0_4 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.0-4",
				Name = "0-4",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Yona, // 0
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 0,
				ObjIndex = 4
			});
			yona0_2.Info.StartConditions = new() { new(new QuestProgress("Main_Yona", 1, 4, 10)) };
			yona0_4.Info.ExecuteConditions = new() { new(new SexType(2)) };
			#endregion

			#region case 5 - Reika
			// Note: SexCheck goes for Main_Reika1 = 3 and Sub_Keigo = 0, 2, 4, 5
			// 2 is for 1_17, 0, 4, 5 is for 1_12.

			var reika1_17 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.1-17",
				Name = "1-17",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Reika, // 0
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 1,
				ObjIndex = 17
			});

			reika1_17.Info.StartConditions = new() {
				new(new QuestProgress("Main_Reika1", 3), new QuestProgress("Sub_Keigo", 2)),
			};
			reika1_17.Info.ExecuteConditions = new() { new(new SexType(1)) };

			var reika1_12 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.1-12",
				Name = "1-12",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Reika, // 0
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 1,
				ObjIndex = 12
			});

			reika1_12.Info.StartConditions = new() {
				new(new QuestProgress("Main_Reika1", 3), new QuestProgress("Sub_Keigo", 0, 4, 5)),
			};
			#endregion

			#region case 6 and 73 - Nami / SlenderYoungLady
			// Note: StartCondition is always run when ExecuteCondition is, so we don't need on both.
			var nami1_27 = this.CreateMxF(new MxFConfig {
				UniqueID = $"HF.CommonSexPlayer.1-27_nami",
				Name = "1-27",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Nami,
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 1,
				ObjIndex = 27
			});
			nami1_27.Info.StartConditions = new() { new(new QuestProgress("Sub_NamiMan", 2)) };
			// nami1_27.Info.ExecuteConditions = new() { new(new QuestProgress("Sub_NamiMan", 2)) };

			var nami1_26 = this.CreateMxF(new MxFConfig {
				UniqueID = $"HF.CommonSexPlayer.1-26_nami",
				Name = "1-26",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.Nami,
				Pregnancy = Pregnant.PregnancyStatus.All,
				ListIndex = 1,
				ObjIndex = 26
			});
			nami1_26.Info.StartConditions = new() { new(new QuestProgress("Sub_NamiMan", 3)) };
			// nami1_26.Info.ExecuteConditions = new() { new(new QuestProgress("Sub_NamiMan", 3)) };

			// I think it is a bug, but YoungLady has a different sex if you got to Sub_NamiMan = 3
			// Whici completely disables this one.
			var slender1_27 = this.CreateMxF(new MxFConfig {
				UniqueID = $"HF.CommonSexPlayer.1-27b_slender",
				Name = "1-27b",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.SlenderYoungLady,
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 1,
				ObjIndex = 27
			});
			slender1_27.Info.StartConditions = new() { new(new QuestProgress("Sub_NamiMan", 0, 1, 2 )) };
			// slender1_27.Info.ExecuteConditions = new() { new(new QuestProgress("Sub_NamiMan", 0, 1, 2 )) };

			var slender1_26 = this.CreateMxF(new MxFConfig {
				UniqueID = $"HF.CommonSexPlayer.1-26b_slender",
				Name = "1-26b",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.SlenderYoungLady,
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 1,
				ObjIndex = 26
			});
			slender1_26.Info.StartConditions = new() { new(new QuestProgress("Sub_NamiMan", 3 )) };
			// slender1_26.Info.ExecuteConditions = new() { new(new QuestProgress("Sub_NamiMan", 3 )) };
			#endregion

			#region case 9 - YoungLady
			var youngLady0_2 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.0-2_young",
				Name = "0-2",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.YoungLady, // 9
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 0,
				ObjIndex = 2
			});
			youngLady0_2.Info.ExecuteConditions = new() { new(new SexType(0)) };

			var youngLady0_3 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.0-3_young",
				Name = "0-3",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.YoungLady, // 9
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 0,
				ObjIndex = 3
			});
			youngLady0_3.Info.ExecuteConditions = new() { new(new SexType(1)) };

			var youngLady0_4 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.0-4_young",
				Name = "0-4",
				Description = "Original scene",
				AnimationPrefix = "A_",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.YoungLady, // 9
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 0,
				ObjIndex = 4
			});
			youngLady0_4.Info.ExecuteConditions = new() { new(new SexType(2)) };
			#endregion

			#region case 15 - Female Native
			this.CreateManxFemaleNative_2Pose(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.1-1",
				Name = "1-1",
				Description = "Original scene",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.FemaleNative, // 15
				Pregnancy = Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 1
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.1-20",
				Name = "1-20",
				Description = "Original scene",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.FemaleNative, // 15
				Pregnancy = ~Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 20
			});
			#endregion

			#region case 16 - Native Girl
			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.1-3",
				Name = "1-3",
				Description = "Original scene",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.NativeGirl, // 16
				Pregnancy = Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 3,
				RequiresDlc = true
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexPlayer.1-28",
				Name = "1-28",
				Description = "Original scene",
				MaleNpcID = NpcID.Man, // 1
				FemaleNpcID = NpcID.NativeGirl, // 16
				Pregnancy = ~Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 28,
				RequiresDlc = true
			});
			#endregion

			this.Save();
		}
	}
}
