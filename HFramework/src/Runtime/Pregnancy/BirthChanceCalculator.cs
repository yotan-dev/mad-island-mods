using HFramework.SexScripts.ScriptContext;

namespace HFramework.Pregnancy
{
	public class BirthChanceCalculator
	{
		public float CalculateBirthChance(CommonStates pregnantCommon, ScriptPlace place) {
			if (place is WorkplaceScriptPlace) {
				// This assumes Workplace is the granma bed, so 100%
				return 100f;
			}

			if (place is SexPlaceScriptPlace) {
				// This assumes SexPlace is the bed, so 60%
				return 60f;
			}

			// Ground, 20%
			return 20f;
		}
	}
}
