using System.Linq;
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
		/// <summary>
		/// (Editor-only) Called by Unity when the script is created/reset.
		/// Puts the default context tags for this kind of script (user can customize if wanted)
		/// </summary>
		public void Reset() {
			this.Info.ContextTags.Add(ContextTags.Toilet);
		}

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
