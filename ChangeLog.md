# ChangeLog

This file lists all the changes made to the mods released in this repository.

They are grouped by Release. Releases follows CalVer formatted as `YYYY.MM.DD` or `YYYY.MM.DD+N`
if multiple releases were made in the same day.

Each mod has its own version which follows SemVer.


## 2025.02.18

### HFramework v1.0.2

- Fix Yona ManRapes not working when Yona is killed instead of stunned


## 2025.02.17

### HFramework v1.0.1

- Fix Yona ManRapes not working
- Fix Female Large Native ManRapes not working


## 2025.02.15

> [!WARNING]
> ExtendedHSystem has been discontinued in favor of the new HFramework and HExtensions mods,
> please remove it from your game and install the other 2 mods.
>

### YotanModCore v1.6.0

Adds more functionalities, game infos and new console commands.

While most of the changes here are focused on mod developers, there is one for players:

- New console commands were added: `/heal`, `/healhp`, `/healst`, `/healfood`, `/healwater`, `/faint`, `/makevirgin`, `/moral`, `/stun`. Check the repo README for details.

Now for changes for Mod developers

- `GameInfo` now provides:
  - whether `JokeMode` is enabled/disabled
  - `CensorType` being applied to the game
  - `CensorBlockSize` being applied to the game
- It now provides an interface to create new Console Commands. See [README](./YotanModCore/README.md) for details
- New constants are now available: `AssWallState`, `BossBattleState`, `CommonSexPlayerState`, `ManRapesState`, `SexInfoIndex`, `ToiletState`.
- `GameLifecycleEvents` were added (See `Events.GameLifecycleEvents`)
- It now provides an interface to create custom buttons in the "Talk" area of NPC chat. See [README](./YotanModCore/README.md) for details
- PropPanels were improved. It now provides a better interface with `MenuItem` class. Please migrate existing menus.


### YoUnnoficialPatches v0.4.0 (and v0.3.0)

1. It now fixes some mosaics that the original decensoring did not clean up properly.
	The censor configuration of the original game is followed when defining how to clear.
	(Config: `FixMosaic` / default: true)
2. It now fixes NPCs trying to start a sex scene when they don't have an animation for it,
	in the vanilla game, this would make them walk to each other and stop.
	(Config: `DontStartInvalidSex` / default: true)

This requires YotanModCore v1.6.0 or newer


### [NEW] HFramework v1.0.0

This is the first release of the HFramework, a rewrite of how H-Scenes are
played in the game, making it pluggable for modders to customize and
play scenes as with more control over it.

This plugin does not do much by itself, but it will be required by other plugins
that touch the H-System of the game.

It requires YotanModCore v1.6.0.

Modders may check the "HFramework" folder README for details on how to use it.


### [NEW] HExtensions v0.1.0

This is the first version of HExtensions, this plugin aims at making some customization
over game's sex scenes. Most of the changes are toggleable.

It serves both as a mod with some cool contents but also as a showcase of the power of
HFramework.

It requires HFramework to work.

For this first release, the following is supported:

1. **Require Foreplay:** Characters refuses insertion if sex meter is not at least at some point.
2. **Dick painter:** Paints characters dick in red if female is virgin or applies a purpleish
	color if using a condom.

There is more to come in future versions!


### Gallery v1.0.0

> [!WARNING]
> 1. This release will reset your gallery progression, unfortunately I could not convert it.
> 2. It now requires HFramework v1.0.0
>

Rework the entire gallery system internals to use HFramework.

This release overhauls how we store your gallery progression, thus existing progression is lost.


## 2025.02.01

### NpcStats v3.0.0

This version overhauls how stats are calculated, bringing several options
and fixing a bug that was giving way more stats than it was supposed to.

In this new version, you can now configure how status are distributed for NPCs
in 3 different categories:

- Enemies
- Tamed NPCs
- Newborn NPCs

Also, this fixes an issue found in the old version: NPCs were gaining stats "twice".
In the original game, NPCs initial stats are done as if they had all stats = to their level.
With NPC Stats v1 and v2, we were still giving this bonus and additionally distributing stats
points over it. Making NPCs both friend and enemy much stronger than they should be.

In case you still want to use the old version, you can enable `ExtraStrong<Type>` options,
and combine it with `Random` for Distribution configs. This will make the base NPC use the mentioned formula.

See the description at the README for more details of how everything works now.
