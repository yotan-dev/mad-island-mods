using System.Linq;
using HFramework.ScriptNodes;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	public class CommonSexNPCScript : SexScript
	{
		public CommonSexNPCScript Create(CommonStates npcA, CommonStates npcB, SexPlace sexPlace) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(npcA, npcB).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.Context.SexPlace = sexPlace;
			tree.Context.SexPlacePos = sexPlace.transform.Find("pos")?.position;
			return (CommonSexNPCScript)tree;
		}
	}
}
