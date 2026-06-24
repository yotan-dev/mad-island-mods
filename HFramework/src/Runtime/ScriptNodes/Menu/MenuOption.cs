using System;
using UnityEngine.Scripting.APIUpdating;

namespace HFramework.ScriptNodes.Menu
{
	[Serializable]
	[Experimental]
	[MovedFrom(true, "HFramework.ScriptNodes", null, "MenuOption")]
	public class MenuOption
	{
		public enum EffectType
		{
			ChangeState,
			Action,
		}

		public string Id = "";
		public string Text = "";
		public EffectType Effect = EffectType.ChangeState;
	}
}
