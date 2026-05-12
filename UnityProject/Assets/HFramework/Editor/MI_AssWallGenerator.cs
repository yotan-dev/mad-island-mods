using HFramework.ScriptGenerator;
using HFramework.SexScripts;
using YotanModCore.Consts;

namespace HFramework
{
	public class MI_AssWallGenerator : BaseSexScriptGenerator
	{
		private class Config
		{
			public string UniqueID;
			public string Name = "AssWall";
			public string Description = "Original scene";
			public int FemaleNpcID;
			public bool RequiresDlc = false;
			public string MinVersion = "0.0.0";
		}

		private void Create(Config config) {
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.AssWall.asset",
				$"Assets/HFramework/Generated/AssWall/{config.UniqueID}.asset"
			) as AssWallScript;

			script.RequiresDLC = config.RequiresDlc;
			script.MinimalGameVersion = config.MinVersion;

			script.UniqueID = config.UniqueID;
			script.Name = config.Name;
			script.Description = config.Description;

			script.Info.Npcs[1].NpcID = config.FemaleNpcID;
		}

		public override void Generate() {
			// From GameManager::AssWallNPCCheck

			this.Create(new Config {
				UniqueID = "HF.AssWall.FemaleNative",
				FemaleNpcID = NpcID.FemaleNative, // 15
			});

			this.Create(new Config {
				UniqueID = "HF.AssWall.NativeGirl",
				FemaleNpcID = NpcID.NativeGirl, // 16
				RequiresDlc = true,
			});

			this.Create(new Config {
				UniqueID = "HF.AssWall.FemaleLargeNative",
				FemaleNpcID = NpcID.FemaleLargeNative, // 17
			});

			this.Create(new Config {
				UniqueID = "HF.AssWall.UnderGroundWoman",
				FemaleNpcID = NpcID.UndergroundWoman, // 44
				MinVersion = "0.2.2"
			});

			this.Create(new Config {
				UniqueID = "HF.AssWall.ElderSisterNative",
				FemaleNpcID = NpcID.ElderSisterNative, // 90
				MinVersion = "0.2.2"
			});

			this.Save();
		}
	}
}
