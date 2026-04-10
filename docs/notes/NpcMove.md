# Npc Move

## searchAngle

When 0, NPCs will not detect/chase enemies, effective "stopping" them from reacting.

This is very important to be taken into account, because Sex scenes usually does this:

```
common.nMove.actType = NPCMove.ActType.Wait;
common.nMove.searchAngle = 0f; // or 180f
```

`NPCMove.ActType.Wait` will just stop the current action. But the tick for the next "AI state" will continue.
Once the tick is reached, if the character has `searchAngle` != 0, it will search for potential enemies and switch states.

If the sex scene is not properly ready for that, the NPC will fall through the world.


Thus:

- use `searchAngle = 0` to prevent them from reacting to enemies
- use `searchAngle = 180f` to allow them to react to enemies -- And make sure to handle if ActType goes to != Wait (you need to e.g. restore their rigid body)
