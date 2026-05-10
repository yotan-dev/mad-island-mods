# Generators

Generators allow you to bulk create sex scripts via code. It is very usefull when you need to create many similar scripts,
just adjusting one or two parameters. This way, you can create a template and generate multiple scripts with different parameters,
and in case in the future you need to update the template, you can just regenerate the scripts and they will all be updated.

Picking from the example in [templates](./templates.md) section, where we have many similar scripts for different NPCs,
you could create a generator that creates scripts for different NPCs, adjusting only the NPC parameter.

Generators are just a fancy name for an Editor Script that implements the `ISexScriptGenerator` interface. Usually, you want to extend `BaseSexScriptGenerator` class, which provides some useful methods for creating scripts from templates.

After the script is created, you can go right click the folder and use `HFramework Tools > Run script generators` and wait for it to finish.

Let's look at an example: (This is a trimmed version of HFramework's own generator - you can see the complete version [here](https://github.com/yotan-dev/mad-island-mods/blob/main/UnityProject/Assets/HFramework/Editor/MI_CommonSexNpcGenerator.cs))

```csharp
namespace HFramework
{
	public class MI_CommonSexNpcGenerator : BaseSexScriptGenerator
	{
		private class Config
		{
			public string UniqueID;
			public string Name;
			public int MaleNpcID;
			public int FemaleNpcID;
			public int ListIndex;
			public int ObjIndex;
		}

		private void SetupCommon(Config config) {
			// Create a new script from template
			var script = this.CreateFromTemplate(
				"Packages/com.yotan-dev.hframework/Runtime/SexScriptTemplates/HF.Template.CommonSexNPC.MxF.asset",
				$"Assets/HFramework/Generated/CommonSexNpc/{config.UniqueID}.asset"
			) as CommonSexNPCScript;

			// Sets the script basic data (Unique ID / Name / NPC IDs)
			script.UniqueID = config.UniqueID;
			script.Name = config.Name;
			script.Info.Npcs[0].NpcID = config.MaleNpcID;
			script.Info.Npcs[1].NpcID = config.FemaleNpcID;

			// Set the ListIndex/ObjIndex
			var instantiator = script.MustFindNodeById<MakeSexPrefab>("MakeSexPrefab").Instantiator as SexListPrefab;
			if (instantiator == null) {
				throw new System.Exception("SexListPrefab not found");
			}

			instantiator.ListIndex = config.ListIndex;
			instantiator.ObjIndex = config.ObjIndex;
		}

		public override void Generate() {
			// Script 1
			this.SetupCommon(new Config {
				UniqueID = "HF.CommonSexNPC.2-0",
				Name = "Doggystyle",
				MaleNpcID = NpcID.MaleNative, // 10
				FemaleNpcID = NpcID.FemaleNative, // 15
				ListIndex = 2,
				ObjIndex = 0
			});

			// Script 2 - This one has some additional conditions
			var reika4_0 = this.SetupCommon(new Config {
				UniqueID = "HF.CommonSexNPC.4-0",
				Name = "4-0",
				MaleNpcID = NpcID.Keigo, // 8
				FemaleNpcID = NpcID.Reika, // 5
				ListIndex = 4,
				ObjIndex = 0
			});
			// Here we include conditions of this script
			reika4_0.Info.StartConditions = new () {
				/* Path 1 */ new(new QuestProgress("Main_Reika1", 2)),
				/* Path 2 */ new(new QuestProgress("Sub_Keigo", 1)),
			};

			// Make sure everything is properly saved in Unity.
			this.Save();
		}
	}
}

```
