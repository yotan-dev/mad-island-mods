using UnityEngine;
using YotanModCore.Items;
using System.Collections.Generic;

namespace YotanModCore
{

	[CreateAssetMenu(fileName = "YMCDataLoader", menuName = "Yotan Mod Core/Data Loader", order = 1)]
	public class YMCDataLoader : ScriptableObject
	{
		public List<CustomItemData> Items = [];
	}
}
