# Craft recipes

Craft recipes are a way to make a new item craftable in the game.

To create a new recipe you must perform 2 major steps:

1. Create the recipe itself
2. Register it to a YMCDataLoader (see [Your first static item](./your-first-item.md) if you don't know how to do this)


## Creating a recipe

To create a recipe:

1. Right click on your assets folder
2. Select `Create > Yotan Mod Core > Craft Recipe`
3. Give it a name (suggestion `<item_name>_recipe`)

Fill its fields with the desired data:

1. **Bench Item Key**: the item key of where this recipe will be available at
	- Use the item name when crafting on benches (e.g. `bench_wood`)
	- Use `@other:hand` when it is a craft that does not require a station (player "hand")
	- Use `@npc:<npc_name>` when it is a craft performed by a NPC. `<npc_name>` is the constant from Yotan Mod Core. (See: [YotanModCore.Consts.NpcID](/api/YotanModCore.Consts.NpcID.html))
	- **Note:** We don't support adding craft to new NPCs yet
2. **Item Key**: the item key of the item to be crafted
3. **Materials**: the list of required items to craft it.
	- Each entry must follow the pattern `<item_key>:<amount>`
	- Example: `wood_01:10` (10 Wood)

Now, include this asset in your Asset Bundle.


## Registering a recipe

To register it, open the `YMCDataLoader` of your Asset Bundle and add this newly created recipe to its `Recipes` list.
