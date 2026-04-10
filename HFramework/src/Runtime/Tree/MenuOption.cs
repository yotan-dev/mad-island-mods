using System;

namespace HFramework.Tree
{
	[Serializable]
	[Experimental]
	public class MenuOption
	{
		public enum EffectType {
			ChangeState,
			Action,
		}

		public string Id = "";
		public string Text = "";
		public EffectType Effect = EffectType.ChangeState;
	}
}
