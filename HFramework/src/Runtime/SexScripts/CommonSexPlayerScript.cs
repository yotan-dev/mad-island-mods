using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;


namespace HFramework.SexScripts
{
	[CreateAssetMenu()]
	[Experimental]
	[SexScriptType(SexScriptTypes.CommonSexPlayer)]
	public class CommonSexPlayerScript : SexScript
	{
		public override SexScript Create(CommonStates[] actors, SexInfo info) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actors).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (info is IHasSexPos hasSexPos) {
				tree.Context.ScriptPlace = new GroundScriptPlace(hasSexPos.Pos);
			}
			if (info is IHasSexType hasSexType) {
				tree.Context.SexType = hasSexType.SexType;
			}
			return tree;
		}
	}
}
