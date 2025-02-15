using UnityEngine;

namespace HFramework.Performer
{
	public abstract class BasePrefabSelector : IPrefabSelector
	{
		public abstract GameObject GetPrefab();
	}
}
