using System.Linq;
using HFramework.ScriptNodes;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;

namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "AssWallScript", menuName = "HFramework/AssWallScript")]
	[SexScriptType(SexScriptTypes.AssWall)]
	[Experimental]
	public class AssWallScript : SexScript
	{
		public override SexScript Create(CommonStates[] actors, SexInfo info) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actors[0], actors[1]).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (info is IHasScriptPlace hasSexPlace) {
				tree.Context.ScriptPlace = hasSexPlace.Place;
			} else {
				PLogger.LogError($"AssWallScript: {info.GetType().Name} SexInfo does not implement IHasSexPlace, and was passed to AssWallScript ({this.UniqueID})");
			}

			return tree;
		}
	}
}
