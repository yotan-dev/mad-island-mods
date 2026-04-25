using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "DeliveryScript", menuName = "HFramework/DeliveryScript")]
	[Experimental]
	[SexScriptType(SexScriptTypes.Delivery)]
	public class DeliveryScript : SexScript
	{
		public override SexScript Create(CommonStates[] actors, SexInfo info) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actors).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (info is IHasScriptPlace hasScriptPlace) {
				tree.Context.ScriptPlace = hasScriptPlace.Place;
			} else if (actors.Length > 0 && actors[0] != null) {
				tree.Context.ScriptPlace = new GroundScriptPlace(actors[0].gameObject.transform.position);
			}

			return tree;
		}
	}
}
