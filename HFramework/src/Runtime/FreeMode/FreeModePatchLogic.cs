using System;
using System.Runtime.CompilerServices;
using MadIsland.FreeMode;
using UnityEngine;
using YotanModCore;

namespace HFramework.FreeMode
{
	/// <summary>
	/// Implementation behind the HFrameworkLoader Harmony patches that extend the
	/// Free Mode co-op replication protocol with family 4 ("mod prefab registry").
	///
	/// Two extension points are required:
	///
	///  - FreeInteractionPrefabBinding.TryResolve: lets family 4 triples resolve to
	///    prefabs registered in <see cref="ModPrefabRegistry"/>, on both the local
	///    spawn path and the remote presentation path.
	///  - FreeInteractionVisual.Valid: the wire validator whitelists families 0-3,
	///    so family 4 visuals would be dropped. We remap the family to a vanilla-
	///    valid one and re-run the original validation, preserving all other checks.
	///
	/// Clients without the loader patches simply fail to resolve the visual and
	/// render nothing - there is no crash or desync.
	///
	/// IMPORTANT: FreeInteractionVisual only exists in game v0.6.0+. Methods that
	/// reference it are marked NoInlining and must only run when
	/// <see cref="Coop.IsSupported"/> is true.
	/// </summary>
	internal static class FreeModePatchLogic
	{
		/// <summary>
		/// Postfix implementation for FreeInteractionPrefabBinding.TryResolve.
		/// Safe to call on any version - only references registry code.
		/// </summary>
		public static bool TryResolveModPrefab(
			SexManager manager,
			int family,
			int list,
			int index,
			out GameObject prefab,
			out string signature)
		{
			prefab = null;
			signature = null;
			if (family != ModPrefabRegistry.Family)
				return false;

			return ModPrefabRegistry.TryResolve(list, index, out prefab, out signature);
		}

		/// <summary>
		/// Postfix implementation for FreeInteractionVisual.Valid.
		/// Only call when <see cref="Coop.IsSupported"/> is true.
		/// </summary>
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static bool ValidateVisual(object visual)
		{
			FreeInteractionVisual value = (FreeInteractionVisual)visual;
			if (value == null || value.family != ModPrefabRegistry.Family)
				return false;

			// Re-run vanilla validation with a whitelisted family to reuse all other
			// checks (signature format, transforms, tracks, slot appearance data).
			int family = value.family;
			int list = value.list;
			value.family = 0;
			value.list = -1;
			bool result = FreeInteractionVisual.Valid(value);
			value.family = family;
			value.list = list;
			return result;
		}
	}
}
