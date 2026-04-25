using HFramework.Scenes;
using UnityEngine;

namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Sex Meter/Create Sex Meter")]
	public class CreateSexMeter : Action
	{
		public float DividerPercent = 0.3f;

		public Vector3 Offset = new(1f, 1f, 0f);

		public bool ShowImmediately = true;

		protected override void OnStart() {
			this.Context.HasSexMeter = true;
		}

		protected override void OnStop() {
		}

		protected override State OnUpdate() {
			SexMeter.Instance.Init(this.Context.ScriptPlace.GetCharacterPosition() + this.Offset, this.DividerPercent);
			if (this.ShowImmediately) {
				SexMeter.Instance.Show();
			}

			return State.Success;
		}
	}
}
