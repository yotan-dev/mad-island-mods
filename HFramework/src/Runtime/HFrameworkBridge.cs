#nullable enable

using System;
using System.Collections;
using HFramework.Performer;
using HFramework.Scenes;
using YotanModCore;
using YotanModCore.Consts;
using UnityEngine;
using HFramework.SexScripts;
using System.Collections.Generic;
using HFramework.SexScripts.Info;
using HFramework.SexScripts.ScriptContext;
using System.Linq;

namespace HFramework
{
	/// <summary>
	/// Bridge between HFramework and HFrameworkLoader.
	/// Result of <see cref="Initializer.Init(ILogger)"/> method.
	///
	/// This class should be kept at the bare minimum and just call appropriate methods in other classes.
	///
	/// NOTE: This is meant to be used by HFrameworkLoader only, to work around some limitations of the split.
	/// DO NOT USE THIS ELSEWHERE!
	/// </summary>
	public class HFrameworkBridge
	{
		public void AfterStartTicks() {
			if (HFConfig.Instance.IsLegacyModeEnabled) {
				PerformerLoader.Load();
				ScenesManager.Instance.Init();
			}

			if (HFConfig.Instance.IsModernModeEnabled) {
				BundleLoader.Load();
			}
		}

		#region NPCMove

		public static event EventHandler<NPCMove.ActType>? OnActTypeChanged;

		public void NPCMove_Pre_Wait(NPCMove __instance) {
			OnActTypeChanged?.Invoke(__instance, __instance.actType);
		}

		#endregion

		#region SexChecks

		public bool SexManager_Pre_SexCheck(CommonStates from, CommonStates to, ref bool __result) {
			if (from.npcID == CommonUtils.GetActivePlayer().npcID)
				__result = SexScriptsManager.Instance.CanStart(SexScriptTypes.CommonSexPlayer, from, to);
			else
				__result = SexScriptsManager.Instance.CanStart(SexScriptTypes.CommonSexNPC, from, to);

			return false;
		}

		public bool SexManager_Pre_RapesCheck(CommonStates from, CommonStates to, ref bool __result) {
			__result = false;

			int activePlayer = CommonUtils.GetActivePlayer().npcID;
			string typeName;
			if (to.npcID == activePlayer) {
				typeName = SexScriptTypes.PlayerRaped;
			} else if (from.npcID == activePlayer) {
				if (to.nMove.actType == NPCMove.ActType.Sleep && to.anim.state.GetCurrent(0).Animation.Name == "A_sleep")
					typeName = SexScriptTypes.ManRapesSleep;
				else
					typeName = SexScriptTypes.ManRapes;
			} else {
				// Npc x Npc
				return true; // @TODO: We don't have this yet
			}

			return SexScriptsManager.Instance.CanStart(typeName, from, to);
		}

		public bool NPCManager_Post_LoveLoveCheck(bool result, CommonStates commonA, CommonStates commonB) {
			// This is a check where we want to know if commonB accepts sex from commonA

			var sexTo = commonB;
			var sexFrom = commonA;

			// Player characters doesn't track love by default, so swap them so we can get the love of the NPC only.
			// @TODO: Can we improve that?
			if (sexTo.npcID == NpcID.Yona || sexTo.npcID == NpcID.Man) {
				var temp = sexTo;
				sexTo = sexFrom;
				sexFrom = temp;
			}

			int loversID = Managers.mn.npcMN.GetLoversID(sexTo, sexFrom.friendID);
			float loveVal = -1;
			if (loversID != -1)
				loveVal = sexTo.lovers[loversID].love;

			return loveVal >= 50f;
		}

		#endregion

		#region AssWall

		public bool SexManager_Pre_AssWall(InventorySlot tmpWall, ref IEnumerator __result) {
			var info = new CommonSexInfo {
				Place = new SexPlaceScriptPlace(tmpWall.GetComponent<SexPlace>()),
			};

			var actors = new CommonStates[] {
				CommonUtils.GetActivePlayer(),
				Managers.mn.inventory.itemSlot[50].common
			};
			var runner = SexScriptsManager.Instance.GetScript(SexScriptTypes.AssWall, actors, info);
			if (runner != null) {
				__result = runner();
			} else {
				PLogger.LogWarning($"No AssWall script found for actors {string.Join(", ", actors.Select(a => a.name))}");
			}

			return false;
		}

		#endregion

		#region CommonSexNPC
		public bool SexManager_Pre_CommonSexNPC(CommonStates npcA, CommonStates npcB, SexPlace sexPlace, ref IEnumerator __result) {
			var actors = new CommonStates[] { npcA, npcB };
			var info = new CommonSexInfo {
				Place = new SexPlaceScriptPlace(sexPlace)
			};

			var runner = SexScriptsManager.Instance.GetScript(SexScriptTypes.CommonSexNPC, actors, info);
			if (runner != null) {
				__result = runner();
			} else {
				PLogger.LogWarning($"No CommonSexNPC script found for actors {string.Join(", ", actors.Select(a => a.name))}");
			}

			return false;
		}
		#endregion

