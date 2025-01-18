using UnityEngine;
using YotanModCore;

namespace HFramework.Performer
{
	public class SexObjPrefabSelector : BasePrefabSelector, IPrefabSelector
	{
		private int ObjIndex;

		public SexObjPrefabSelector() { }

		public SexObjPrefabSelector(int objIndex)
		{
			this.ObjIndex = objIndex;
		}

		public override GameObject GetPrefab()
		{
			return Managers.mn.sexMN.sexObj[this.ObjIndex];
		}
	}
}
