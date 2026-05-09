# Game Update Process

This is for project maintainers.

What needs to be done whenever Mad Island releases a new version.

## Assemblies update
1. Go to AssemblyStripper and generate the updated assemblies:
	- `dotnet run v0.5.10.0 "D:\SteamLibrary\steamapps\common\Mad Island/"`

## GameInfo update
1. Copy the new build date (the one that appears on the main menu)
2. Update YotanModCore GameInfo.cs version map (Build Date -> Patch note version)

## Asset Info/Constants update
1. Use AssetRipper to dump all assets into a new folder (e.g. `Assets_v05100`)
2. Run `generate` script to update constants
	- `npm.cmd run generate ..\..\Assets_v05100\`

## Code updates
1. Use dotPeek to dump the code
2. Compare the new code with the old code to see what has changed

### SexManager
Changes to SexManager will require updates to HFramework definition files

### InventoryManager::SubInventoryLoad
This method is used to load the crafting station recipes. If it changes, we need to update the CraftDB.cs file.

### UIManager::NPCMenuOpen
This method is used to load the NPC crafting station recipes. If it changes, we need to update the CraftDB.cs file.

