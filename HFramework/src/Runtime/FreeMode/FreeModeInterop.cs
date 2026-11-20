using System;
using System.Runtime.CompilerServices;
using MadIsland.FreeMode;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.FreeMode
{
	/// <summary>
	/// Helpers for making mod-spawned sex visuals replicable by the Free Mode
	/// (multiplayer) co-op layer. Everything here is a safe no-op outside Free Mode,
	/// when no co-op session is running, or on game versions without Free Mode.
	///
	/// Usage:
	///  1. Spawn the scene visual with <see cref="SpawnVisual"/> instead of
	///     GameObject.Instantiate.
	///  2. After obtaining the SkeletonAnimation, call the matching Observe* method.
	///  3. Teardown requires nothing - the binding/source clean themselves up when
	///     the object is disabled or destroyed.
	///
	/// IMPORTANT: the MadIsland.FreeMode types only exist in game v0.6.0+. Any
	/// method that references them is marked NoInlining and must only be called
	/// after checking <see cref="Coop.IsSupported"/>.
	/// </summary>
	public static class FreeModeInterop
	{
		/// <summary>Family id used on the wire for mod-registered prefabs.</summary>
		public const int ModPrefabFamily = ModPrefabRegistry.Family;

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static bool IsFreeMode() {
			SexManager sexMN = Managers.sexMN;
			return sexMN != null && FreeModeScene.IsFreeMode(sexMN);
		}

		/// <summary>
		/// Registers a custom prefab so it can be replicated to remote clients in
		/// Free Mode co-op. Registration order does not matter (entries are indexed
		/// by sorted key) but all clients must register the same set of prefabs -
		/// typically achieved by registering every scene prefab at plugin init.
		/// </summary>
		/// <param name="prefab">The prefab GameObject (must contain a SkeletonAnimation).</param>
		/// <param name="modNamespace">Stable namespace for the registering mod (plugin GUID recommended).</param>
		public static void RegisterPrefab(GameObject prefab, string modNamespace = ModPrefabRegistry.DefaultNamespace) {
			ModPrefabRegistry.Register(prefab, modNamespace);
		}

		/// <summary>
		/// Identifies a vanilla sex prefab as a (family, list, index) triple by
		/// scanning sexMN.sexObj (family 0) and sexMN.sexList (family 1).
		/// </summary>
		private static bool TryIdentifyVanilla(SexManager sexMN, GameObject prefab, out int family, out int list, out int index) {
			family = -1;
			list = -1;
			index = -1;

			GameObject[] sexObj = sexMN.sexObj;
			if (sexObj != null) {
				for (int i = 0; i < sexObj.Length; i++) {
					if (sexObj[i] == prefab) {
						family = 0;
						index = i;
						return true;
					}
				}
			}

			SexManager.SexList[] sexList = sexMN.sexList;
			if (sexList != null) {
				for (int l = 0; l < sexList.Length; l++) {
					GameObject[] objs = sexList[l]?.sexObj;
					if (objs == null)
						continue;
					for (int i = 0; i < objs.Length; i++) {
						if (objs[i] == prefab) {
							family = 1;
							list = l;
							index = i;
							return true;
						}
					}
				}
			}

			return false;
		}

		/// <summary>
		/// Instantiates a sex visual prefab, tagging it with a
		/// FreeInteractionPrefabBinding so the co-op layer can identify and
		/// replicate it to remote clients. Falls back to a plain Instantiate when
		/// not in Free Mode or the prefab cannot be identified.
		/// </summary>
		public static GameObject SpawnVisual(GameObject prefab, Vector3 position, Quaternion rotation) {
			if (Coop.IsSupported) {
				GameObject result = SpawnVisualFreeMode(prefab, position, rotation);
				if (result != null)
					return result;
			}
			return UnityEngine.Object.Instantiate(prefab, position, rotation);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static GameObject SpawnVisualFreeMode(GameObject prefab, Vector3 position, Quaternion rotation) {
			SexManager sexMN = Managers.sexMN;
			if (prefab == null || sexMN == null || !FreeModeScene.IsFreeMode(sexMN))
				return null;

			GameObject spawned = null;

			if (TryIdentifyVanilla(sexMN, prefab, out int family, out int list, out int index))
				spawned = FreeInteractionPrefabBinding.Create(sexMN, family, list, index, prefab, position, rotation);
			else if (ModPrefabRegistry.TryGetId(prefab, out list, out index))
				spawned = FreeInteractionPrefabBinding.Create(sexMN, ModPrefabRegistry.Family, list, index, prefab, position, rotation);

			if (spawned == null)
				return null;

			// Same as vanilla InstantiateFreeList: while a downed-recovery
			// presentation is active, objects spawned for it are tracked so they
			// are destroyed if the presentation is cancelled (no-op otherwise).
			return FreeCoopSession.TrackDownedRecoveryObject(sexMN, spawned);
		}

		/// <summary>Player interacting with an NPC (e.g. CommonSexPlayer, ManRapes).</summary>
		public static void ObservePlayerNpc(GameObject visual, CommonStates player, CommonStates npc, SkeletonAnimation animation) {
			if (!Coop.IsSupported || !IsFreeMode())
				return;
			ObservePlayerNpcInternal(visual, player, npc, animation);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ObservePlayerNpcInternal(GameObject visual, CommonStates player, CommonStates npc, SkeletonAnimation animation) {
			try {
				FreeInteractionSource.Observe(visual, player, npc, animation);
			} catch (Exception e) {
				PLogger.LogError($"FreeModeInterop.ObservePlayerNpc failed: {e.Message}");
			}
		}

		/// <summary>NPC interacting with another NPC (e.g. CommonSexNPC).</summary>
		public static void ObserveNpcPair(GameObject visual, CommonStates first, CommonStates second, SkeletonAnimation animation) {
			if (!Coop.IsSupported || !IsFreeMode())
				return;
			ObserveNpcPairInternal(visual, first, second, animation);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ObserveNpcPairInternal(GameObject visual, CommonStates first, CommonStates second, SkeletonAnimation animation) {
			try {
				FreeInteractionSource.ObserveNpcPair(visual, first, second, animation);
			} catch (Exception e) {
				PLogger.LogError($"FreeModeInterop.ObserveNpcPair failed: {e.Message}");
			}
		}

		/// <summary>NPC performing a solo interaction (e.g. OnaniNPC).</summary>
		public static void ObserveNpcSolo(GameObject visual, CommonStates npc, SkeletonAnimation animation) {
			if (!Coop.IsSupported || !IsFreeMode())
				return;
			ObserveNpcSoloInternal(visual, npc, animation);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ObserveNpcSoloInternal(GameObject visual, CommonStates npc, SkeletonAnimation animation) {
			try {
				FreeInteractionSource.ObserveNpcSolo(visual, npc, animation);
			} catch (Exception e) {
				PLogger.LogError($"FreeModeInterop.ObserveNpcSolo failed: {e.Message}");
			}
		}

		/// <summary>NPC interacting with a downed player (e.g. PlayerRaped).</summary>
		public static void ObserveDowned(GameObject visual, CommonStates player, CommonStates npc, SkeletonAnimation animation) {
			if (!Coop.IsSupported || !IsFreeMode())
				return;
			ObserveDownedInternal(visual, player, npc, animation);
		}

		[MethodImpl(MethodImplOptions.NoInlining)]
		private static void ObserveDownedInternal(GameObject visual, CommonStates player, CommonStates npc, SkeletonAnimation animation) {
			try {
				FreeInteractionSource.ObserveDowned(visual, player, npc, animation);
			} catch (Exception e) {
				PLogger.LogError($"FreeModeInterop.ObserveDowned failed: {e.Message}");
			}
		}

		/// <summary>
		/// Picks the appropriate Observe variant from a set of scene actors:
		/// a pair containing the local player uses Observe, two NPCs use
		/// ObserveNpcPair, a lone NPC uses ObserveNpcSolo.
		/// </summary>
		public static void ObserveActors(GameObject visual, CommonStates[] actors, SkeletonAnimation animation) {
			if (!Coop.IsSupported || !IsFreeMode() || actors == null || actors.Length == 0)
				return;

			// The co-op layer requires the "player" to be the actual local player
			// (FreeCoopSession.LocalPlayer), not a character that merely has a
			// player-like npcID
			CommonStates activePlayer;
			try {
				activePlayer = CommonUtils.GetActivePlayer();
			} catch {
				activePlayer = null;
			}

			CommonStates player = null;
			CommonStates npc = null;
			CommonStates npc2 = null;
			foreach (CommonStates actor in actors) {
				if (actor == null)
					continue;
				if (actor == activePlayer)
					player = actor;
				else if (npc == null)
					npc = actor;
				else
					npc2 = actor;
			}

			if (player != null && npc != null)
				ObservePlayerNpcInternal(visual, player, npc, animation);
			else if (npc != null && npc2 != null)
				ObserveNpcPairInternal(visual, npc, npc2, animation);
			else if (npc != null)
				ObserveNpcSoloInternal(visual, npc, animation);
		}
	}
}
