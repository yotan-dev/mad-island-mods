using System.Collections;
using YotanModCore;

namespace HFramework.ScriptNodes.Sound
{
	/// <summary>
	/// Fades out the specified BGM.
	/// It will wait until the fade is complete before completing.
	/// </summary>
	[Experimental]
	[ScriptNode("HFramework", "Sound/BGM Fade")]
	public class BGMFade : Action
	{
		public int BGMId;

		private IEnumerator coroutine;

		private IEnumerator BGMFadeCoroutine() {
			yield return Managers.sexMN.StartCoroutine(Managers.mn.sound.GoBGMFade(this.BGMId));
		}

		protected override void OnStart() {
			coroutine = BGMFadeCoroutine();
		}

		protected override State OnUpdate() {
			if (coroutine?.MoveNext() ?? false) {
				return State.Running;
			}

			return State.Success;
		}

		protected override void OnStop() {
			if (coroutine != null) {
				Managers.sexMN.StopCoroutine(coroutine);
				coroutine = null;
			}
		}
	}
}
