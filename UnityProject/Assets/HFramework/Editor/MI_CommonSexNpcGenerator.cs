using System.Collections.Generic;
using HFramework.ScriptGenerator;
using HFramework.ScriptNodes;
using HFramework.ScriptNodes.PrefabSetup;
using HFramework.SexScripts;
using HFramework.SexScripts.Info.NpcConditions;
using YotanModCore.Consts;

namespace HFramework
{
	public class MI_CommonSexNpcGenerator : BaseSexScriptGenerator
	{
		private class Config
		{
			public string UniqueID;
			public string Name;
			public string Description;
			public int ListIndex;
			public int ObjIndex;
			public bool RequiresDlc = false;
			public string MinVersion = "0.0.0";
			public string AnimationPrefix = "A_";
		}

		private class MxFConfig : Config
		{
			public int MaleNpcID;
			public int FemaleNpcID;
			public Pregnant.PregnancyStatus Pregnancy = Pregnant.PregnancyStatus.NotPregnant;
		}

		private class FxFConfig : Config
		{
			public int FemaleANpcID;
			public Pregnant.PregnancyStatus APregnancy = Pregnant.PregnancyStatus.NotPregnant;
			public int FemaleBNpcID;
			public Pregnant.PregnancyStatus BPregnancy = Pregnant.PregnancyStatus.NotPregnant;
		}

		private void SetupCommon(CommonSexNPCScript script, Config config) {
			script.UniqueID = config.UniqueID;
			script.Name = config.Name;
			script.Description = config.Description;

			var setupNode = script.nodes.Find(node => node.ID == "Setup") as Setup;
			if (setupNode == null) {
				throw new System.Exception("Setup node not found");
			}

			var instantiator = setupNode.Instantiator as SexListPrefab;
			if (instantiator == null) {
				throw new System.Exception("SexListPrefab not found");
			}

			instantiator.listIndex = config.ListIndex;
			instantiator.objIndex = config.ObjIndex;

			if (config.AnimationPrefix != "A_") {
				var animNodes = script.nodes.FindAll((node) => node is LoopAnimationForTime && node.ID.StartsWith("Anim_"));
				foreach (var node in animNodes) {
					var animNode = (LoopAnimationForTime)node;
					animNode.animName = config.AnimationPrefix + animNode.animName[2..]; // removes the "A_" prefix
				}

				var animFinish = (AnimateOnce)script.nodes.Find((node) => node is AnimateOnce && node.ID == "Anim_Finish");
				animFinish.animationName = config.AnimationPrefix + animFinish.animationName[2..]; // removes the "A_" prefix
			}
		}


		private CommonSexNPCScript CreateMxF(MxFConfig config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.CommonSexNPC.MxF.asset",
				$"Assets/HFramework/Generated/CommonSexNpc/{config.UniqueID}.asset"
			) as CommonSexNPCScript;

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

		private CommonSexNPCScript CreateFxF(FxFConfig config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.CommonSexNPC.MxF.asset",
				$"Assets/HFramework/Generated/CommonSexNpc/{config.UniqueID}.asset"
			) as CommonSexNPCScript;

			this.SetupCommon(script, config);

			script.Info.Npcs[0].NpcID = config.FemaleANpcID;
			script.Info.Npcs[0].Conditions = new NpcCondition[] {
				new Pregnant() {
					Pregnancy = config.APregnancy
				}
			};

			script.Info.Npcs[1].NpcID = config.FemaleBNpcID;
			script.Info.Npcs[1].Conditions = new NpcCondition[] {
				new Pregnant() {
					Pregnancy = config.BPregnancy
				}
			};

			var sequenceNode = script.nodes.Find((node) => node.ID == "Sequence") as Sequence;
			if (sequenceNode == null) {
				throw new System.Exception("Sequence node not found");
			}

			var emitCumNode = sequenceNode.children.Find((child) => child.ID == "Emit_Cum");
			if (emitCumNode == null) {
				throw new System.Exception("Emit_Cum node not found");
			}

			script.RemoveChild(sequenceNode, emitCumNode);
			script.nodes.Remove(emitCumNode);

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

