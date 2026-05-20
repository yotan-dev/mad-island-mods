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
#pragma warning disable CS0618 // Type or member is obsolete
			SexEvents.OnPenetrateVagina.Triggered += this.OnActStart;
			SexEvents.OnPenetrateAss.Triggered += this.OnActStart;
			SexEvents.OnPerformHandJob.Triggered += this.OnActStart;
			SexEvents.OnPerformTitFuck.Triggered += this.OnActStart;
			SexEvents.OnPerformScissor.Triggered += this.OnActStart;
			SexEvents.OnPenetrateMouth.Triggered += this.OnActStart;
			SexEvents.OnLickVagina.Triggered += this.OnActStart;
#pragma warning restore CS0618 // Type or member is obsolete
			SexEvents.OnInteractPenis2Vagina.Triggered += this.OnActStart;
			SexEvents.OnInteractPenis2Ass.Triggered += this.OnActStart;
			SexEvents.OnInteractPenis2Mouth.Triggered += this.OnActStart;
			SexEvents.OnInteractPenis2Hand.Triggered += this.OnActStart;
			SexEvents.OnInteractPenis2Tits.Triggered += this.OnActStart;
			SexEvents.OnInteractVagina2Vagina.Triggered += this.OnActStart;
			SexEvents.OnInteractTongue2Vagina.Triggered += this.OnActStart;
		}

		private void OnActStart(object sender, FromToEventArgs e) {
			// Forced sex, regardless of the type, is counted as rape on start
			if (e.Ctx.SexScript.Info.ContextTags.Contains(ContextTags.Forced)) {
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Rapes);
			}
		}
	}
}
