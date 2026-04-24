using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;


namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	[SexScriptType(SexScriptTypes.CommonSexPlayer)]
	public class CommonSexPlayerScript : SexScript
	{
		public CommonSexPlayerScript Create(CommonStates actorA, CommonStates actorB, Vector3 pos, int sexType) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actorA, actorB).Select(npc => new ContextNpc(npc, null)).ToArray();
			tree.Context.ScriptPlace = new GroundScriptPlace(pos);
			tree.Context.SexType = sexType;
			return (CommonSexPlayerScript)tree;
		}
	}
}
