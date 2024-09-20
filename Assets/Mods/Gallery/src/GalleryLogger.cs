#nullable enable

using System;
using System.Collections.Generic;
using System.Diagnostics;
using BepInEx.Logging;
using YotanModCore;

namespace Gallery
{
	public class GalleryLogger
	{
		private static ManualLogSource? LogSource;

		private class GalleryLogListener : ILogListener
		{
			private DiskLogListener DiskLogger = new DiskLogListener("GalleryLogger.txt", LogLevel.All, true, false);

			public void Dispose()
			{
				DiskLogger.Dispose();
			}

			public void LogEvent(object sender, LogEventArgs eventArgs)
			{
				if (eventArgs.Source.SourceName == "GalleryLogger")
					this.DiskLogger.LogEvent(sender, eventArgs);
			}
		}

		public static void Init()
		{
			if (!Config.Instance.WriteLogs.Value) {
				LogSource = null;
				return;
			}

			LogSource = Logger.CreateLogSource("GalleryLogger");
			var logListener = new GalleryLogListener();
			Logger.Listeners.Add(logListener);

			LogSource?.LogDebug("=============== Session started ===============");
		}

		private static void CheckNewNpc(CommonStates npc)
		{
			if (npc == null)
				return;

			if (CommonUtils.GetName(npc).StartsWith("Unknown"))
				LogSource?.LogWarning($"***** New NPC Found: {npc.charaName} / {npc.name} / {npc.npcID} *****");
		}

		private static void LogScene(string typeText, string name, CommonStates charaA, CommonStates charaB, string detail)
		{
			CheckNewNpc(charaA);
			CheckNewNpc(charaB);

			var charaAName = CommonUtils.DetailedString(charaA);
			var charaBName = CommonUtils.DetailedString(charaB);

			LogSource?.LogInfo($"[{DateTime.Now}] {{ type: \"{typeText}\", scene: \"{name}\", detail: \"{detail}\", charaA: \"{charaAName}\", charaB: \"{charaBName}\" }}");
		}

		public static void SceneStart(string name, CommonStates charaA, CommonStates charaB, string detail)
		{
			LogScene("start", name, charaA, charaB, detail);
		}

		public static void SceneEnd(string name, CommonStates charaA, CommonStates charaB, string detail)
		{
			LogScene("end", name, charaA, charaB, detail);
		}

		private static void LogScene(string state, string name, Dictionary<string, CommonStates> charas, Dictionary<string, string> infos, bool unhandled)
		{
			foreach (var chara in charas.Values) {
				CheckNewNpc(chara);
			}

			if (unhandled) {
				LogSource?.LogWarning($"[{DateTime.Now}] Unhandled scene found!! [{state} - {name}]");
			}

			LogSource?.LogMessage($"[{DateTime.Now}] [{state} - {name}]");
			foreach (var info in infos) {
				LogSource?.LogInfo($"\t- {info.Key}: {info.Value}");
			}
			foreach (var chara in charas) {
				LogSource?.LogInfo($"\t- {chara.Key}: {CommonUtils.DetailedString(chara.Value)}");
			}
		}

		public static void SceneStart(string name, Dictionary<string, CommonStates> charas, Dictionary<string, string> infos, bool unhandled = false)
		{
			LogScene("start", name, charas, infos, unhandled);
		}

		public static void SceneEnd(string name, Dictionary<string, CommonStates> charas, Dictionary<string, string> infos, bool unhandled = false)
		{
			LogScene("end", name, charas, infos, unhandled);
		}

		public static void SexCountChanged(CommonStates from, CommonStates to, SexManager.SexCountState sexState, string detail)
		{
			CheckNewNpc(from);
			CheckNewNpc(to);

			var fromName = CommonUtils.DetailedString(from);
			var toName = CommonUtils.DetailedString(to);

			LogSource?.LogMessage($"[{DateTime.Now}] [SexCountChanged]");
			LogSource?.LogInfo($"\t- sexState: {sexState}");
			LogSource?.LogInfo($"\t- from: {fromName}");
			LogSource?.LogInfo($"\t- to: {toName}");
			LogTrace("SexCount trace");
		}

		public static void LogError(string message)
		{
			LogSource?.LogError(message);
			LogSource?.LogError(new StackTrace(true).ToString());
		}

		public static void LogTrace(string message)
		{
			LogSource?.LogInfo(message);
			LogSource?.LogInfo(new StackTrace(true).ToString());
		}
	
		public static void LogDebug(string message)
		{
			LogSource?.LogInfo($"[Debug] {message}");
		}

		public static void SceneErrorToPlayer(string scene, Exception error) {
			Managers.mn.StartCoroutine(Managers.mn.eventMN.GoCautionSt("[MOD] Gallery Plugin ERRROR!! Please report."));
			GalleryLogger.LogError($"Failed to process {scene} hook -- this may generate inconsistent scening/gallery.");
			GalleryLogger.LogError(error.Message);
			GalleryLogger.LogError(error.StackTrace);
		}
	}
}
