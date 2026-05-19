using HFramework.SexScripts.ScriptContext;
using YotanModCore;

namespace HFramework.Events.EventHandlers
{
	/// <summary>
	/// Handles all places where we need to increate SexCount.Normals
	/// </summary>
	internal class NormalCountEventHandler
	{
		internal static NormalCountEventHandler Instance { get; private set; } = new NormalCountEventHandler();

		internal void Init() {
			// Only penetration of ass/vagina are counted as normal sex
			SexEvents.OnPenetrateVagina.Triggered += this.OnPenetrationStart;
			SexEvents.OnPenetrateAss.Triggered += this.OnPenetrationStart;
		}

		private void OnPenetrationStart(object sender, FromToEventArgs e) {
			if (e.Ctx.SexScript.Info.ContextTags.Contains(ContextTags.Normal)) {
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Normal);
			}
		}
	}
}
