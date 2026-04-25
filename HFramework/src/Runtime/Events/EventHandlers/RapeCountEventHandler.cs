using HFramework.SexScripts.ScriptContext;
using YotanModCore;

namespace HFramework.Events.EventHandlers
{
	/// <summary>
	/// Handles all places where we need to increate SexCount.Rapes
	/// </summary>
	internal class RapeCountEventHandler
	{
		internal static RapeCountEventHandler Instance { get; private set; } = new RapeCountEventHandler();

		internal void Init() {
			SexEvents.OnPenetrateVagina.Triggered += this.OnActStart;
			SexEvents.OnPenetrateAss.Triggered += this.OnActStart;
			SexEvents.OnPerformHandJob.Triggered += this.OnActStart;
			SexEvents.OnPerformTitFuck.Triggered += this.OnActStart;
			SexEvents.OnPerformScissor.Triggered += this.OnActStart;
			SexEvents.OnPenetrateMouth.Triggered += this.OnActStart;
			SexEvents.OnLickVagina.Triggered += this.OnActStart;
		}

		private void OnActStart(object sender, FromToEventArgs e) {
			// Forced sex, regardless of the type, is counted as rape on start
			if (e.ctx.SexScript.Info.ContextTags.Contains(ContextTags.Forced)) {
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Rapes);
			}
		}
	}
}
