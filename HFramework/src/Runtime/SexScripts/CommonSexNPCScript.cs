using System.Linq;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	[SexScriptType(SexScriptTypes.CommonSexNPC)]
	public class CommonSexNPCScript : SexScript
	{
		public override SexScript Create(CommonStates[] actors, SexInfo info) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actors).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (info is IHasScriptPlace hasScriptPlace) {
				tree.Context.ScriptPlace = hasScriptPlace.Place;
			}
			return tree;
		}
	}
}
