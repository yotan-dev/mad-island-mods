using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using static CraftManager;

namespace YotanModCore.Items
{
	/// <summary>
	/// Holds craft station recipes
	/// </summary>
	public class CraftDB : ScriptableObject
	{
		public static CraftDB Instance { get; private set; }

		private static readonly Dictionary<string, int> DefaultBenches = new()
		{
			// Official benches from:
			// - Can be found by looking at StaticGroup > CraftManager
			{ "@other:hand", 0 },
			{ "campfire_01", 1 },
			{ "bench_wood", 2 },
			{ "bench_iron", 3 },
			{ "furnace_01", 4 },
			{ "bench_meat", 5 },
			{ "bench_spider", 6 },
			{ "bench_mens", 7 },
			{ "bench_plant", 8 },
			{ "bench_wing", 9 },
			{ "bench_sand", 10 },
			{ "bench_pretty", 11 },
			{ "@npc:OldWomanNative", 12 },
			{ "@npc:OldManNative", 13 },
			{ "@npc:ElderSisterNative", 14 },
			{ "@npc:ElderBrotherNative", 15 },
			{ "@npc:Cassie2", 16 },
			{ "bench_xmas", 17 },
			{ "stonemill_01", 18 },
			{ "bench_cook_01", 19 },
			{ "bench_cook_02", 20 },
			{ "@npc:Mermaid", 21 },
			{ "bench_summer", 22 },
			{ "bench_halloween", 23 },
			{ "bench_hand", 24 },
			{ "bench_workshop", 25 },
			{ "bench_chaos", 26 },
			{ "@npc:Merry", 27 },
			{ "bench_surround", 28 },
			{ "bench_spring", 29 },
		};

		/// <summary>
		/// Maps Bench IDs to Craft IDs
		/// Note that some benches does not have a linked item id, and thus we use a special prefix:
		/// - <x> -- no prefix, bench item is <x>
		/// - @other:<x> -- no special asignment (E.g. hand)
		/// - @npc:<x> -- craft from npc <x>, where <x> is a constant from YotanModCore NpcID
		/// </summary>
		private Dictionary<string, int> BenchToCraftId = new(CraftDB.DefaultBenches);

		private readonly Dictionary<string, List<CraftInfo>> RecipesForBench = [];

		private CraftInfo BuildCraftInfo(CraftRecipe recipe)
		{
			var targetItem = Managers.mn.itemMN.FindItem(recipe.ItemKey);
			if (targetItem == null)
				throw new ConstraintException($"No item with code {recipe.ItemKey}");

			var gameObj = new GameObject($"CraftInfo_{recipe.BenchItemKey}_{recipe.ItemKey}");
			GameObject.DontDestroyOnLoad(gameObj);

			gameObj.AddComponent<CustomItemData>().ShallowCopy(targetItem);

			var craftInfo = gameObj.AddComponent<CraftInfo>();
			craftInfo.name = recipe.ItemKey;

			craftInfo.required = new CraftInfo.Required[recipe.Materials.Count];
			for (int i = 0; i < recipe.Materials.Count; i++)
			{
				var material = Managers.mn.itemMN.FindItem(recipe.Materials[i].ItemCode);
				if (material == null)
					throw new ConstraintException($"No item with code {recipe.Materials[i].ItemCode}");

				craftInfo.required[i] = new CraftInfo.Required()
				{
					itemData = material,
					count = recipe.Materials[i].Count,
				};
			}

			return craftInfo;
		}

		public bool RegisterCraft(CraftRecipe recipe)
		{
			try
			{
				var recipeList = RecipesForBench.GetValueOrDefault(recipe.BenchItemKey, null);
				if (recipeList == null)
				{
					recipeList = [];
					RecipesForBench.Add(recipe.BenchItemKey, recipeList);
				}

				recipeList.Add(this.BuildCraftInfo(recipe));
				return true;
			}
			catch (Exception e)
			{
				PLogger.LogError($"Failed to register craft {recipe?.ItemKey ?? "NULL"} for bench {recipe.BenchItemKey ?? "NULL"}");
				PLogger.LogError(e);
				return false;
			}
		}

		public int GetCraftIdForBench(string benchItemKey)
		{
			var craftId = BenchToCraftId.GetValueOrDefault(benchItemKey, -1);
			if (craftId == -1)
			{
				PLogger.LogWarning($"Unknown bench item key: {benchItemKey}");
				return -1;
			}

			return craftId;
		}

		internal void SetDefaultBenches()
		{
			this.BenchToCraftId = new Dictionary<string, int>(DefaultBenches);
		}

		internal void LoadRecipes(CraftManager craftManager)
		{
			PLogger.LogDebug("Loading recipes");

			// We need to load the default benches first whenever the manager is reloaded,
			// as we mutate that afterwards
			this.SetDefaultBenches();

			foreach (var benchKey in RecipesForBench.Keys)
			{
				var craftId = BenchToCraftId.GetValueOrDefault(benchKey, -1);
				var newRecipes = RecipesForBench.GetValueOrDefault(benchKey, null);

				// Already exists, append with new crafts
				if (craftId != -1)
				{
					var craftInfo = craftManager.craftData[craftId].craftInfo;
					var startIdx = craftInfo.Length;
					Array.Resize(ref craftInfo, craftInfo.Length + newRecipes.Count);
					craftManager.craftData[craftId].craftInfo = craftInfo;
					
					for (int i = 0; i < newRecipes.Count; i++)
						craftInfo[startIdx + i] = newRecipes[i];
				}
				else
				{
					craftId = craftManager.craftData.Length;
					
					var craftData = new CraftData()
					{
						craftName = benchKey,
						craftInfo = newRecipes.ToArray(),
					};

					craftManager.craftData = craftManager.craftData.Append(craftData).ToArray();

					BenchToCraftId.Add(benchKey, craftId);
				}
			}
		}

		internal static void Init()
		{
			Assert.IsNull(Instance, "CraftDB already exists!");

			CraftDB.Instance = ScriptableObject.CreateInstance<CraftDB>();
			PLogger.LogInfo("CraftDB Initialized");
		}

		public static bool IsDefaultBench(string benchKey)
		{
			return DefaultBenches.ContainsKey(benchKey);
		}
	}
}
