using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;


namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	public class PlayerRapedScript : SexScript
	{
		public PlayerRapedScript Create(CommonStates rapist, CommonStates victim) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(rapist, victim).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.Context.SexPlace = null;
			tree.Context.SexPlacePos = null;
			return (PlayerRapedScript)tree;
		}
	}
}
