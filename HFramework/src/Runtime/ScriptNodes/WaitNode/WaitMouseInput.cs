using System;
using UnityEngine;

namespace HFramework.ScriptNodes.WaitNode
{
	[Serializable]
	[Experimental]
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
