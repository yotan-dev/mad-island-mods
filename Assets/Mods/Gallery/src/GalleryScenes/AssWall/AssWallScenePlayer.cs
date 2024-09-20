using System.Collections;
using YotanModCore;
using Gallery.Patches;
using Spine.Unity;
using UnityEngine;

namespace Gallery.GalleryScenes.AssWall
{
	public class AssWallScenePlayer
	{
		public SkeletonAnimation tmpCommonAnim;

		private CommonStates Player;

		private CommonStates Girl;

		private InventorySlot WallSlot;

		public AssWallScenePlayer(CommonStates player, CommonStates girl, InventorySlot tmpWall) {
			this.Player = player;
			this.Girl = girl;
			this.WallSlot = tmpWall;
			PropPanelPatch.OnPropAction += PropAction;
		}

		~AssWallScenePlayer() {
			PropPanelPatch.OnPropAction -= PropAction;
		}

		private void PropAction(int propActProgress, int id, int state)
		{
			if (propActProgress != 1)
				return;

			Managers.mn.StartCoroutine(this.Play(state));
		}

		private IEnumerator SceneMain()
		{
			CommonStates pCommon;
			CommonStates girlCommon;
			SexPlace sexPlace;
			
			Managers.uiManager.UIManager.PropPanelOpen(1, Managers.mn.sexMN.transform.position);

			pCommon = this.Player;
			girlCommon = this.Girl;
			this.Girl = girlCommon;
			sexPlace = this.WallSlot.GetComponent<SexPlace>();
			sexPlace.user = Managers.mn.gameMN.player;
			
			this.tmpCommonAnim = Managers.mn.inventory.tmpSubInventory.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.tmpCommonAnim.skeleton.SetSkin("Man");
			this.tmpCommonAnim.skeleton.SetSlotsToSetupPose();
			Managers.mn.randChar.SetCharacter(Managers.mn.inventory.tmpSubInventory.gameObject, null, pCommon);
			Managers.mn.randChar.SetAssWall(girlCommon, this.tmpCommonAnim.gameObject);
			this.tmpCommonAnim.state.SetAnimation(0, "A_idle", true);
			Managers.mn.uiMN.PropPanelStateChange(0, 2, 4, true);
			Managers.mn.uiMN.PropPanelStateChange(1, 3, 3, true);

			while (Managers.mn.uiMN.propActProgress == 1) {
				yield return null;
			}
			
			this.tmpCommonAnim.skeleton.SetSkin("default");
			this.tmpCommonAnim.skeleton.SetSlotsToSetupPose();

			Managers.uiManager.UIManager.PropPanelHideAll();
			
			sexPlace.user = null;
			this.Girl = null;
			PropPanelPatch.OnPropAction -= PropAction;
		}

		private void Insert()
		{
			string animSt = this.Girl.npcID.ToString("") + "_";
			if (this.tmpCommonAnim.state.GetCurrent(0).Animation.Name != animSt + "A_Loop_01" && this.tmpCommonAnim.state.GetCurrent(0).Animation.Name != animSt + "A_Loop_02")
			{
				Managers.mn.uiMN.PropPanelStateChange(0, 7, 4, true);
				Managers.mn.uiMN.PropPanelStateChange(1, 8, 5, true);
				Managers.mn.uiMN.PropPanelStateChange(2, 6, 6, true);
				Managers.mn.uiMN.PropPanelStateChange(3, 3, 3, true);
				this.tmpCommonAnim.state.SetAnimation(0, animSt + "A_Loop_01", true);
			}
			else
			{
				Managers.mn.uiMN.PropPanelStateChange(0, 5, 4, true);
				Managers.mn.uiMN.PropPanelStateChange(1, 3, 3, true);
				Managers.mn.uiMN.PropPanelStateChange(2, 0, 0, false);
				Managers.mn.uiMN.PropPanelStateChange(3, 0, 0, false);
				this.tmpCommonAnim.state.SetAnimation(0, "A_idle", true);
			}
		}

		private void Speed()
		{
			string animSt = this.Girl.npcID.ToString("") + "_";
			if (this.tmpCommonAnim.state.GetCurrent(0).Animation.Name == animSt + "A_Loop_01")
			{
				this.tmpCommonAnim.state.SetAnimation(0, animSt + "A_Loop_02", true);
			}
			else if (this.tmpCommonAnim.state.GetCurrent(0).Animation.Name == animSt + "A_Loop_02")
			{
				this.tmpCommonAnim.state.SetAnimation(0, animSt + "A_Loop_01", true);
			}
		}

		private IEnumerator Bust()
		{
			string animSt = this.Girl.npcID.ToString("") + "_";
			Managers.mn.uiMN.propPanel.SetActive(false);
			this.tmpCommonAnim.state.SetAnimation(0, animSt + "A_Finish", false);
			float animTime = this.tmpCommonAnim.state.GetCurrent(0).AnimationEnd;
			while (animTime > 0f && Managers.mn.uiMN.propActProgress == 1)
			{
				animTime -= Time.deltaTime;
				yield return null;
			}
			if (Managers.mn.uiMN.propActProgress == 1)
			{
				this.tmpCommonAnim.state.SetAnimation(0, animSt + "A_Finish_idle", true);
				Managers.mn.uiMN.PropPanelStateChange(0, 2, 4, true);
				Managers.mn.uiMN.PropPanelStateChange(1, 3, 3, true);
				Managers.mn.uiMN.PropPanelStateChange(3, 0, 0, false);
				Managers.mn.uiMN.propPanel.SetActive(true);
			}
		}

		// public IEnumerator Play(CommonStates man, CommonStates girl)
		public IEnumerator Play(int state)
		{
			if (state != 0 && this.tmpCommonAnim == null) {
				yield break;
			}

			switch (state)
			{
				case 0: // Start
					yield return this.SceneMain();
					break;
				case 3:
					Managers.mn.uiMN.propActProgress = -1;
					break;
				case 4:
					this.Insert();
					break;
				case 5:
					this.Speed();
					break;
				case 6:
					yield return this.Bust();
					break;
			}
			yield break;
		}
	}
}