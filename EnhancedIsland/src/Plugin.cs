using System.IO;
using BepInEx;
using BepInEx.Bootstrap;

namespace EnhancedIsland
{
	[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
	[BepInDependency("YotanModCore", "2.0.0")]
	// Soft dependency on old mods to soften the migration process - Rmove later
	[BepInDependency("EnhanceWorkplaces", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("DisassembleItems", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("IncreaseZoom", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("ItemSlotColor", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("NpcStats", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("CraftColors", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("StackNearby", BepInDependency.DependencyFlags.SoftDependency)]
	[BepInDependency("WarpBody", BepInDependency.DependencyFlags.SoftDependency)]
	public class Plugin : BaseUnityPlugin
	{
		internal static string PluginPath = "";

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
			PluginPath = Path.GetDirectoryName(this.Info.Location);
			PLogger._Logger = base.Logger;
			PConfig.Instance.Init(Config);

			if (!IsDuplicated("EnhanceWorkplaces"))
				new BetterWorkplaces.Main().Init();

			if (!IsDuplicated("DisassembleItems"))
				new DisassembleItems.Main().Init();

			if (!IsDuplicated("IncreaseZoom"))
				new IncreaseZoom.Main().Init();

			if (!IsDuplicated("ItemSlotColor"))
				new ItemColorInSlot.Main().Init();

			if (!IsDuplicated("NpcStats"))
				new NpcStats.Main().Init();

			if (!IsDuplicated("CraftColors"))
				new RequirementChecker.Main().Init();

			new RotateObject.Main().Init();

			if (!IsDuplicated("StackNearby"))
				new StackNearby.Main().Init();

			if (!IsDuplicated("WarpBody"))
				new WarpBody.Main().Init();

			PLogger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
		}
	}
}
