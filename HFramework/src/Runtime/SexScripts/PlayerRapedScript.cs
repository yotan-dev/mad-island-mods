using System.Linq;
using HFramework.ScriptNodes;
using UnityEngine;


namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	public class PlayerRapedScript : SexScript
	{
		public PlayerRapedScript Create(CommonStates rapist, CommonStates victim) {
			var tree = Clone();
			tree.context.Actors = this.Info.BuildNpcs(rapist, victim).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.context.SexPlace = null;
			tree.context.SexPlacePos = null;
			return (PlayerRapedScript)tree;
		}
	}
}
