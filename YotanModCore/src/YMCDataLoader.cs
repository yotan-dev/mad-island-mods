using UnityEngine;
using YotanModCore.Items;
using System.Collections.Generic;

namespace YotanModCore
{
	/// <summary>
	/// ScriptableObject that bridges custom prefabs/objects to be loaded into the game by YotanModCore.<br />
	/// During start up, asset bundles will be scanned for this object and its contents will be loaded.
	/// </summary>
	[Experimental]
	[CreateAssetMenu(fileName = "YMCDataLoader", menuName = "Yotan Mod Core/Data Loader", order = 1)]
	public class YMCDataLoader : ScriptableObject
	{
		[Tooltip("Add your custom items 'inventory data' here so they get loaded into the game")]
		public List<CustomItemData> Items = [];

		[Tooltip("Add your custom craft recipes here so they get loaded into the game")]
		public List<CraftRecipe> CraftRecipes = [];
	}
}
