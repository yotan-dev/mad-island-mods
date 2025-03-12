using BepInEx;
using BepInEx.Bootstrap;

namespace EnhancedIsland
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	[BepInDependency("YotanModCore", BepInDependency.DependencyFlags.HardDependency)]
	// Soft dependency on old mods to soften the migration process - Rmove later
	[BepInDependency("CraftColors", BepInDependency.DependencyFlags.SoftDependency)]
	public class Plugin : BaseUnityPlugin
	{
		/// <summary>
		/// Temporary safety check for people who forget to delete the old plugins
		/// </summary>
		/// <param name="pluginName"></param>
		/// <returns></returns>
		private bool IsDuplicated(string pluginName)
		{
			if (!Chainloader.PluginInfos.ContainsKey(pluginName))
				return false;

			PLogger.LogError($"\"{pluginName}\" plugin detected. This plugin was replaced by \"{MyPluginInfo.PLUGIN_NAME}\". Please remove it!");
			return true;
		}

		private void Awake()
		{
			PLogger._Logger = base.Logger;
			PConfig.Instance.Init(Config);

			if (!IsDuplicated("CraftColors"))
				new RequirementChecker.RequirementChecker().Init();

			PLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
		}
	}
}
