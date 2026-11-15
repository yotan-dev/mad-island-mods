using System;
using System.Runtime.CompilerServices;
using MadIsland.FreeMode;
using UnityEngine;

namespace YotanModCore
{
	/// <summary>
	/// Compatibility helpers for the game's co-op "FreeMode" (v0.6.0+).
	///
	/// IMPORTANT: the MadIsland.FreeMode types only exist in game v0.6.0+.
	/// Any method that references them is marked NoInlining and must only be
	/// called after checking <see cref="IsSupported"/> — a direct call inside
	/// code that runs on older versions will crash with TypeLoadException when
	/// the containing method is JIT-compiled.
	/// </summary>
	public static class Coop
	{
		private static readonly Version FreeModeVersion = new Version(0, 6, 0);

		/// <summary>
		/// True when the game version supports co-op FreeMode (v0.6.0+).
		/// Safe to call on any version — does not reference FreeMode types.
		/// </summary>
		public static bool IsSupported => GameInfo.GameVersion >= FreeModeVersion;

		/// <summary>
		/// True when a co-op session is active or a lobby join is pending.
		/// Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool SessionActive()
		{
			return FreeCoopSession.Active != null || FreeCoopSession.PendingLobby != null;
		}

		/// <summary>
		/// True when the given component lives in a networked co-op scene.
		/// Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool InNetworkScene(Component owner)
		{
			return FreeCoopSession.NetworkScene(owner);
		}

		/// <summary>
		/// True when the local peer is a co-op client (not the host).
		/// False in single-player and on the host.
		/// Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool IsClient()
		{
			return FreeCoopSession.Client;
		}

		/// <summary>
		/// True when the local peer may mutate shared world state:
		/// single-player or co-op host. Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool CanMutateWorld()
		{
			return !FreeCoopSession.Client;
		}

		/// <summary>
		/// Routes a BlocksInput check through the game's save/exit/network prompt state.
		/// Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool BlocksInput(Component owner)
		{
			return FreeSaveSession.BlocksInput(owner);
		}

		/// <summary>
		/// Routes a damage request through the co-op session.
		/// Returns true when the game handled/routed the hit and the caller
		/// should skip applying local damage. Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool Damage(CommonStates target, CommonStates attacker, float rate)
		{
			return FreeCoopSession.Damage(target, attacker, rate);
		}

		/// <summary>
		/// Routes an area-effect damage instance through the co-op session.
		/// Returns true when the game handled/routed the hit (including co-op
		/// clients, where the host applies it authoritatively) and the caller
		/// should skip applying local damage. Only call when <see cref="IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool RouteAreaEffect(DamageTrigger source, CommonStates target)
		{
			return FreeCoopSession.RouteAreaEffect(source, target);
		}

		// /// <summary>
		// /// Instantiates a free-mode primary interaction object (e.g., sex recovery) and tracks it for co-op.
		// /// Only call when <see cref="IsSupported"/> is true.
		// /// </summary>
		// [MethodImpl(MethodImplOptions.NoInlining)]
		// public static GameObject InstantiateFreePrimary(SexManager sexManager, int index, Vector3 position, Quaternion rotation)
		// {
		// 	return FreeCoopSession.TrackDownedRecoveryObject(sexManager, FreeInteractionPrefabBinding.Create(sexManager, 0, -1, index, sexManager.sexObj[index], position, rotation));
		// }

		// /// <summary>
		// /// Instantiates a free-mode list interaction object (e.g., sex recovery list) and tracks it for co-op.
		// /// Only call when <see cref="IsSupported"/> is true.
		// /// </summary>
		// [MethodImpl(MethodImplOptions.NoInlining)]
		// public static GameObject InstantiateFreeList(
		// 	SexManager sexManager,
		// 	int list,
		// 	int index,
		// 	Vector3 position,
		// 	Quaternion rotation)
		// {
		// 	return FreeCoopSession.TrackDownedRecoveryObject(sexManager, FreeInteractionPrefabBinding.Create(sexManager, 1, list, index, sexManager.sexList[list].sexObj[index], position, rotation));
		// }
	}
}
