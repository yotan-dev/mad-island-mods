using System.Xml.Serialization;
using UnityEngine;

namespace HFramework.Performer
{
	[XmlInclude(typeof(SexListPrefabSelector))]
	[XmlInclude(typeof(SexObjPrefabSelector))]
	public abstract class BasePrefabSelector : IPrefabSelector
	{
		public abstract GameObject GetPrefab();
	}
}
