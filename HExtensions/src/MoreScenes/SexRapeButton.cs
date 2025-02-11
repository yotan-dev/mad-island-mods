using System.Collections;
using YotanModCore;
using YotanModCore.NpcTalk;

namespace HExtensions.MoreScenes
{
	public class SexRapeButton : TalkButton
	{
		public SexRapeButton() : base("Sex: Rape") { }

		public override bool ShouldShow(CommonStates common)
		{
			return true;
		}

		private bool IsHavingSex = false;

		

		public IEnumerator Sex(CommonStates girl, CommonStates man)
		{
			if (this.IsHavingSex)
				yield break;

			this.IsHavingSex = true;

			yield return Main.Ctrler.StartSex(man, girl);

			this.IsHavingSex = false;
			yield break;
		}

		public override void OnClick()
		{
			CommonStates menuNPC = Managers.mn.gameMN.menuNPC;
			if (menuNPC != null && Managers.mn.sexMN.SexCheck(Managers.mn.gameMN.playerCommons[GameManager.selectPlayer], menuNPC))
			{
				float num = 0f;
				for (int i = 0; i < menuNPC.lovers.Count; i++)
				{
					if (menuNPC.lovers[i].friendID == GameManager.selectPlayer)
					{
						num = menuNPC.lovers[i].love;
						break;
					}
				}
				Managers.mn.npcMN.StartCoroutine(
					Managers.mn.npcMN.NPCTalk(menuNPC, NPCManager.TalkType.SexInvite, true)
				);
				if (num <= 50f)
				{
					CommonStates.Debuff debuff = menuNPC.debuff;
					if (debuff == null || debuff.perfume <= 0f)
					{
						menuNPC.nMove.actType = NPCMove.ActType.Idle;
						return;
					}
				}
				Managers.mn.npcMN.StartCoroutine(
					this.Sex(menuNPC, Managers.mn.gameMN.playerCommons[GameManager.selectPlayer])
				);
				return;
			}
		}
	}
}
