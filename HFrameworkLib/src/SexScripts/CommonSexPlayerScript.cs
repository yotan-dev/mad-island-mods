using System.Linq;
using HFramework.Tree;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	public class CommonSexPlayerScript : SexScript
	{
		public CommonSexPlayerScript Create(CommonStates actorA, CommonStates actorB, Vector3 pos, int sexType) {
			var tree = Clone();
			tree.context.Npcs = [.. this.Info.BuildNpcs(actorA, actorB).Select(npc => new ContextNpc(npc, null))];
			tree.context.SexPlace = null;
			tree.context.SexPlacePos = pos;
			tree.context.SexType = sexType;
			return (CommonSexPlayerScript)tree;
		}
	}
}
