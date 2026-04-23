using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "DeliveryScript", menuName = "HFramework/DeliveryScript")]
	[Experimental]
	public class DeliveryScript : SexScript
	{
		public DeliveryScript Create(CommonStates common, WorkPlace workPlace, SexPlace sexPlace) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(common).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (workPlace != null) {
				tree.Context.ScriptPlace = new WorkplaceScriptPlace(workPlace);
			} else if (sexPlace != null) {
				tree.Context.ScriptPlace = new SexPlaceScriptPlace(sexPlace);
			} else {
				tree.Context.ScriptPlace = new GroundScriptPlace(common.gameObject.transform.position);
			}

			return (DeliveryScript)tree;
		}
	}
}
