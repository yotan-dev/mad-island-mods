using UnityEngine;
using HFramework.SexScripts.ScriptContext;

namespace HFramework.ScriptNodes.PrefabSetup
{
	public interface IAppearanceSetter
	{
		void SetAppearance(GameObject prefab, CommonContext ctx);
	}
}
