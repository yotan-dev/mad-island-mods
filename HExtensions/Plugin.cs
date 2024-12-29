using BepInEx;
using UnityEngine;

namespace HExtensions
{
	[BepInPlugin("HExtensions", "HExtensions", "0.1.0")]
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

			PLogger.LogInfo($"Plugin HExtensions is loaded!");
		}
	}
}
