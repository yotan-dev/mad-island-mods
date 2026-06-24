using System;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Flow.WaitNode
{
	[Serializable]
	[Experimental]
	[MovedFrom(true, "HFramework.ScriptNodes.WaitNode", null, "WaitMouseInput")]
	public class WaitMouseInput : WaitKind
	{
		public enum MouseButtonType
		{
			Left = 0,
			Right = 1,
			Middle = 2
		}

		public enum MouseInputType
		{
			Up,
			Down
		}

		public MouseButtonType Button;

		public MouseInputType InputType;

		public override void Start() {

		}

		public override bool IsDone() {
			switch (this.InputType) {
				case MouseInputType.Down:
					return Input.GetMouseButtonDown((int)this.Button);

				case MouseInputType.Up:
					return Input.GetMouseButtonUp((int)this.Button);
			}

			return false;
		}
	}
}
