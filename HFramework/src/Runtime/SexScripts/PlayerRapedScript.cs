using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;


namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	[SexScriptType(SexScriptTypes.PlayerRaped)]
	public class PlayerRapedScript : SexScript
	{
		public override SexScript Create(CommonStates[] actors, SexInfo info) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actors).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (actors.Length > 1 && actors[1] != null) {
				tree.Context.ScriptPlace = new GroundScriptPlace(actors[1].gameObject.transform.position);
			}
			return tree;
		}
	}
}
