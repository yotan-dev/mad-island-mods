using System;
using System.Linq;

namespace HFramework.SexScripts.Info.Conditions
{
	[Serializable]
	[Experimental]
	public class SexType : Condition
	{
		public int[] SexTypeValues;

		public override bool CanExecute(SexInfo info) {
			if (info is IHasSexType hasSexType) {
				return this.SexTypeValues.Contains(hasSexType.SexType);
			}

			PLogger.LogError("SexType condition not supported for this script!!");
			return false;
		}
	}
}
