﻿using BepInEx;
using UnityEngine;

namespace HExtensions
{
	[BepInPlugin("HExtensions", "HExtensions", "0.1.0")]
	[BepInDependency("HFramework", "1.0.0")]
	public class Plugin : BaseUnityPlugin
	{
		public static AssetBundle Assets;

		public static ManagersScript ManagerScript;

		private void Awake()
		{
			PLogger._Logger = Logger;
			HExtensions.Config.Instance.Init(Config);

			if (HExtensions.Config.Instance.RequireForeplay.Enabled.Value)
				new RequireForeplay.Main().Init();

			if (HExtensions.Config.Instance.EnableDickPainter.Value)
				new DickPainter.Main().Init();

			if (HExtensions.Config.Instance.EnableExtendedScenes.Value)
				new ExtendedScenes.Main().Init();

			// new MoreScenes.Main().Init();

			PLogger.LogInfo($"Plugin HExtensions is loaded!");
		}
	}
}
