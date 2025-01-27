using System.Xml.Serialization;
using UnityEngine;
using YotanModCore;

namespace HFramework.Performer
{
	public class SexObjPrefabSelector : BasePrefabSelector, IPrefabSelector
	{
		[XmlAttribute("objIndex")]
		public int ObjIndex { get; set; }

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
