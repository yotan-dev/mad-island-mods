#nullable enable

namespace HFramework.SexScripts.ScriptContext
{
	[Experimental]
	public class ContextNpc
	{
		public CommonStates Common { get; set; }
		public float? Angle { get; set; }

		public ContextNpc(CommonStates common, float? angle) {
			Common = common;
			Angle = angle;
		}
	}
}
