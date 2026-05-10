using System.Linq;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using UnityEngine;


namespace HFramework.SexScripts
{
	[CreateAssetMenu(fileName = "PlayerRaped", menuName = "HFramework/Player Raped", order = 20)]
	[Experimental]
	[SexScriptType(SexScriptTypes.PlayerRaped)]
	public class PlayerRapedScript : SexScript
	{
		/// <summary>
		/// (Editor-only) Called by Unity when the script is created/reset.
		/// Puts the default context tags for this kind of script (user can customize if wanted)
		/// </summary>
		public void Reset() {
			this.Info.ContextTags.Add(ContextTags.Forced);
		}

		public override SexScript Create(CommonStates[] actors, SexInfo info) {
			var tree = Clone();
			tree.Context.Actors = this.Info.BuildNpcs(actors).Select(npc => new ContextNpc(npc, null)).ToArray();
			if (actors.Length > 1 && actors[1] != null) {
				tree.Context.ScriptPlace = new GroundScriptPlace(actors[1].gameObject.transform.position);
			}
			return tree;
		}
	}
}
