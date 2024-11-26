using HarmonyLib;
using UnityEngine;
using YotanModCore;

namespace WarpBody
{
	internal class PlayerMovePatch
	{
		private static Vector3 GetRespawnPos()
		{
			GameObject player = Managers.mn.gameMN.player;
			RevivePoint[] componentsInChildren = GameObject.Find("StaticBG/RevivePoint").GetComponentsInChildren<RevivePoint>(true);
			float num = 1000f;
			int num2 = 0;
			for (int i = 0; i < componentsInChildren.Length; i++) {
				float num3 = Vector3.Distance(player.transform.position, componentsInChildren[i].transform.position);
				if (num3 <= num && componentsInChildren[i].gameObject.name == "Rev_Gen") {
					num2 = i;
					num = num3;
				}
			}

			return componentsInChildren[num2].transform.position;
		}

		[HarmonyPatch(typeof(PlayerMove), "Update")]
		[HarmonyPostfix]
		private static void Post_PlayerMove_Update(PlayerMove __instance)
		{
			if (Input.GetKeyDown(KeyCode.P) && __instance.searchNPC != null)
			{
				WarpBody(__instance, __instance.searchNPC);
			}
		}

		private static void WarpBody(PlayerMove playerMove, CommonStates npc)
		{
			if (npc.dead != 1) {
				playerMove.StartCoroutine(Managers.mn.eventMN.GoCautionSt("NPC is not dead."));
				return;
			}

			Managers.mn.npcMN.NPCTeleport(npc, PlayerMovePatch.GetRespawnPos());
		}
	}
}