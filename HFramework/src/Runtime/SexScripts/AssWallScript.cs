using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "AssWallScript", menuName = "HFramework/AssWallScript")]
	[SexScriptType(SexScriptTypes.AssWall)]
	[Experimental]
	public class AssWallScript : SexScript
	{
		public AssWallScript Create(CommonStates common, CommonStates girl, InventorySlot tmpWall) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(common, girl).Select(npc => new ContextNpc(npc, null)).ToArray();
			var sexPlace = tmpWall.GetComponent<SexPlace>();
			tree.Context.ScriptPlace = new SexPlaceScriptPlace(sexPlace);
			return (AssWallScript)tree;
		}
	}
}
