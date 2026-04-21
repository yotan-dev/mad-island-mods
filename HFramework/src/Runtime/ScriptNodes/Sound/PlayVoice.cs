using YotanModCore;

namespace HFramework.ScriptNodes.Sound
{
	[ScriptNode("HFramework", "Sound/Play Voice")]
	public class PlayVoice : Action
	{
		[ActorIndex]
		public int Actor;

		public string VoiceType;

		protected override void OnStart() {

		}

		protected override State OnUpdate() {
			var actor = this.Context.Actors[this.Actor].Common;
			Managers.mn.sound.GoVoice(actor.voiceID, this.VoiceType, actor.transform.position);

			return State.Success;
		}

		protected override void OnStop() {

		}
	}
}
