using BepInEx;

namespace Base
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	public class Plugin : BaseUnityPlugin
	{
		private void Awake()
		{
			PLogger._Logger = base.Logger;
			PLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
		}
	}
}
