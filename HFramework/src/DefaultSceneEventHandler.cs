using System.Collections;
using HFramework.Scenes;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace HFramework
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

		public override IEnumerable OnNormalSex(CommonStates a, CommonStates b)
		{
			Managers.mn.sexMN.SexCountChange(a, b, SexManager.SexCountState.Normal);
			yield return null;
		}

		public override IEnumerable OnRape(IScene scene, CommonStates from, CommonStates to)
		{
			Managers.mn.sexMN.SexCountChange(to, from, SexManager.SexCountState.Rapes);
			if (scene is ManRapes manRapes) {
				float changeRate = manRapes.LoveChange ? -10f : 0f;
				to.LoveChange(from, changeRate, false);
			}
			yield return null;
		}

		public override IEnumerable OnToilet(CommonStates from, CommonStates to)
		{
			Managers.mn.sexMN.SexCountChange(from, to, SexManager.SexCountState.Toilet);
			yield return null;
		}

		/// <summary>
		/// Called when a creampie is performed.
		/// </summary>
		/// <param name="from">The character who came</param>
		/// <param name="to">The character being creampied</param>
		/// <returns></returns>
		public override IEnumerable OnCreampie(CommonStates from, CommonStates to)
		{
			Managers.mn.sexMN.SexCountChange(to, from, SexManager.SexCountState.Creampie);
			yield return null;
		}

		public override IEnumerable OnDelivery(Delivery scene, CommonStates mother)
		{
			foreach (var x in scene.SpawnChild())
				yield return x;

			Managers.mn.sexMN.SexCountChange(mother, null, SexManager.SexCountState.Delivery);
			mother.age++;
		}

		public override IEnumerable AfterSex(IScene scene, CommonStates from, CommonStates to)
		{
			// if (scene is CommonSexPlayer commonSexPlayer)
			// {
			// 	if (SexMeter.Instance.FillAmount == 1f)
			// 		from.LoveChange(to, 10f, false);
			// 	else if (SexMeter.Instance.FillAmount < 0.3f)
			// 		from.LoveChange(to, -5f, false);
			// }
			// else if (scene is CommonSexNPC)
			// {
			// 	from.LoveChange(to, 10f, false);
			// 	to.LoveChange(from, 10f, false);
			// }

			yield return null;
		}
	}
}
