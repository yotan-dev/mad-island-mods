using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace YoUnnoficialPatches.Patches
{
	/**
	 * Tries to fix some censor/mosaic that the original game can't clean up properly.
	 * If true, it will fix them according to your censor configs.
	 */
	internal class FixMosaicPatch
	{
		private class TargetMosaic
		{
			public string BaseObjectName;

			public string AnimPath;

			public string MaterialName;

			public bool Patched = false;
		}

		private static List<TargetMosaic> Targets = [
			new TargetMosaic() {
				BaseObjectName = "gen_01_sex_yona_01",
				AnimPath = "Scale/Anim",
				MaterialName = "gen_01_rapes_yona_01_gen_01_rapes_yona_012",
			},
			new TargetMosaic() {
				BaseObjectName = "girl_01_down",
				AnimPath = "Scale/Anim",
				MaterialName = "girl_01_down_girl_01_down2",
			},
			new TargetMosaic() {
				BaseObjectName = "gengirl_01_onani_01",
				AnimPath = "Scale/Anim",
				MaterialName = "gengirl_01_ona_01_gengirl_01_ona_012",
			},
		];
		[HarmonyPrefix, HarmonyPatch(typeof(RandomCharacter), "SetCharacter")]
		private static void Pre_RandomCharacter_SetCharacter(GameObject hChara)
		{
			var objectsToPatch = Targets.FindAll(x => x.BaseObjectName == hChara.name && !x.Patched);
			if (objectsToPatch.Count == 0)
				return;

			foreach (var target in objectsToPatch)
			{
				foreach (var mat in hChara.transform.Find(target.AnimPath).GetComponent<MeshRenderer>().sharedMaterials)
				{
					if (mat.name == target.MaterialName)
					{
						mat.SetFloat("_BlockSize", GameInfo.CensorBlockSize);
						target.Patched = true;
					}
				}
			}
		}
	}
}
