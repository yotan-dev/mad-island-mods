using System;
using System.Collections.Generic;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using Spine;
using Spine.Unity;
using UnityEngine;

namespace HFramework.FreeMode
{
	/// <summary>
	/// Registry of mod-provided prefabs that can be replicated by the Free Mode
	/// co-op layer (MadIsland.FreeMode). The co-op protocol identifies interaction
	/// visuals by (family, list, index, signature). Vanilla only knows families 0-3;
	/// HFramework claims family 4 where "list" is a stable key derived from the
	/// registering mod's namespace and "index" is the ordinal position of the
	/// prefab within that namespace's alphabetically sorted registry.
	/// </summary>
	internal static class ModPrefabRegistry
	{
		public const int Family = 4;

		public const string DefaultNamespace = "HFramework";

		private class Entry
		{
			public string Key;
			public int ListKey;
			public GameObject Prefab;
		}

		/// <summary>List key -> entries keyed and sorted by their unique key.</summary>
		private static readonly Dictionary<int, SortedList<string, Entry>> Lists =
			new Dictionary<int, SortedList<string, Entry>>();

		private static readonly Dictionary<GameObject, Entry> ByPrefab = new Dictionary<GameObject, Entry>();

		/// <summary>
		/// Deterministic 16-bit key for a mod namespace (e.g. its plugin GUID).
		/// </summary>
		public static int ListKey(string modNamespace)
		{
			using (SHA256 sha = SHA256.Create())
			{
				byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes("hframework-freemode-ns|" + (modNamespace ?? "")));
				return (hash[0] << 8) | hash[1];
			}
		}

		private static string EntryKey(string modNamespace, GameObject prefab)
		{
			return modNamespace + "|" + prefab.name;
		}

		/// <summary>
		/// Registers a prefab under a mod namespace. Index assignment is derived from
		/// the alphabetical order of keys, so the relative order of Register calls
		/// does not matter - only the complete set of registered keys must match
		/// across clients.
		/// </summary>
		public static void Register(GameObject prefab, string modNamespace = DefaultNamespace)
		{
			if (prefab == null || ByPrefab.ContainsKey(prefab))
				return;

			modNamespace = modNamespace ?? DefaultNamespace;
			int listKey = ListKey(modNamespace);
			string key = EntryKey(modNamespace, prefab);

			if (!Lists.TryGetValue(listKey, out SortedList<string, Entry> entries))
			{
				entries = new SortedList<string, Entry>(StringComparer.Ordinal);
				Lists[listKey] = entries;
			}

			if (entries.ContainsKey(key))
			{
				PLogger.LogWarning($"ModPrefabRegistry: duplicate key \"{key}\" for a different prefab; skipping.");
				return;
			}

			Entry entry = new Entry { Key = key, ListKey = listKey, Prefab = prefab };
			entries.Add(key, entry);
			ByPrefab[prefab] = entry;
		}

		/// <summary>
		/// Returns the (list, index) tuple identifying the prefab on the wire.
		/// Unknown prefabs are auto-registered under the default namespace so any
		/// custom scene prefab is replicable without explicit registration.
		/// </summary>
		public static bool TryGetId(GameObject prefab, out int list, out int index)
		{
			list = -1;
			index = -1;
			if (prefab == null)
				return false;

			if (!ByPrefab.ContainsKey(prefab))
				Register(prefab, DefaultNamespace);

			if (!ByPrefab.TryGetValue(prefab, out Entry entry))
				return false;

			list = entry.ListKey;
			index = Lists[list].IndexOfKey(entry.Key);
			return index >= 0;
		}

		/// <summary>
		/// Inverse of TryGetId, used by the FreeInteractionPrefabBinding.TryResolve patch.
		/// </summary>
		public static bool TryResolve(int list, int index, out GameObject prefab, out string signature)
		{
			prefab = null;
			signature = null;
			if (!Lists.TryGetValue(list, out SortedList<string, Entry> entries) || index < 0 || index >= entries.Count)
				return false;

			prefab = entries.Values[index].Prefab;
			if (prefab == null)
				return false;

			return TryComputeSignature(prefab, out signature);
		}

		/// <summary>
		/// Replicates the signature scheme used by FreeInteractionPrefabBinding.TryResolve
		/// so that mod prefabs produce the same signature on every client: a SHA-256 over
		/// the prefab name, skeleton data asset name, and skeleton data hash (or the
		/// sorted animation name/duration list when no hash is available).
		/// </summary>
		private static bool TryComputeSignature(GameObject prefab, out string signature)
		{
			signature = null;
			SkeletonAnimation animation = (prefab.transform.Find("Scale/Anim") ?? prefab.transform.Find("scale/Anim"))?.GetComponent<SkeletonAnimation>();
			if (animation == null || animation.skeletonDataAsset == null)
				return false;

			try
			{
				SkeletonDataAsset dataAsset = animation.skeletonDataAsset;
				SkeletonData data = dataAsset.GetSkeletonData(true);
				if (data == null)
					return false;

				StringBuilder sb = new StringBuilder("free-interaction-prefab-v1|");
				AppendToken(sb, prefab.name);
				AppendToken(sb, dataAsset.name);
				AppendToken(sb, data.Hash);
				if (string.IsNullOrEmpty(data.Hash))
				{
					List<Spine.Animation> animations = new List<Spine.Animation>();
					foreach (Spine.Animation anim in data.Animations)
					{
						if (anim == null || float.IsNaN(anim.Duration) || float.IsInfinity(anim.Duration))
							return false;
						animations.Add(anim);
					}
					animations.Sort((a, b) => StringComparer.Ordinal.Compare(a.Name, b.Name));
					foreach (Spine.Animation anim in animations)
					{
						AppendToken(sb, anim.Name);
						AppendToken(sb, anim.Duration.ToString("R", CultureInfo.InvariantCulture));
					}
				}

				using (SHA256 sha = SHA256.Create())
				{
					byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString()));
					signature = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}

		private static void AppendToken(StringBuilder value, string token)
		{
			token = token ?? "";
			value.Append(token.Length.ToString(CultureInfo.InvariantCulture)).Append(':').Append(token).Append('|');
		}
	}
}
