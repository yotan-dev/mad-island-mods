using System.Linq;
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
		/// <summary>
		/// (Editor-only) Called by Unity when the script is created/reset.
		/// Puts the default context tags for this kind of script (user can customize if wanted)
		/// </summary>
		public void Reset() {
			this.Info.ContextTags.Add(ContextTags.Normal);
		}

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
