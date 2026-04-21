using System.Linq;
using HFramework.ScriptNodes;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "DeliveryScript", menuName = "HFramework/DeliveryScript")]
	[Experimental]
	public class DeliveryScript : SexScript
	{
		public DeliveryScript Create(CommonStates common, WorkPlace workPlace, SexPlace sexPlace) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(common, null).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.Context.SexPlace = sexPlace;
			tree.Context.SexPlacePos = sexPlace.transform.Find("pos")?.position;
			return (DeliveryScript)tree;
		}
	}
}
