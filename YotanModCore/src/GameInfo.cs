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
		/// It follows the format of Major.Minor.Build.Patch, using C#'s Version class
		/// 
		/// If version could not be determined, it returns 999_999_999_999
		/// 
		/// To compare/display, prefer using ToVersion and ToVersionString.
		/// </summary>
		public static Version GameVersion { get { return Instance._GameVersion; } }

		/// <summary>
		/// True if DLC is installed
		/// </summary>
		public static bool HasDLC { get { return Instance._HasDLC; } }

		/// <summary>
		/// True if Joke mode is enabled
		/// </summary>
		public static bool JokeMode { get { return SaveManager.SaveSettingStatic.joke == 1; } }

		/// <summary>
		/// Returns the level of censorship being applied
		/// </summary>
		public static CensorType CensorType { get { return Instance._CensorType; } }

		/// <summary>
		/// Returns the size of a censorship block
		/// </summary>
		public static float CensorBlockSize { get { return Instance._CensorBlockSize; } }

		/// <summary>
		/// Converts major/minor/build/patch version to GameVersion
		/// </summary>
		/// <param name="major"></param>
		/// <param name="minor"></param>
		/// <param name="build"></param>
		/// <param name="patch"></param>
		/// <returns></returns>
		public static Version ToVersion(int major, int minor, int build, int patch = 0)
		{
			return new Version(major, minor, build, patch);
		}

		/// <summary>
		/// Parses a version string into GameVersion. A version string is composed of "<major>.<minor>.<build>.<patch>"
		/// where each of the 4 values are integers.
		/// <build> and <patch> may be ommited, but major and minor are required.
		/// 
		/// Examples:
		/// - "0.1" -> 0.1.0.0
		/// - "0.1.2" -> 0.1.2.0
		/// - "0.1.2.3" -> 0.1.2.3
		/// </summary>
		/// <param name="str"></param>
		/// <returns></returns>
		/// <exception cref="Exception"></exception>
		public static Version ToVersion(string str)
		{
			return new Version(str);
		}

		/// <summary>
		/// Converts a "GameVersion" number into a version string.
		/// See <see cref="GameVersion"/> for structure details
		/// </summary>
		/// <param name="version"></param>
		/// <returns></returns>
		public static string ToVersionString(Version version)
		{
			return version.ToString();
		}

		private static GameInfo Instance = new GameInfo();

		private bool _HasDLC = false;

		private Version _GameVersion = new Version(0, 0, 0, 0);

		private CensorType _CensorType = CensorType.Default;

		private float _CensorBlockSize = 7f;

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
			this._CensorType = this.DetectCensorType();
			if (this._CensorType == CensorType.Fanza)
				this._CensorBlockSize = 17f;
			if (this._CensorType == CensorType.None)
				this._CensorBlockSize = 0.01f;
		}

		private CensorType DetectCensorType()
		{
			if (File.Exists(Application.dataPath + "/StreamingAssets/XML/FNZ.bat"))
				return CensorType.Fanza;
			
			if (File.Exists(Application.dataPath + "/StreamingAssets/XML/None.bat"))
				return CensorType.None;
			
			return CensorType.Default;
		}

		private Version ParseVersion()
		{
			var buildTimestamp = Resources.Load<TextAsset>("BuildDate").text[1..];

			Dictionary<string, Version> buildDateVersion = new Dictionary<string, Version>() {
				{ "2024/06/28--10:56", ToVersion("0.0.12") },
				{ "2024/07/27--15:32", ToVersion("0.1.3") },
				{ "2024/08/09--18:40", ToVersion("0.1.5") },
				{ "2024/08/18--18:14", ToVersion("0.1.6") },
				{ "2024/09/08--20:46", ToVersion("0.1.8") },
				{ "2024/10/18--12:06", ToVersion("0.2.0") }, // Beta
				{ "2024/10/20--21:56", ToVersion("0.2.1") }, // Beta
				{ "2024/11/15--21:26", ToVersion("0.2.2") }, // Beta
				{ "2024/11/22--13:58", ToVersion("0.2.3") }, // Beta
				{ "2024/12/15--10:19", ToVersion("0.2.4") }, // Beta
				{ "2024/12/18--11:33", ToVersion("0.2.5") }, // Beta
				{ "2024/12/19--15:14", ToVersion("0.2.6") }, // Beta
				{ "2024/12/21--15:10", ToVersion("0.2.7") },
				{ "2025/02/28--16:12", ToVersion("0.3.0") }, // Beta
				{ "2025/02/28--17:32", ToVersion("0.3.1") }, // Beta
				{ "2025/03/07--00:04", ToVersion("0.3.2") }, // Beta
				{ "2025/03/07--20:41", ToVersion("0.3.3") }, // Beta
				{ "2025/03/08--16:35", ToVersion("0.3.4") }, // Beta / Main
				{ "2025/04/19--19:24", ToVersion("0.4.0") }, // Beta
				{ "2025/04/19--20:21", ToVersion("0.4.1") }, // Beta
				{ "2025/06/06--14:50", ToVersion("0.4.2") }, // Beta
				{ "2025/06/06--16:37", ToVersion("0.4.3") }, // Beta
				{ "2025/06/08--09:10", ToVersion("0.4.3.2") }, // Beta
			};

			if (buildDateVersion.ContainsKey(buildTimestamp))
				return buildDateVersion[buildTimestamp];

			PLogger.LogWarning($"Unknown build date {buildTimestamp}. Using heuristics.");
			Dictionary<string, Version> dateToVersion = new Dictionary<string, Version>() {
				{ "2024/06/10", ToVersion("0.0.8") }, // "--16:36" not 100% sure ("oldver" beta on August 4th, 2024)
				{ "2024/07/19", ToVersion("0.1.0") },
				{ "2024/07/20", ToVersion("0.1.1") },
				{ "2024/07/26", ToVersion("0.1.2") },
				{ "2024/08/09", ToVersion("0.1.4") },
				{ "2024/09/08", ToVersion("0.1.7") }, // 0.1.7 was released a bit before 0.1.8, same day
				{ "2025/06/08", ToVersion("0.4.3.1") }, // Beta; 0.4.3.1 was released a bit before 0.4.3.2, same day
			};

			var buildDate = buildTimestamp[..10];
			if (dateToVersion.ContainsKey(buildDate))
				return dateToVersion[buildDate];

			PLogger.LogError($"Could not determine version by build date. Using 999.999.999.999");
			return ToVersion("999.999.999.999");
		}
	}
}