		#region CommonSexPlayer

		public bool SexManager_Pre_CommonSexPlayer(CommonStates pCommon, CommonStates nCommon, Vector3 pos, int sexType, ref IEnumerator __result) {
			var info = new PlayerSexInfo {
				Pos = pos,
				SexType = sexType,
			};
			var actors = new CommonStates[] { pCommon, nCommon };

			var runner = SexScriptsManager.Instance.GetScript(SexScriptTypes.CommonSexPlayer, actors, info);
			if (runner != null) {
				__result = runner();
			} else {
				PLogger.LogWarning($"No CommonSexPlayer script found for {pCommon.name} and {nCommon.name}");
			}

			return false;
		}

		#endregion

		#region Daruma

		public bool SexManager_Pre_DarumaSex(int state, InventorySlot tmpDaruma, ref IEnumerator __result) {
			var pCommon = CommonUtils.GetActivePlayer();
			var girlCommon = Managers.mn.inventory.itemSlot[50].common;

			var scene = new Daruma(pCommon, girlCommon, tmpDaruma);
			__result = scene.Run();

			return false;
		}

		#endregion

		#region Delivery

		public bool SexManager_Pre_Delivery(CommonStates common, WorkPlace tmpWorkPlace, SexPlace tmpSexPlace, ref IEnumerator __result) {
			var info = new CommonSexInfo();
			if (tmpWorkPlace != null) {
				info.Place = new WorkplaceScriptPlace(tmpWorkPlace);
			} else if (tmpSexPlace != null) {
				info.Place = new SexPlaceScriptPlace(tmpSexPlace);
			}
			var actors = new CommonStates[] { common };

			var runner = SexScriptsManager.Instance.GetScript(SexScriptTypes.Delivery, actors, info);
			if (runner != null) {
				__result = runner();
			} else {
				PLogger.LogWarning($"No Delivery script found for {common.name}");
			}

			return false;
		}

		#endregion

		#region ManRapes

		public bool SexManager_Pre_ManRapes(CommonStates girl, CommonStates man, ref IEnumerator __result) {
			var scene = new ManRapes(girl, man);
			__result = scene.Run();
			return false;
		}

		#endregion

		#region ManRapesSleep

		public bool SexManager_Pre_ManRapesSleep(CommonStates girl, CommonStates man, ref IEnumerator __result) {
			var scene = new ManRapesSleep(girl, man);
			__result = scene.Run();
			return false;
		}

		#endregion

		#region OnaniNPC

		public bool SexManager_Pre_OnaniNPC(CommonStates common, SexPlace sexPlace, float upMoral, ref IEnumerator __result) {
			var scene = new OnaniNPC(common, sexPlace, upMoral);
			__result = scene.Run();
			return false;
		}

		#endregion

		#region PlayerRaped

		public bool SexManager_Pre_PlayerRaped(CommonStates to, CommonStates from, ref IEnumerator __result) {
			var info = new SexInfo();
			var actors = new CommonStates[] { from, to };

			var runner = SexScriptsManager.Instance.GetScript(SexScriptTypes.PlayerRaped, actors, info);
			if (runner != null) {
				__result = runner();
			} else {
				PLogger.LogWarning($"No PlayerRaped script found for {to.name}");
			}

			return false;
		}

		#endregion

		#region Slave

		public bool SexManager_Pre_Slave(InventorySlot tmpSlave, ref IEnumerator __result) {
			var scene = new Slave(CommonUtils.GetActivePlayer(), tmpSlave);
			__result = scene.Run();
			return false;
		}

		public IEnumerator SexManager_Post_Slave(IEnumerator result) {
			while (result.MoveNext())
				yield return result.Current;

			// The original caller sets this and expects the scene to clean it up...
			// but this is not the responsability of the scene, so here we go.
			if (Managers.mn.uiMN.propActProgress == 5)
				Managers.mn.uiMN.propActProgress = PropPanelConst.Type.None;
		}

		#endregion

		#region Toilet

		public bool SexManager_Pre_Toilet(InventorySlot tmpToile, ref IEnumerator __result) {
			var scene = new Toilet(CommonUtils.GetActivePlayer(), Managers.mn.inventory.itemSlot[50].common, tmpToile);
			__result = scene.Run();
			return false;
		}

		#endregion

		#region TranspilePlayerCheck

		/// <summary>
		/// Checks whether st can sex with a NPC.
		/// It should return > 1 if it can (currently, returns 999)
		/// </summary>
		/// <param name="st"></param>
		/// <returns></returns>
		public int CanSexWithNpc(CommonStates st) {
			// We allow player characters to do it, as long as they are not the active player
			// otherwise it would block player control.
			// This is being controlled by the sex scenes anyway, and in the default game it would
			// never happen.
			int ret = 0;
			if (st.npcID != CommonUtils.GetActivePlayer().npcID)
				ret = 999;

			return ret;
		}

		#endregion
	}
}