			#region case 5 - Reika
			// Note: SexCheck skips the DueDate check when sexLimit == 0,
			//       but it is later blocked in CommonSexNpc, probably a bug and does not make a difference for us.
			var reika4_0 = this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.4-0",
				Name = "4-0",
				Description = "Original scene",
				AnimationPrefix = "Love_A_",
				MaleNpcID = NpcID.Keigo, // 8
				FemaleNpcID = NpcID.Reika, // 5
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 4,
				ObjIndex = 0
			});
			reika4_0.Info.StartConditions = new () {
				new SexScripts.Info.ConditionGroup() {
					Conditions = new SexScripts.Info.Conditions.Condition[] {
						new SexScripts.Info.Conditions.QuestProgress() {
							QuestName = "Main_Reika1",
							QuestValues = new int[] { 2 },
						}
					}
				},
				new SexScripts.Info.ConditionGroup() {
					Conditions = new SexScripts.Info.Conditions.Condition[] {
						new SexScripts.Info.Conditions.QuestProgress() {
							QuestName = "Sub_Keigo",
							QuestValues = new int[] { 1 },
						}
					}
				},
			};
			#endregion

			#region case 7 - Takumi
			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.6-1",
				Name = "6-1",
				Description = "Original scene",
				AnimationPrefix = "Love_A_",
				MaleNpcID = NpcID.Takumi, // 7
				FemaleNpcID = NpcID.YoungLady, // 9
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 6,
				ObjIndex = 1
			});
			#endregion

			#region case 10 - Male Native

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.2-0",
				Name = "Doggystyle",
				Description = "Original scene",
				MaleNpcID = NpcID.MaleNative, // 10
				FemaleNpcID = NpcID.FemaleNative, // 15
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 2,
				ObjIndex = 0
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.2-1",
				Name = "2-1",
				Description = "Original scene",
				MaleNpcID = NpcID.MaleNative, // 10
				FemaleNpcID = NpcID.NativeGirl, // 16
				Pregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 2,
				ObjIndex = 1,
				RequiresDlc = true
			});

			#endregion

			#region case 15 - Female Native

			this.CreateFxF(new FxFConfig {
				UniqueID = "HF.CommonSexNPC.15-15",
				Name = "15-15",
				Description = "Original scene",
				FemaleANpcID = NpcID.FemaleNative, // 15
				APregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				FemaleBNpcID = NpcID.FemaleNative, // 15
				BPregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 9,
				ObjIndex = 0
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.1-1",
				Name = "1-1",
				Description = "Original scene",
				MaleNpcID = NpcID.YoungMan, // 89
				FemaleNpcID = NpcID.FemaleNative, // 15
				Pregnancy = Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 1
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.1-20",
				Name = "1-20",
				Description = "Original scene",
				MaleNpcID = NpcID.YoungMan, // 89
				FemaleNpcID = NpcID.FemaleNative, // 15
				Pregnancy = ~Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 20
			});

			#endregion

			#region case 16 - Native Girl

			this.CreateFxF(new FxFConfig {
				UniqueID = "HF.CommonSexNPC.10-0",
				Name = "10-0",
				Description = "Original scene",
				FemaleANpcID = NpcID.NativeGirl, // 16
				APregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				FemaleBNpcID = NpcID.NativeGirl, // 16
				BPregnancy = ~Pregnant.PregnancyStatus.PregnantReady,
				ListIndex = 10,
				ObjIndex = 0,
				RequiresDlc = true
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.1-3",
				Name = "1-3",
				Description = "Original scene",
				MaleNpcID = NpcID.YoungMan, // 89
				FemaleNpcID = NpcID.NativeGirl, // 16
				Pregnancy = Pregnant.PregnancyStatus.NotPregnant,
				ListIndex = 1,
				ObjIndex = 3,
				RequiresDlc = true
			});

			this.CreateMxF(new MxFConfig {
				UniqueID = "HF.CommonSexNPC.1-28",
				Name = "1-28",
				Description = "Original scene",
				MaleNpcID = NpcID.YoungMan, // 89
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
