using System.Linq;
using HFramework.Tree;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	public class CommonSexNPCScript : SexScript
	{
		public CommonSexNPCScript Create(CommonStates npcA, CommonStates npcB, SexPlace sexPlace) {
			var tree = Clone();
			tree.context.Npcs = [.. this.Info.BuildNpcs(npcA, npcB).Select(npc => new ContextNpc(npc, null))];
			tree.context.SexPlace = sexPlace;
			return (CommonSexNPCScript)tree;
		}
	}
}
