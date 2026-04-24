using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HFramework.Performer;
using HFramework.Scenes;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using YotanModCore;

namespace HFramework.SexScripts
{
	public class SexScriptsManager
	{
		public static SexScriptsManager Instance { get; } = new SexScriptsManager();

		private readonly HashSet<string> LoadedScripts = new();

		private readonly Dictionary<string, List<SexScript>> ScriptsByType = new();

		/// <summary>
		/// Validates a sex script for basic structure and data integrity.
		/// </summary>
		/// <param name="script">The sex script to validate.</param>
		/// <param name="error">Error message if validation fails, null otherwise.</param>
		/// <returns>True if the script is valid, false otherwise.</returns>
		private static bool IsScriptValid(SexScripts.SexScript script, out string error) {
			error = null;
			try {
				if (script.Info == null)
					throw new Exception("Info is null");
				if (script.Info.Npcs == null || script.Info.Npcs.Length == 0)
					throw new Exception("No NPCs");

				foreach (var npc in script.Info.Npcs) {
					if (npc.Conditions == null)
						throw new Exception($"NPC {npc.NpcID} has NULL conditions");
				}
			} catch (Exception e) {
				error = e.Message;
			}

			return error == null;
		}

		/// <summary>
		/// Checks if a sex script can be loaded based on version and DLC requirements.
		/// </summary>
		/// <param name="script">The sex script to check.</param>
		/// <returns>True if the script can be loaded, false otherwise.</returns>
		private bool CanScriptBeLoaded(SexScript script) {
			if (!string.IsNullOrEmpty(script.MinimalGameVersion)) {
				if (GameInfo.GameVersion < GameInfo.ToVersion(script.MinimalGameVersion)) {
					PLogger.LogWarning($"Skipping {script.name}: Minimal game version {script.MinimalGameVersion} is higher than current game version {GameInfo.GameVersion}");
					return false;
				}
			}

			if (script.RequiresDLC && !GameInfo.HasDLC) {
				PLogger.LogWarning($"Skipping {script.name}: Requires DLC but DLC is not available");
				return false;
			}

			if (!IsScriptValid(script, out string error)) {
				PLogger.LogError($"Skipping {script.name}: {error}");
				return false;
			}

			return true;
		}

		/// <summary>
		/// Adds a sex script to the manager if it passes all validation checks.
		/// </summary>
		/// <param name="script">The sex script to add.</param>
		/// <returns>True if the script was added successfully, false otherwise.</returns>
		public bool AddScript(SexScript script) {
			if (LoadedScripts.Contains(script.UniqueID)) {
				PLogger.LogError($"Sex script \"{script.UniqueID}\" already loaded. You have 2 scripts with same UniqueID! Skipping the second one.");
				return false;
			}

			if (!this.CanScriptBeLoaded(script)) {
				return false;
			}

			LoadedScripts.Add(script.UniqueID);

			var type = script.GetType().GetCustomAttributes(typeof(SexScriptTypeAttribute), false);
			if (type.Length == 0) {
				PLogger.LogError($"Sex script \"{script.UniqueID}\" has no SexScriptTypeAttribute. Skipping.");
				return false;
			}

			var typeAttribute = (SexScriptTypeAttribute)type[0];
			if (!ScriptsByType.ContainsKey(typeAttribute.TypeName)) {
				ScriptsByType[typeAttribute.TypeName] = new List<SexScript>();
			}

			ScriptsByType[typeAttribute.TypeName].Add(script);

			return true;
		}

		/// <summary>
		/// Checks if a sex script of the specified type can be started with the given actors.
		/// </summary>
		/// <param name="typeName">The type name of the sex script.</param>
		/// <param name="actors">The actors involved in the sex script.</param>
		/// <returns>True if a sex script can be started, false otherwise.</returns>
		public bool CanStart(string typeName, params CommonStates[] actors) {
			do { // Little hack to make code cleaner -- can be removed once we drop legacy support
				if (!HFConfig.Instance.IsModernModeEnabled) {
					break;
				}

				if (!ScriptsByType.ContainsKey(typeName)) {
					// @TODO: Enable once we have fully converted to modern mode
					// PLogger.LogWarning($"No sex scripts found for type \"{typeName}\".");
					break;
				}

				var canStart = ScriptsByType[typeName].Any(script => script.Info.CanStart(actors));
				if (canStart) {
					return true;
				}
			} while (false);

			if (HFConfig.Instance.IsLegacyModeEnabled) {
				switch (typeName) {
					case SexScriptTypes.CommonSexPlayer:
						return SexChecker.CanFriendSex(CommonSexPlayer.Name, actors[0], actors[1]);

					case SexScriptTypes.CommonSexNPC:
						return SexChecker.CanFriendSex(CommonSexNPC.Name, actors[0], actors[1]);

					case SexScriptTypes.ManRapes:
						return SexChecker.CanRape(ManRapes.Name, actors[0], actors[1]);

					case SexScriptTypes.ManRapesSleep:
						return SexChecker.CanRape(ManRapesSleep.Name, actors[0], actors[1]);

					case SexScriptTypes.PlayerRaped:
						return SexChecker.CanRape(PlayerRaped.Name, actors[0], actors[1]);
				}
			}

			return false;
		}

		public Func<IEnumerator> GetScript(string typeName, CommonStates[] actors, SexInfo info) {
			List<Func<IEnumerator>> candidates = new();
			do { // Little hack to make code cleaner -- can be removed once we drop legacy support
				if (!HFConfig.Instance.IsModernModeEnabled) {
					break;
				}

				if (!ScriptsByType.ContainsKey(typeName)) {
					// @TODO: Enable once we have fully converted to modern mode
					// PLogger.LogWarning($"No sex scripts found for type \"{typeName}\".");
					break;
				}

				ScriptsByType[typeName]
					.FindAll(script => script.Info.CanStart(actors) && script.Info.CanExecute(info))
					.ForEach(script => candidates.Add(() => new TreeWrapper().Run(script.Create(actors, info))));
			} while (false);

			if (HFConfig.Instance.IsLegacyModeEnabled) {
				switch (typeName) {
					// case SexScriptTypes.CommonSexPlayer:
					// 	return SexChecker.CanFriendSex(CommonSexPlayer.Name, actors[0], actors[1]);

					case SexScriptTypes.CommonSexNPC:
						if (info is IHasScriptPlace scriptPlace && scriptPlace.Place is SexPlaceScriptPlace sexPlace) {
							var legacyScene = new CommonSexNPC(actors[0], actors[1], sexPlace.Place);
							if (ScenesManager.Instance.HasPerformer(legacyScene, PerformerScope.Sex, actors)) {
								candidates.Add(() => legacyScene.Run());
							}
						}
						break;

					// case SexScriptTypes.ManRapes:
					// 	return SexChecker.CanRape(ManRapes.Name, actors[0], actors[1]);

					// case SexScriptTypes.ManRapesSleep:
					// 	return SexChecker.CanRape(ManRapesSleep.Name, actors[0], actors[1]);

					// case SexScriptTypes.PlayerRaped:
					// 	return SexChecker.CanRape(PlayerRaped.Name, actors[0], actors[1]);
				}
			}

			if (candidates.Count == 0) {
				return null;
			}

			return candidates[UnityEngine.Random.Range(0, candidates.Count)];
		}
	}
}
