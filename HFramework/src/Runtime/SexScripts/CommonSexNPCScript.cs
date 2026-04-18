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
			tree.context.Actors = this.Info.BuildNpcs(npcA, npcB).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.context.SexPlace = sexPlace;
			tree.context.SexPlacePos = sexPlace.transform.Find("pos")?.position;
			return (CommonSexNPCScript)tree;
		}
	}
}
