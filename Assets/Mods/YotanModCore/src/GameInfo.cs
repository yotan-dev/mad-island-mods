using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace YotanModCore
{
	/// <summary>
	/// Class with general informations about the game.
	/// </summary>
	public class GameInfo
	{
		/// <summary>
		/// GameVersion based on patch notes.
		/// It follows the format of Major.Minor.Patch where each has space up to 999.
		/// 
		/// If version could not be determined, it returns 999_999_999
		/// 
		/// Thus: (GameVersion -> String)
		/// 12 -> 0.0.12
		/// 1_000 -> 0.1.0
		/// 1_001 -> 0.1.1
		/// 1_000_000 -> 1.0.0
		/// 
		/// To compare/display, prefer using ToVersion and ToVersionString.
		/// </summary>
		public static int GameVersion { get { return Instance._GameVersion; } }

		/// <summary>
		/// True if DLC is installed
		/// </summary>
		public static bool HasDLC { get { return Instance._HasDLC; } }

		/// <summary>
		/// Converts major/minor/patch version to GameVersion
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="patch"></param>
		/// <returns></returns>
		public static int ToVersion(int major, int minor, int patch) {
			return major * 1_000_000 + minor * 1_000 + patch;
		}

		/// <summary>
		/// Parses a version string into GameVersion. A version string is composed of "<major>.<minor>.<patch>"
		/// where each of the 3 values are integers.
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static int ToVersion(string str) {
			var parts = str.Split('.');
			if (parts.Length != 3)
				throw new Exception("Invalid version string: " + str);

			return ToVersion(int.Parse(parts[0]), int.Parse(parts[1]), int.Parse(parts[2]));
		}

		/// <summary>
		/// Converts a "GameVersion" number into a version string.
		/// See <see cref="GameVersion"/> for structure details
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public static string ToVersionString(int version) {
			return (version / 1_000_000) + "." + (version / 1_000) + "." + (version % 1_000);
		}
		
		private static GameInfo Instance = new GameInfo();

		private bool _HasDLC = false;

		private int _GameVersion = 0;

		private GameInfo()
		{
			string path = Application.dataPath + "/StreamingAssets/DLC/dlc_00";
			if (!File.Exists(path))
			{
				path = Application.dataPath + "/StreamingAssets/DLC/dlc_00.zip";
			}

			/**
			 * The proper check would be:
			 * SaveManager.dlc00 != null;
			 *
			 * but it won't be there as soon as the app starts (during plugin initialization), so
			 * we don't always have it. Checking directly for the file would give us the existence,
			 * but won't check if it failed to load. But I guess that's fine.
			 */
			this._HasDLC = File.Exists(path);
			this._GameVersion = this.ParseVersion();
		}

		private int ParseVersion()
		{
			var buildTimestamp = Resources.Load<TextAsset>("BuildDate").text.Remove(0, 1);
			
			Dictionary<string, int> buildDateVersion = new Dictionary<string, int>() {
				{ "2024/06/28--10:56", ToVersion("0.0.12") },
				{ "2024/07/27--15:32", ToVersion("0.1.3") },
				{ "2024/08/09--18:40", ToVersion("0.1.5") },
				{ "2024/08/18--18:14", ToVersion("0.1.6") },
				{ "2024/09/08--20:46", ToVersion("0.1.8") },
			};

			if (buildDateVersion.ContainsKey(buildTimestamp))
				return buildDateVersion[buildTimestamp];

			PLogger.LogWarning($"Unknown build date {buildTimestamp}. Using heuristics.");
			Dictionary<string, int> dateToVersion = new Dictionary<string, int>() {
				{ "2024/06/10", ToVersion("0.0.8") }, // "--16:36" not 100% sure ("oldver" beta on August 4th, 2024)
				{ "2024/07/19", ToVersion("0.1.0") },
				{ "2024/07/20", ToVersion("0.1.1") },
				{ "2024/07/26", ToVersion("0.1.2") },
				{ "2024/08/09", ToVersion("0.1.4") },
				{ "2024/09/08", ToVersion("0.1.7") }, // 0.1.7 was released a bit before 0.1.8, same day
			};

			var buildDate = buildTimestamp.Substring(0, 10);
			if (dateToVersion.ContainsKey(buildDate))
				return dateToVersion[buildDate];
			
			PLogger.LogError($"Could not determine version by build date. Using 999.999.999");
			return ToVersion("999.999.999");
		}
	}
}