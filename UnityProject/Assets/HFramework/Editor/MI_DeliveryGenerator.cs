using HFramework.ScriptGenerator;
using HFramework.SexScripts;
using HFramework.SexScripts.Info.NpcConditions;
using YotanModCore.Consts;

namespace HFramework
{
	public class MI_DeliveryGenerator : BaseSexScriptGenerator
	{
		private class Config
		{
			public string UniqueID;
			public string Name = "Delivery";
			public string Description = "Original scene";
			public int NpcID;
			public bool RequiresDlc = false;
			public string MinVersion = "0.0.0";
		}

		private void Create(Config config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.Delivery.asset",
				$"Assets/HFramework/Generated/Delivery/{config.UniqueID}.asset"
			) as DeliveryScript;

			script.RequiresDLC = config.RequiresDlc;
			script.MinimalGameVersion = config.MinVersion;

			script.UniqueID = config.UniqueID;
			script.Name = config.Name;
			script.Description = config.Description;

			script.Info.Npcs[0].NpcID = config.NpcID;
			script.Info.Npcs[0].Conditions = new NpcCondition[] {
				new Pregnant(Pregnant.PregnancyStatus.PregnantReady)
			};
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

			this.Create(new Config {
				UniqueID = "HF.Delivery.FemaleNative",
				NpcID = NpcID.FemaleNative, // 15
			});

			this.Create(new Config {
				UniqueID = "HF.Delivery.NativeGirl",
				NpcID = NpcID.NativeGirl, // 16
				RequiresDlc = true,
			});

			this.Create(new Config {
				UniqueID = "HF.Delivery.FemaleLargeNative",
				NpcID = NpcID.FemaleLargeNative, // 17
				MinVersion = "0.4.2"
			});

			this.Create(new Config {
				UniqueID = "HF.Delivery.UnderGroundWoman",
				NpcID = NpcID.UndergroundWoman, // 44
				MinVersion = "0.4.2"
			});

			this.Create(new Config {
				UniqueID = "HF.Delivery.Yona",
				NpcID = NpcID.Yona, // 0
				MinVersion = "0.5.0"
			});
			this.Create(new Config {
				UniqueID = "HF.Delivery.YoungLady",
				NpcID = NpcID.YoungLady, // 9
				MinVersion = "0.5.0"
			});

			this.Create(new Config {
				UniqueID = "HF.Delivery.Daughter",
				NpcID = NpcID.Daughter, // 144
				MinVersion = "0.5.9",
				RequiresDlc = true
			});
			this.Create(new Config {
				UniqueID = "HF.Delivery.UnderGroundGirl",
				NpcID = NpcID.UndergroundGirl, // 142
				MinVersion = "0.5.9",
				RequiresDlc = true
			});
			this.Create(new Config {
				UniqueID = "HF.Delivery.LargeNativeGirl",
				NpcID = NpcID.LargeNativeGirl, // 140
				MinVersion = "0.5.9",
				RequiresDlc = true
			});


			this.Save();
		}
	}
}
