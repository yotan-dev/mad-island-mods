using System.Collections;
using YotanModCore;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Consts;

namespace Gallery.UI.SceneController
{
	public class ManRapesSleepScene
	{
		public static ManRapesSleepScene Instance { get; private set; } = new ManRapesSleepScene();

		public static ManRapesSleepScene Create()
		{
			Instance = new ManRapesSleepScene();
			return Instance;
		}

		private int tmpCommonState = 0;

		private GameObject tmpCommonSex = null;
		private CommonStates tmpCommonGirl;
		private SkeletonAnimation tmpCommonAnim;

		private IEnumerator SceneStart(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			Vector3 pos = girl.transform.position;
			int npcID = girl.npcID;

			// Create Scene
			switch (npcID)
			{
				case NpcID.FemaleNative:
					this.tmpCommonSex = Object.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[4], pos, Quaternion.identity);
					break;

				case NpcID.NativeGirl:
					this.tmpCommonSex = Object.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[5], pos, Quaternion.identity);
					break;

				default:
					yield break;
			}

			// Setup actors
			this.tmpCommonGirl = girl;
			Managers.mn.randChar.SetCharacter(this.tmpCommonSex, girl, man);
			// nMove = girl.GetComponent<NPCMove>();
			// nMove.actType = NPCMove.ActType.Wait;
			// nMove.movable = false;
			// nMove.RBState(false);

			// clear states
			this.tmpCommonState = 0;

			// Setup interaction UI
			Managers.mn.uiMN.PropPanelOpen(PropPanelConst.Type.SleepRapeGallery, pos);

			// Setup animation
			this.tmpCommonAnim = this.tmpCommonSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.tmpCommonAnim.state.SetAnimation(0, "A_idle", true);

			// Sex Loop
			yield return new WaitUntil(() => this.tmpCommonSex == null);

