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
#pragma warning disable CS0618 // Type or member is obsolete
			SexEvents.OnPenetrateVagina.Triggered += this.OnPenetrationStart;
			SexEvents.OnPenetrateAss.Triggered += this.OnPenetrationStart;
#pragma warning restore CS0618 // Type or member is obsolete

			SexEvents.OnInteractPenis2Vagina.Triggered += this.OnPenetrationStart;
			SexEvents.OnInteractPenis2Ass.Triggered += this.OnPenetrationStart;
		}

		private void OnPenetrationStart(object sender, FromToEventArgs e) {
			if (e.Ctx.SexScript.Info.ContextTags.Contains(ContextTags.Normal)) {
				Managers.sexMN.SexCountChange(e.To, e.From, SexManager.SexCountState.Normal);
			}
		}
	}
}
