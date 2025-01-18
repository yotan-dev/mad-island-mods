using System.Xml.Serialization;
using UnityEngine;
using YotanModCore;

namespace HFramework.Performer
{
	public class SexListPrefabSelector : BasePrefabSelector, IPrefabSelector
	{
		[XmlAttribute("listIndex")]
		public int ListIndex { get; set; }

		[XmlAttribute("objIndex")]
		public int ObjIndex { get; set; }

		public SexListPrefabSelector() {}

		public SexListPrefabSelector(int listIndex, int objIndex)
		{
			this.ListIndex = listIndex;
			this.ObjIndex = objIndex;
		}

		public override GameObject GetPrefab()
		{
			return Managers.mn.sexMN.sexList[this.ListIndex].sexObj[this.ObjIndex];
		}
	}
}
