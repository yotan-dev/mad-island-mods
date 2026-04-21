using System.Linq;
using HFramework.ScriptNodes;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "AssWallScript", menuName = "HFramework/AssWallScript")]
	[Experimental]
	public class AssWallScript : SexScript
	{
		public AssWallScript Create(CommonStates common, CommonStates girl, InventorySlot tmpWall) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(common, girl).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.Context.SexPlace = tmpWall.GetComponent<SexPlace>();
			tree.Context.SexPlacePos = tree.Context.SexPlace.transform.Find("pos")?.position;
			return (AssWallScript)tree;
		}
	}
}
