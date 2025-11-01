using BepInEx;
using HarmonyLib;
using YotanModCore.DataStore;

namespace Samples
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = base.Logger;
			PLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

			DataStoreManager.RegisterDataStore(typeof(KillCountStore), () => new KillCountStore(), typeof(KillCountStore.Data));
			DataStoreManager.RegisterDataStore(typeof(DamageTakenStore), () => new DamageTakenStore(), typeof(DamageTakenStore.Data));

			Harmony.CreateAndPatchAll(typeof(DamageTakenPatch));
			Harmony.CreateAndPatchAll(typeof(KillCountPatch));
		}
	}
}
