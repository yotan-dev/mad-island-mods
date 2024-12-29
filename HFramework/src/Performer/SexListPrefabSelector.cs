using UnityEngine;
using YotanModCore;

namespace HFramework.Performer
{
	public class SexListPrefabSelector : IPrefabSelector
	{
		private int ListIndex;
		private int ObjIndex;

		public SexListPrefabSelector(int listIndex, int objIndex)
		{
			this.ListIndex = listIndex;
			this.ObjIndex = objIndex;
		}

		public GameObject GetPrefab()
		{
			return Managers.mn.sexMN.sexList[this.ListIndex].sexObj[this.ObjIndex];
		}
	}
}
