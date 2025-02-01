# ChangeLog

This file lists all the changes made to the mods released in this repository.

They are grouped by Release. Releases follows CalVer formatted as `YYYY.MM.DD` or `YYYY.MM.DD+N`
if multiple releases were made in the same day.

Each mod has its own version which follows SemVer.

## Unreleased

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
