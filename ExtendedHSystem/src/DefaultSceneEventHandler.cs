using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace ExtendedHSystem
{
	public class DefaultSceneEventHandler : SceneEventHandler
	{
		public DefaultSceneEventHandler() : base("exthsystem_default_handler")
		{
		}

		public override IEnumerable PlayerDefeated()
		{
			Managers.mn.uiMN.MainCanvasView(false);
			yield return Managers.mn.sexMN.StartCoroutine(Managers.mn.sound.GoBGMFade(1));
			GameObject.Find("UIFXPool").transform.Find("ReviveSlider").GetComponent<Slider>().gameObject.SetActive(false);
			yield return new WaitForSeconds(1f);
			Managers.mn.uiMN.SkipView(true);
		}

		public override IEnumerable PlayerRaped(CommonStates player, CommonStates rapist, bool silent)
		{
			Managers.mn.sexMN.SexCountChange(player, rapist, SexManager.SexCountState.Rapes);
			yield return null;
		}

		public override IEnumerable BeforeRespawn()
		{
			Managers.mn.uiMN.SkipView(false);
			yield return Managers.mn.eventMN.FadeOut(1f);
		}

		public override IEnumerable AfterRape(CommonStates victim, CommonStates rapist)
		{
			if (rapist.debuff.discontent == 4)
				rapist.MoralChange(20f, null, NPCManager.MoralCause.None);

			yield return null;
		}

		public override IEnumerable Respawn(CommonStates player, CommonStates other)
		{
			player.life = (int)(player.maxLife * 0.1);
			player.CommonLifeChange(0.0, 0);
			player.faint = (int)(player.maxFaint * 0.2);
			Managers.mn.gameMN.FaintImageChange();
			
			Managers.mn.sexMN.StartCoroutine(Managers.mn.sexMN.ReviveToNearPoint(other.npcID));
			yield return null;
		}
	}
}