# Mad Island Mods

Mods for Mad Island game.


## Before you begin

Before you start, you need to know and understand that:

1. Every mod here can potentially break your game. They are changing the game code in ways the devs did not expect/know;
2. If you find a bug, check BepInEx console to check if a mod is not causing issues.
3. **Before** you report a bug to the game developer, try reproducing the bug without mods.

Let's not make the life of the developers harder. And responsibly enjoy a modded game :)


## Supported version

Since the game is in early access and getting active updates, the real support is focused in the latest version.

Unless otherwise noted in the mod section:

1. The current main version of the game should be supported;
2. The current beta (if any) version of the game should be supported;
3. versions >= old and < current are supported in a "best effort approach".
   1. If something breaks, I will try to add a workaround, but won't test it.
   2. New stuff also won't be tested.

When a new version is released, it is likely to break mods (see BepInEx console!!),
and I will fix it once I notice/have time. Feel free to contribute if you want.


## Installation

Before installing any mod, you must have BepInEx installed.

See installation instructions at: https://docs.bepinex.dev/articles/user_guide/installation/index.html

You need to install x64 version of BepInEx v5 (latest stable v5 available).

After you have it installed, download the mods that you need (and requirements, if applicable) and extract the files into `Mad Island Folder/BepInEx/plugins`.

If the mod contains a single dll, the dll must be laying in the `plugins` folder (in other words, `Mad Island Folder/BepInEx/plugins/modname.dll` should exist).

If the mod contains a folder with 2 or more files, this folder must be in `plugins` one (in other words, `Mad Island Folder/BepInEx/plugins/modname/modname.dll` should exist).

If the mod mentions configuration, you should first start the game once so configuration files get generated, shut down the game and configure it at `BepInEx/config/` folder.


## Mod list

### Yotan Mod Core

This is an utility library for mods. Most mods require it, but it doesn't bring anything into the game by itself.

**For mod developers:**

You can check its API directly in the source code. I tried to keep it more or less documented. Feel free to depend on it.

It will follow semantic versioning and I will write about deprecations/removals when they come.

Please let me know if you are depending on it :)


### Gallery

By far the most ambitious of those mods. This implements a Gallery of game scenes.

![gallery](docs/imgs/gallery.png)

1. After it gets installed, a new "Gallery" button is added to the main menu. The Gallery will only show scenes that you have unlocked.
2. To unlock a scene, you must execute it until "the end" (e.g. orgasm reached)

**Important:**

1. Currently, I am tracking many scenes that are not in Gallery yet
2. I am likely not tracking a scene or two, which once I found out may come out blocked. But most of them should be covered.

**Requirements:**

1. Yotan Mod Core must be installed.

**Technical notes:**

Progress is tracked in "GalleryState.xml" in Mad Islands root folder. This file is not saved to Steam Cloud.


### Craft Colors

Shows what is missing to craft an item in Red.

![craft](docs/imgs/craft.png)

**Requirements:**

1. Yotan Mod Core must be installed.


### Disassemble Items

Crafted something by mistake? Don't need it any longer? Press `T` and get the materials back.

**Requirements:**

1. Yotan Mod Core must be installed.


### Enhance Workplaces

Does some improvements to NPC work.

Officially, NPCs works every few seconds, for 20 seconds, performing an action which generates 1 item. This is always like that.

This mod changes it a bit:

- There is a random factor over how much items an NPC will get.
- The random factor is improved by NPC level and morale.
- There is also a cap (so they don't get too much).

It currently affects Mining and Logging. When the drop of an item does happen, the amount will be:

- Lv1 items: (stone / wood)
  - Gives between 1 and 1 + Bonus items, capped at 10
  - Bonus starts at 0 and is increased by 1 every 10 levels or 25 moral
- Lv2 items: (iron / sap / wood 2)
  - Gives between 1 and 1 + bonus items, capped at 5
  - Bonus starts at 0 and is increased by 1 every 20 levels or 50 moral
- Lv3 items: (diamond)
  - Gives between 1 and 1 + bonus items, capped at 3
  - Bonus starts at 0 and is increased by 1 every 30 levels or 75 moral

Also, when doing Harvesting, if the NPC has a Fishing rod in their first inventory
slot, they will first try to collect any Fishtrap that has fishes in it.

**Requirements:**

1. Yotan Mod Core must be installed.


### Increase Zoom

Allows you to zoom out much further.

![zoom](docs/imgs/zoom.png)

**Note:** Zooming out is likely to use more PC resources, since the clipping panes/etc are also increased.


### Item slot with colors

Adds item color marker to item slots. Only for items that are dyeable.

![alt text](docs/imgs/slotcolor.png)

**Requirements:**

1. Yotan Mod Core must be installed.


### NPC Stats

Properly give stats points and applies then to NPCs when they are born/friendlied.

I made this mod because I noticed that high level captured NPCs or new borns were too weak, making more sense to level
one from the scratch. This makes them get real points and randomly distributes them.

**Requirements:**

1. Yotan Mod Core must be installed.


### Stack nearby

Press `V` to stack items from your inventory into all nearby chests.

**Note:** It lags a bit


### Warp body

Press `P` while checking the inventory of a body to warp them to the nearest "respawn" point.

Useful when you get a pile of body near your base.

**Requirements:**

1. Yotan Mod Core must be installed.


### YoUnnoficialPatches

Bug fixes implemented via modding. Let me know if there are other bugs that could be looked into.

Included:

- Translation of some boss names from Japanese to English
- Translation of Quest UI titles to english
- Translation of Surgery table / Cute Mirror to english
- Translation of Craft "Ingredients", Shop "Buy", and "Paused" texts
- Translation of Dye table UI from Japanese to English


I won't fix:

- Friend NPCs attacking new childs (without being able to hit) -- too hard to change with mods
- Automatically moving to the right -- I could not find the cause, looks like out of the game

**Requirements:**

1. Yotan Mod Core must be installed.


## Compatibility with Melon Loader

You can use Melon Loader mods together with BepInEx mods. But to do that, you **CAN'T** install Melon Loader directly. Instead you have to:

1. Install BepInEx: https://docs.bepinex.dev/articles/user_guide/installation/index.html
2. Install BepInEx Melon Loader plugin: https://github.com/BepInEx/BepInEx.MelonLoader.Loader
3. Put your MelonLoader mods inside MLLoader/Mods


## Support

If you find a bug, have questions or feature requests, feel free to open an issue. Feature requests may not be implemented by me.


## Contributing

Contributions are welcome. For small changes/bug fixes, just fork it and open a PR and I will review.

For bigger changes/new features, please open an issue for discussion first.


## Development Setup

This project is currently using [ThunderKit](https://github.com/PassivePicasso/ThunderKit) -- too much? yeah... maybe, I thought it would be great for the Gallery.

You must install Unity 2021.3.6f and open this repository in it. Install ThunderKit and import MadIsland files to it.

This should be enough to give you the list of pipelines and allow you to build the plugins.

**NEVER** commit original game files.


## License

- Some code is copied/very similar to original decompiled source, those are kinda of propietary to Mad Island developers;
- Most of the code is made by me and is free to be reused as long as kept free and open source (be it as git repo or source distributed together with your mod).


## Project status

I think most of the mods have reached their "end state", while I do think I will come back from time to time, please note that I may not always be active.

Feel free to contribute, reviewing PRs are easier than making the changes, so it is much appreciated :)
