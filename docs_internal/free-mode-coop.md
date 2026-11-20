# Make HFramework custom scenes replicate in Free Mode co-op via custom family

Add a Harmony-patched family=4 to FreeInteractionPrefabBinding.TryResolve + FreeInteractionVisual.Valid backed by an HFramework prefab registry, spawn via FreeInteractionPrefabBinding.Create, and bind FreeInteractionSource.Observe so remote clients see streamed visuals — without touching sexList.

## Summary

The Free Mode co-op layer replicates sex visuals by sending `{family, list, index, signature}` and re-resolving the prefab remotely via `FreeInteractionPrefabBinding.TryResolve`. Vanilla supports families 0–3 only. Instead of mutating `sexMN.sexList`, HFramework registers family **4** = "mod registry": `list` = a stable per-mod key (e.g. hash of plugin GUID), `index` = slot in that mod's deterministically-sorted prefab registry. Two Harmony patches (TryResolve + Visual.Valid) enable it on both capture and resolve paths; everything no-ops without Free Mode.

## Implementation Steps

1. **Prefab registry** (`HFramework/src/Runtime/FreeMode/FreeModeRegistry.cs` or similar)
   - Mods register scene prefabs with a stable string key (plugin GUID + scene ID).
   - Slots assigned by ordinal sort on the key → identical `index` on every client.
   - `list` = deterministic 16-bit key derived from mod GUID (e.g. first 2 bytes of SHA256/GUID), so multiple mods can't collide on index 0.
   - Signature: replicate the vanilla scheme — SHA256 over prefab name + skeletonDataAsset name + skeleton hash (fallback: sorted animation name/duration list) → 64-hex string. Must satisfy `FreeInteractionVisual.Signature()` (64 hex chars).

2. **Harmony patches** (`HFramework/src/Runtime/FreeMode/FreeModePatches.cs`) — apply only if `MadIsland.FreeMode` types exist (soft/reflection check or try-catch):
   - **Postfix `FreeInteractionPrefabBinding.TryResolve`**: when `family == 4` and original returned false → resolve prefab from registry by `(list, index)`, compute signature, set `__result`/outs.
   - **Postfix `FreeInteractionVisual.Valid`**: when `__result == false` and `value.family == 4` → temporarily remap `family` to 0/`list` to -1 and re-invoke `Valid` (postfix won't recurse since family is no longer 4). This preserves all other validation (signature format, vectors, tracks, slots).

3. **Spawn helper** — replace `GameObject.Instantiate(prefab, pos, rot)` at scene spawn sites with a helper that looks up the prefab's `(family, list, index)` from the registry and calls `FreeInteractionPrefabBinding.Create(sexMN, family, list, index, prefab, pos, rot)` (public static; falls back to plain Instantiate when unregistered or Free Mode absent). Vanilla-prefab scenes can keep using selectors and map to families 0/1.

4. **Observe calls** — after spawn + getting `SkeletonAnimation`, call `FreeInteractionSource.Observe(visual, playerCommon, npcCommon, anim)` (player scenes) or `ObserveNpcPair`/`ObserveNpcSolo` (NPC scenes), mirroring vanilla call sites. Teardown is automatic (OnDisable/OnDestroy → EndSource).

5. **Call sites to update** (~9 TODO-marked + script nodes): `Scenes/Slave.cs:176`, `OnaniNPC.cs:94`, `PlayerRaped.cs:157,183`, `ManRapesSleep.cs:191`, `ManRapes.cs:124`, `Daruma.cs:125`, `CommonSexNPC.cs:186`, `CommonSexPlayer.cs:277`, `ScriptNodes/PrefabSetup/SexListPrefab.cs`, `SexObjPrefab.cs`, `CustomPrefab.cs`.

## Files to Create/Modify

- NEW `HFramework/src/Runtime/FreeMode/FreeModeRegistry.cs` — registry + signature computation
- NEW `HFramework/src/Runtime/FreeMode/FreeModePatches.cs` — the two Harmony patches
- NEW `HFramework/src/Runtime/FreeMode/FreeModeSpawn.cs` — spawn+observe helper (reflection-safe)
- `HFramework/src/Runtime/Scenes/*.cs`, `ScriptNodes/PrefabSetup/*.cs` — swap Instantiate calls
- `HFramework` plugin init — register prefabs + apply patches

## Verification

- [ ] Build; solo play unchanged (Free Mode absent → everything no-ops)
- [ ] Two-client Free Mode: start custom scene on one peer → remote sees animated visual
- [ ] Remote `TryResolve`+signature match succeeds (log failures)
- [ ] Visual despawns remotely when scene ends / actor state invalidates
- [ ] Two mods both using family 4 with different `list` keys don't collide

## Risks/Considerations

- **Determinism**: registry index must be identical on all peers — sorted by stable string key, not load order. Mismatched mod sets → signature mismatch → remote silently shows nothing (no crash).
- **All clients need the patch**: family 4 visuals are dropped as invalid on unpatched clients — acceptable (just invisible).
- **Prefab requirements**: needs `SkeletonAnimation` + valid `skeletonDataAsset` for signature generation.
- **12m validation**: host rejects visuals >12m from actor entities.
- **Visuals only**: remote clients see replayed animation; scene logic/events stay owner-side.
- **list-key collision**: 16-bit hash of plugin GUID — astronomically unlikely; could add a manifest check later.
