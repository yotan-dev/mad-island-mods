using UnityEngine;

namespace EnhancedIsland.RotateObject
{
	internal static class RotateUtils
	{
		internal static void RotateCheck(GameObject tmpBuild)
		{
			// @FIXME: Can't find how to make this work.
			// 1. making a negative x scale works reasonably well but:
			//  - it is not saved to save file
			//  - unity screams that we are making BoxCollider size negative
			// 2. Using eulerAngles to rotate works for simple objects but:
			//  - complex objects which has more than 1 child sprite get messed up (draw order goes wrong / objects disappear)
			if (PConfig.Instance.Key.Value.IsDown())
			{
				var rotation = tmpBuild.transform.eulerAngles;
				if (rotation.y == 180f) {
					rotation.y = 0;
					rotation.x = 0;
				} else {
					rotation.y = 180;
					rotation.x = 300;
				}
				tmpBuild.transform.eulerAngles = rotation;
			}
		}
	}
}
