using System.Linq;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	[SexScriptType(SexScriptTypes.CommonSexNPC)]
	public class CommonSexNPCScript : SexScript
	{
		public CommonSexNPCScript Create(CommonStates npcA, CommonStates npcB, SexPlace sexPlace) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(npcA, npcB).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.Context.ScriptPlace = new SexPlaceScriptPlace(sexPlace);
			return (CommonSexNPCScript)tree;
		}
	}
}
