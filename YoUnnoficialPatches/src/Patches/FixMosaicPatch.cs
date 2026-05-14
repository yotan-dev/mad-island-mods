using System;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace YoUnnoficialPatches.Patches
{
	/**
	 * Tries to fix some censor/mosaic that the original game can't clean up properly.
	 * It will fix them according to your censor configs.
	 */
	internal class FixMosaicPatch
	{
		private class TargetMosaic
		{
			public Func<GameObject> GetPrefab;

			public string AnimPath;

			public string AnimationName;

			public string MaterialName;

			public bool Patched = false;
		}

		private static readonly List<TargetMosaic> Targets = [
			// Sex Prefabs
			new TargetMosaic() { // 2-4
				GetPrefab = () => Managers.sexMN.sexList[2].sexObj[4], //"gen_01_sex_yona_01",
				AnimPath = "Scale/Anim",
				AnimationName = "A_Loop_01",
				MaterialName = "gen_01_rapes_yona_01_gen_01_rapes_yona_012",
			},
			new TargetMosaic() { // 6-1
				GetPrefab = () => Managers.sexMN.sexList[6].sexObj[1], // "boyfriend_01_rapes_yona_01_prefab",
				AnimPath = "Scale/Anim",
				AnimationName = "A_Loop_01",
				MaterialName = "boyfriend_01_rapes_yona_01_boyfriend_01_rapes_yona_012",
			},
			new TargetMosaic() { // 9-1
				GetPrefab = () => Managers.sexMN.sexList[9].sexObj[1], // "gengirl_01_onani_01",
				AnimPath = "Scale/Anim",
				AnimationName = "A_Loop_01",
				MaterialName = "gengirl_01_ona_01_gengirl_01_ona_012",
			},

			// Yona after defeat (downed in respawn)
			new TargetMosaic() {
				GetPrefab = () => Managers.sexMN.downBody, // "girl_01_down",?
				AnimPath = "Scale/Anim",
				AnimationName = "A_idle",
				MaterialName = "girl_01_down_girl_01_down2",
			},

			// Takumi NPC
			new TargetMosaic() {
				GetPrefab = () => Managers.mn.npcMN.npcPrefab[NpcID.Takumi],
				AnimPath = "scale/Anim",
				AnimationName = "A_idle",
				MaterialName = "boyfriend_01_boyfriend_013",
			},
		];

		internal static void Apply() {
			int index = 0;
			foreach (var target in Targets) {
				string name = $"<Unkown_{index}>";
				index++;

				GameObject sexObj = null;
				try {
					// Create a dummy sex prefab
					sexObj = GameObject.Instantiate(target.GetPrefab());
					name = sexObj.name;

					// Set the animation to the one that triggers the material to exist
					var animObj = sexObj.transform.Find(target.AnimPath);
					if (animObj == null) {
						throw new Exception($"Could not find animation object {target.AnimPath} on {name}");
					}

					var skelAnim = animObj.GetComponent<SkeletonAnimation>();
					if (skelAnim == null) {
						throw new Exception($"Could not find SkeletonAnimation component on {target.AnimPath} on {name}");
					}

					var track = skelAnim.state.SetAnimation(0, target.AnimationName, false);
					track.TimeScale = 0; // Keep it paused so we don't run other stuff

					// Find the material and set the block size
					bool found = false;
					foreach (var material in skelAnim.GetComponent<MeshRenderer>().sharedMaterials) {
						if (material.name == target.MaterialName) {
							material.SetFloat("_BlockSize", GameInfo.CensorBlockSize);
							found = true;
							break;
						}
					}

					if (!found) {
						throw new Exception($"Could not find material {target.MaterialName} on {name}");
					}
				} catch (Exception ex) {
					PLogger.LogError($"Failed to fix mosaic for {name}: {ex.Message}");
					PLogger.LogError(ex);
				} finally {
					// Clean up (regardless of success or failure)
					if (sexObj != null) {
						GameObject.Destroy(sexObj);
					}
				}
			}

			PLogger.LogInfo("Mosaics fixed.");
		}
	}
}
