using HFramework.Tree;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	public class CommonSexNPCScript : SexScript
	{
		public CommonSexNPCScript Create(CommonStates npcA, CommonStates npcB, SexPlace sexPlace) {
			var tree = Clone();
			tree.context.NpcA = npcA;
			tree.context.NpcB = npcB;
			tree.context.SexPlace = sexPlace;
			return (CommonSexNPCScript)tree;
		}
	}
}
