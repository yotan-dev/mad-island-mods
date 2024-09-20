using System.Collections;
using YotanModCore;
using Spine.Unity;
using UnityEngine;
using YotanModCore.Consts;

namespace Gallery.GalleryScenes.ManRapes
{
	public class ManRapesScenePlayer
	{
		public IEnumerator Play(CommonStates man, CommonStates girl)
		{
			GameObject tmpSex = null;
			var tmpSexType = "A_";
			var tmpSexCountType = 0;
			int girlNpcID = girl.npcID;

			// Environment Setup
			if (girlNpcID <= NpcID.Mummy)
			{
				switch (girlNpcID)
				{
					case NpcID.FemaleNative:
						if (girl.faint <= 0f)
						{
							tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[21], man.gameObject.transform.position, Quaternion.identity);
						}
						else
						{
							if (girl.dissect[4] == 1 && girl.dissect[5] == 1)
							{
								tmpSexType = "DisLeg_A_";
							}
							tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[0], man.gameObject.transform.position, Quaternion.identity);
						}
						Managers.mn.randChar.SetCharacter(tmpSex, girl, man);
						break;
					case NpcID.NativeGirl:
						if (girl.faint <= 0f)
						{
							tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[22], man.gameObject.transform.position, Quaternion.identity);
						}
						else
						{
							tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[2], man.gameObject.transform.position, Quaternion.identity);
						}
						Managers.mn.randChar.SetCharacter(tmpSex, girl, man);
						break;
					case NpcID.FemaleLargeNative:
						tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[15], man.gameObject.transform.position, Quaternion.identity);
						Managers.mn.randChar.SetCharacter(tmpSex, girl, man);
						break;
					case NpcID.OldManNative:
						break;
					case NpcID.OldWomanNative:
						tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[10], man.gameObject.transform.position, Quaternion.identity);
						Managers.mn.randChar.SetCharacter(tmpSex, null, man);
						break;
					default:
						if (girlNpcID == NpcID.Mummy)
						{
							tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[6], man.gameObject.transform.position, Quaternion.identity);
							Managers.mn.randChar.SetCharacter(tmpSex, null, man);
							Managers.mn.randChar.LoadMummy(girl, tmpSex);
							tmpSexType = "Rape_A_";
							tmpSexCountType = 1;
						}
						break;
				}
			}
			else if (girlNpcID != NpcID.UnderGroundWoman)
			{
				if (girlNpcID != NpcID.ElderSisterNative)
				{
					if (girlNpcID == NpcID.Shino)
					{
						tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[19], man.gameObject.transform.position, Quaternion.identity);
						Managers.mn.randChar.SetCharacter(tmpSex, girl, man);
						tmpSexType = "Rape_A_";
					}
				}
				else
				{
					tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[18], man.gameObject.transform.position, Quaternion.identity);
					Managers.mn.randChar.SetCharacter(tmpSex, girl, man);
				}
			}
			else
			{
				tmpSex = GameObject.Instantiate<GameObject>(Managers.mn.sexMN.sexList[1].sexObj[8], man.gameObject.transform.position, Quaternion.identity);
				Managers.mn.randChar.SetCharacter(tmpSex, null, man);
				Managers.mn.randChar.LoadGenUnder(girl, tmpSex);
			}

			if (tmpSex == null)
			{
				yield break;
			}

			SkeletonAnimation sexAnim = tmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			GameObject tmpSexScale = null;
			if (man.pMove.scale.transform.localScale.x == -1f)
			{
				tmpSexScale = tmpSex.transform.Find("Scale").gameObject;
				tmpSexScale.transform.localScale = new Vector3(-1f, 1f, 1f);
			}
			NPCMove nMove = girl.GetComponent<NPCMove>();
			if (nMove.actType != NPCMove.ActType.Dead)
			{
				nMove.actType = NPCMove.ActType.Wait;
			}
			nMove.RBState(false);
			CapsuleCollider girlColl = girl.GetComponent<CapsuleCollider>();
			MeshRenderer girlMesh = girl.anim.GetComponent<MeshRenderer>();
			if (girlColl != null)
			{
				girlColl.enabled = false;
			}
			girlMesh.enabled = false;
			man.GetComponent<PlayerMove>();
			MeshRenderer manMesh = man.anim.GetComponent<MeshRenderer>();
			manMesh.enabled = false;
			Managers.mn.randChar.HandItemHide(girl, true);
			Managers.mn.randChar.HandItemHide(man, true);
			string text = tmpSexType + "Attack_loop";
			if (sexAnim.skeleton.Data.FindAnimation(text) != null)
			{
				sexAnim.state.SetAnimation(0, text, true);
				sexAnim.state.Data.SetMix(text, tmpSexType + "Attack_attack", 0f);
			}

			// =======================
			float limitTime = 5f;
			float animTime = limitTime;
			bool failed = false;
			// Image breakTime = GameObject.Find("UIFXPool").transform.Find("TimeCircleBack/TimeCircle").GetComponent<Image>();
			// breakTime.gameObject.transform.parent.transform.position = man.transform.position + Vector3.up * 2f;
			// breakTime.gameObject.transform.parent.gameObject.SetActive(true);
			// breakTime.fillAmount = 1f;
			string attackAnimName = tmpSexType + "Attack_attack";
			while (tmpSex != null && !Input.GetMouseButtonDown(0))
			{
				bool flag = false;
				if (Input.GetKeyDown(KeyCode.Space))
				{
					flag = true;
				}

				if (sexAnim.skeleton.Data.FindAnimation(attackAnimName) != null && sexAnim.AnimationName == attackAnimName && sexAnim.AnimationState.GetCurrent(0).TrackTime >= sexAnim.state.GetCurrent(0).AnimationEnd)
				{
					sexAnim.state.SetAnimation(0, tmpSexType + "Attack_loop", true);
				}
				if (sexAnim.skeleton.Data.FindAnimation(attackAnimName) != null && sexAnim.AnimationName != attackAnimName && flag)
				{
					sexAnim.state.SetAnimation(0, attackAnimName, false);
				}
				yield return null;
			}

			if (sexAnim.skeleton.Data.FindAnimation(tmpSexType + "Attack_giveup") != null)
			{
				sexAnim.state.SetAnimation(0, tmpSexType + "Attack_giveup", true);
			}

			yield return new WaitForSeconds(0.5f);
			yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

			sexAnim.state.SetAnimation(0, tmpSexType + "AttackToSex", false);
			animTime = sexAnim.state.GetCurrent(0).Animation.Duration;
			while (animTime > 0f)
			{
				animTime -= Time.deltaTime;
				yield return null;
			}

			sexAnim.state.SetAnimation(0, tmpSexType + "Loop_01", true);
			yield return null;
			yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

			sexAnim.state.SetAnimation(0, tmpSexType + "Loop_02", true);
			yield return null;
			yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

			sexAnim.state.SetAnimation(0, tmpSexType + "Finish", false);
			yield return null;

			animTime = sexAnim.state.GetCurrent(0).Animation.Duration;
			while (animTime > 0f && !Input.GetMouseButtonDown(0))
			{
				animTime -= Time.deltaTime;
				yield return null;
			}

			sexAnim.state.SetAnimation(0, tmpSexType + "Finish_idle", true);
			yield return null;
			yield return new WaitUntil(() => Input.GetMouseButtonDown(0));

			GameObject.Destroy(tmpSex);
			yield break;
		}
	}
}