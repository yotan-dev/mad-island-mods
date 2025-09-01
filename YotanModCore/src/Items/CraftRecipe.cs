#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;

namespace YotanModCore.Items
{
	/// <summary>
	/// Declares a Crafting Recipe.
	/// A Crafting Recipe enables an item to be created at a certain place by using certain materials
	/// </summary>
	[Experimental]
	[CreateAssetMenu(fileName = "New Craft Recipe", menuName = "Yotan Mod Core/Craft Recipe", order = 2)]
	public class CraftRecipe : ScriptableObject, ISerializationCallbackReceiver
	{
		/// <summary>
		/// "key" of the Bench used to craft the item. May be:
		/// - An Item Key, when the bench is a crafting station
		/// - `@other:hand` for hand crafting (no station)
		/// - `@npc:<name>` where <name> is the YotanModCore constant for NPC crafting
		/// </summary>
		[Tooltip("The key of the bench where this item is crafted at")]
		public string BenchItemKey = "";

		/// <summary>
		/// "key" of the resulting item
		/// </summary>
		[Tooltip("The key of the resulting item")]
		public string ItemKey = "";

		/// <summary>
		/// List of materials required to craft the item
		/// </summary>
		[NonSerialized]
		public List<ItemAmountEntry> Materials = [];

		/// <summary>
		/// List of materials required to craft the item.
		/// Must follow the pattern `<itemCode>:<count>`
		///
		/// This is later serailized to Materials, as Unity is not able to properly serialize
		/// objects inside an external DLL/AssetBundle.
		/// </summary>
		[Tooltip("List of materials required to craft the item. Must follow the pattern `<itemCode>:<count>`")]
		[SerializeField]
		private string[]? _Materials = [];

		public void OnAfterDeserialize()
		{
			this.Materials = [];
			if (_Materials == null)
			{
				Debug.LogWarning("_Materials is null. Initializing it to empty array.");
				_Materials = [];
			}

			for (int i = 0; i < _Materials.Length; i++)
			{
				var parts = _Materials[i].Split(':');
				if (parts.Length != 2 || parts[1].Length == 0)
					continue;

				this.Materials.Add(new ItemAmountEntry()
				{
					ItemCode = parts[0],
					Count = float.Parse(parts[1]),
				});
			}
		}

		public void OnBeforeSerialize()
		{
			/* Nothing to do here */
		}
	}
}
