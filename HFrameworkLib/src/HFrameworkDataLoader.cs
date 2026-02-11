using UnityEngine;
using System.Collections.Generic;
using HFramework.Tree;
using HFramework.SexScripts;

namespace HFramework
{
	/// <summary>
	/// ScriptableObject that bridges custom prefabs/objects to be loaded into the game by HFramework.<br />
	/// During start up, asset bundles will be scanned for this object and its contents will be loaded.
	/// </summary>
	// [Experimental]
	[CreateAssetMenu(fileName = "HFrameworkDataLoader", menuName = "HFramework/Data Loader", order = 1)]
	public class HFrameworkDataLoader : ScriptableObject
	{
		[Tooltip("Add your custom prefabs here so they get loaded into the game")]
		public List<SexScript> Prefabs = [];
	}
}
