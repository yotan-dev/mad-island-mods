using UnityEngine;
using YotanModCore;

namespace HFramework.Performer
{
	public class SexObjPrefabSelector : IPrefabSelector
	{
		private int ObjIndex;

		public SexObjPrefabSelector(int objIndex)
		{
			this.ObjIndex = objIndex;
		}

		public GameObject GetPrefab()
		{
			return Managers.mn.sexMN.sexObj[this.ObjIndex];
		}
	}
}