			Managers.mn.uiMN.propPanel.SetActive(false);
			Managers.mn.uiMN.MainCanvasView(true);
		}

		private IEnumerator StartRape(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			this.tmpCommonState = ManRapeSleepConst.SubState.Raping;

			// Setup animations + new menu options
			this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_idle", true);
			if (this.tmpCommonGirl != null)
			{
				Managers.mn.sound.GoVoice(this.tmpCommonGirl.voiceID, "close", this.tmpCommonAnim.transform.position);
			}
			Managers.mn.uiMN.PropPanelHideAll();
			Managers.mn.uiMN.PropPanelStateChange(0, PropPanelConst.Text.Insert, ManRapeSleepConst.Actions.Insert, true); // Insert
			Managers.mn.uiMN.PropPanelStateChange(1, PropPanelConst.Text.Leave, ManRapeSleepConst.Actions.Leave, true); // Leave

			yield break;
		}

		private IEnumerator StartDiscretlyRape(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			if (sexType == 0) // Sleep powder
			{
				this.tmpCommonState = ManRapeSleepConst.SubState.MaybeSleepPowder; // ??
			}
			else
			{
				this.tmpCommonState = ManRapeSleepConst.SubState.DiscretlyRaping; // Discretly rape
			}

			// Setup animation + new menu options
			this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_idle", true);
			Managers.mn.uiMN.PropPanelHideAll();
			Managers.mn.uiMN.PropPanelStateChange(0, PropPanelConst.Text.Insert, ManRapeSleepConst.Actions.Insert, true); // Insert
			Managers.mn.uiMN.PropPanelStateChange(1, PropPanelConst.Text.Leave, ManRapeSleepConst.Actions.Leave, true); // Leave

			yield break;
		}

		private IEnumerator Insert(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			// Disable options
			Managers.mn.uiMN.propPanel.SetActive(false);

			// Player insert animation
			if (this.tmpCommonState == ManRapeSleepConst.SubState.Raping)
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_start", false);
			}
			else
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_start", false);
			}
			float animTime = this.tmpCommonAnim.state.GetCurrent(0).AnimationEnd;
			while (animTime >= 0f)
			{
				animTime -= Time.deltaTime;
				yield return null;
			}

			// Setup fucking animation
			if (this.tmpCommonState == ManRapeSleepConst.SubState.Raping)
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_loop_01", true);
			}
			else
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_loop_01", true);
			}

			// Setup new menu options
			Managers.mn.uiMN.propPanel.SetActive(true);
			Managers.mn.uiMN.PropPanelHideAll();
			Managers.mn.uiMN.PropPanelStateChange(0, PropPanelConst.Text.Speed, ManRapeSleepConst.Actions.ChangeSpeed, true); // speed
			if (this.tmpCommonState == ManRapeSleepConst.SubState.MaybeSleepPowder)
			{
				Managers.mn.uiMN.PropPanelStateChange(1, PropPanelConst.Text.UseSleepPowder, ManRapeSleepConst.Actions.FuckSleepPowder, true);
			}
			Managers.mn.uiMN.PropPanelStateChange(2, PropPanelConst.Text.Finish, ManRapeSleepConst.Actions.Bust, true); // Bust
			Managers.mn.uiMN.PropPanelStateChange(3, PropPanelConst.Text.Leave, ManRapeSleepConst.Actions.Leave, true); // Leave
		}

		private IEnumerator ChangeSpeed(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			if (this.tmpCommonState == ManRapeSleepConst.SubState.Raping)
			{
				if (this.tmpCommonAnim.state.GetCurrent(0).Animation.Name != "A_rapes_loop_02")
				{
					this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_loop_02", true);
				}
				else
				{
					this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_loop_01", true);
				}
			}
			else if (this.tmpCommonAnim.state.GetCurrent(0).Animation.Name != "A_sleep_loop_02")
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_loop_02", true);
			}
			else
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_loop_01", true);
			}

			yield break;
		}

		private IEnumerator Bust(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			// Disable menu
			Managers.mn.uiMN.propPanel.SetActive(false);

			// Play bust animation
			if (this.tmpCommonState == ManRapeSleepConst.SubState.Raping)
			{
				this.tmpCommonGirl.faint = 0f;
				this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_finish", false);
			}
			else
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_finish", false);
			}
			float animTime = this.tmpCommonAnim.state.GetCurrent(0).AnimationEnd;
			yield return new WaitForSeconds(animTime);

			// Enter idle animation
			if (this.tmpCommonState == ManRapeSleepConst.SubState.Raping)
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_rapes_finish_idle", true);
			}
			else
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_finish_idle", true);
			}

			// Setup new menu options
			Managers.mn.uiMN.propPanel.SetActive(true);
			Managers.mn.uiMN.PropPanelHideAll();
			Managers.mn.uiMN.PropPanelStateChange(0, PropPanelConst.Text.Insert, ManRapeSleepConst.Actions.Insert, true);
			Managers.mn.uiMN.PropPanelStateChange(1, PropPanelConst.Text.Leave, ManRapeSleepConst.Actions.Leave, true);
		}

		private IEnumerator SleepPowderSex(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			if (this.tmpCommonAnim.state.GetCurrent(0).Animation.Name == "A_sleep_loop_03")
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_loop_01", true);
			}
			else
			{
				this.tmpCommonAnim.state.SetAnimation(0, "A_sleep_loop_03", true);
			}
			yield break;
		}


		/// <summary>
		/// [man] rapes [girl] that is sleeping.
		/// state vs sexType:
		/// 	state 1 - agressive rape
		/// 		sexType - always 0
		/// 	state 2 - discret rape
		/// 		sexType - 0 = sleep powder
		/// 				- 1 = just acting
		/// </summary>
		/// <param name="state"></param>
		/// <param name="girl"></param>
		/// <param name="man"></param>
		/// <param name="sexType"></param>
		/// <returns></returns>
		public IEnumerator ManRapesSleep(int state, CommonStates girl, CommonStates man, int sexType = 0)
		{
			switch (state)
			{
				case ManRapeSleepConst.Start: // Start
					yield return this.SceneStart(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.StartRape: // Rape
					yield return this.StartRape(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.StartDiscretlyRape: // Discretly rape
					yield return this.StartDiscretlyRape(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.Insert: // Insert
					yield return this.Insert(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.ChangeSpeed: // Speed
					yield return this.ChangeSpeed(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.Bust: // Bust
					yield return this.Bust(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.SleepPowderSex:
					yield return this.SleepPowderSex(state, girl, man, sexType);
					break;

				case ManRapeSleepConst.Leave:
					Object.Destroy(this.tmpCommonSex);
					break;
			}
			yield break;
		}

	}
}