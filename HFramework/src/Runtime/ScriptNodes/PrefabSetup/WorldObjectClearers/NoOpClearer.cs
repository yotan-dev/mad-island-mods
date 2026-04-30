using HFramework.SexScripts.ScriptContext;

namespace HFramework.ScriptNodes.PrefabSetup.WorldObjectClearers
{
	public class NoOpClearer : WorldObjectClearer
	{
		public override void Clear(CommonContext ctx) {
			// No operation
		}
	}
}
