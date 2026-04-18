using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.ScriptNodes
{
	[Experimental]
	public class SexStart : Action
	{
		protected override void OnStart()
		{

		}

		protected override void OnStop()
		{

		}

		protected override State OnUpdate()
		{
			// @TODO: Emit Penetration event.

			return State.Success;
		}
	}
}
