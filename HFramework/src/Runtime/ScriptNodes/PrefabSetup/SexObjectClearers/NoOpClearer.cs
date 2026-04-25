using HFramework.SexScripts.ScriptContext;

namespace HFramework.ScriptNodes.PrefabSetup.SexObjectClearers
{
	public class NoOpClearer : SexObjectClearer
	{
		public override void Clear(CommonContext ctx) {
			// No operation
		}
	}
}
