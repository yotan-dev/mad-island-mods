using UnityEngine;
using YotanModCore.Extensions;

namespace HFramework.Tree
{
	[Experimental]
	public class SexStartNode : ActionNode
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
