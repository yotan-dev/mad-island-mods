using System.Collections;
using System.Collections.Generic;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes
{
	public class Delivery : IScene, IScene2
	{
		public static readonly string Name = "Delivery";

		/// <summary>
		/// Girl delivering
		/// </summary>
		public readonly CommonStates Girl;

		public readonly WorkPlace WorkPlace;

		public readonly SexPlace SexPlace;

		private ISceneController Controller;

		private readonly GameObject TargetPosObject;

		private SexPerformer Performer;

		private SkeletonAnimation Anim;

		public float SuccessRate = 100f;

		private bool Destroyed = false;

		public Delivery(CommonStates girl, WorkPlace workPlace, SexPlace sexPlace)
		{
			this.Girl = girl;
			this.WorkPlace = workPlace;
			this.SexPlace = sexPlace;

			if (this.WorkPlace != null)
			{
				this.TargetPosObject = this.WorkPlace.transform.Find("pos").gameObject;
				this.SuccessRate = 100f;
			}
			else if (this.SexPlace != null)
			{
				this.TargetPosObject = this.SexPlace.transform.Find("pos").gameObject;
				this.SuccessRate = 60f;
			}
			else
			{
				this.TargetPosObject = null;
				this.SuccessRate = 20f;
			}
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			this.Controller.SetScene(this);
		}

		private IEnumerator MoveToPosition()
		{
			float animTime = 30f;
			Managers.mn.sexMN.StartCoroutine(Managers.mn.story.MovePosition(this.Girl.gameObject, this.TargetPosObject.gameObject.transform.position, 2f, "A_walk", true, true, 0.1f, 40f));

			NPCMove girlMove = this.Girl.nMove;
			bool reached = false;
			while (
				animTime > 0f
				&& (girlMove.actType == NPCMove.ActType.Wait || girlMove.actType == NPCMove.ActType.Interval)
				&& !reached
			)
			{
				animTime -= Time.deltaTime;
				if (Vector3.Distance(this.Girl.gameObject.transform.position, this.TargetPosObject.transform.position) <= 0.5f)
					reached = true;
				else if (this.Girl.anim.state.GetCurrent(0).Animation.Name != "A_walk" && girlMove.actType != NPCMove.ActType.Interval)
					this.Girl.anim.state.SetAnimation(0, "A_walk", true);
				yield return null;
			}

			this.Girl.gameObject.transform.position = this.TargetPosObject.gameObject.transform.position;
			if (animTime <= 0f)
			{
				yield return false;
				yield break;
			}

			yield return this.Controller.LoopAnimation(this, this.Girl.anim, "A_idle");
		}

		private IEnumerator StopGirl()
		{
			NPCMove girlMove = this.Girl.nMove;
			if (girlMove.actType == NPCMove.ActType.Wait)
			{
				girlMove.actType = NPCMove.ActType.Idle;
				yield return new WaitForSeconds(0.1f);
			}

			girlMove.actType = NPCMove.ActType.Wait;
			yield return new WaitForSeconds(0.1f);

			if (girlMove.actType != NPCMove.ActType.Wait)
				this.Destroy();
		}

		private void DisableLiveGirl()
		{
			CapsuleCollider aColl = this.Girl.GetComponent<CapsuleCollider>();
			if (aColl != null)
				aColl.enabled = false;

			if (this.TargetPosObject != null)
				this.Girl.gameObject.transform.position = this.TargetPosObject.gameObject.transform.position;

			Managers.mn.randChar.HandItemHide(this.Girl, true);
		}

		private void EnableLiveGirl()
		{
			Managers.mn.randChar.HandItemHide(this.Girl, false);
			CapsuleCollider aColl = this.Girl.GetComponent<CapsuleCollider>();

			if (aColl != null)
				aColl.enabled = true;

			NPCMove girlMove = this.Girl.nMove;
			if (girlMove.actType == NPCMove.ActType.Wait)
			{
				this.Girl.anim.state.SetAnimation(0, "A_idle", true);
				girlMove.actType = NPCMove.ActType.Idle;
			}
		}

		private void GetChildGender(out int gender, out string addText)
		{
			addText = "";

			LayerMask mask = LayerMask.GetMask(["BG"]);
			Collider[] array = Physics.OverlapSphere(this.Girl.gameObject.transform.position, 3f, mask);
			if (array.Length != 0)
			{
				for (int i = 0; i < array.Length; i++)
				{
					CookSupport component = array[i].gameObject.GetComponent<CookSupport>();
					if (component != null)
					{
						switch (component.supportType)
						{
							case CookSupport.SupportType.DeliveryBoy:
								gender = Gender.Male;
								addText = Managers.mn.textMN.logTexts[26];
								return;

							case CookSupport.SupportType.DeliveryGirl:
								gender = Gender.Female;
								addText = Managers.mn.textMN.logTexts[26];
								return;
						}
					}
				}
			}

			gender = Random.Range(0, 2) == 0 ? Gender.Female : Gender.Male;
		}

		private int GetChildNpcId(int gender)
		{
			switch (gender)
			{
				case Gender.Female:
					if (GameInfo.HasDLC)
						return NpcID.NativeGirl;
					else
						return NpcID.FemaleNative;

				case Gender.Male:
					if (GameInfo.HasDLC)
						return NpcID.NativeBoy;
					else if (CommonUtils.GetPregnantFatherId(this.Girl) == NpcID.Man)
						return NpcID.YoungMan;
					else
						return NpcID.MaleNative;

				default:
					PLogger.LogError("GetChildNpcId: Unexpected gender: " + gender);
					return NpcID.MaleNative;
			}
		}

		public IEnumerator SpawnChild()
		{
			this.GetChildGender(out int gender, out string addText);
			int npcId = this.GetChildNpcId(gender);

			yield return Managers.mn.npcMN.GenSpawn(npcId, this.Girl.gameObject, null, 0f, null, true);

			CommonStates tmpGenerate = Managers.mn.npcMN.tmpGenerate;
			Managers.mn.npcMN.tmpGenerate = null;
			if (gender == Gender.Female)
				tmpGenerate.charaName = Managers.mn.randChar.GirlNaming();
			else
				tmpGenerate.charaName = Managers.mn.randChar.ManNaming();

			tmpGenerate.age = 0;
			tmpGenerate.friendID = Managers.mn.npcMN.CreateFriendID();
			Managers.mn.npcMN.NPCtoFriend(tmpGenerate.gameObject, true, false);
			Managers.mn.randChar.GenChildSet(tmpGenerate, this.Girl);
			string log = addText + Managers.mn.textMN.texts[14].Replace("XXXX", this.Girl.charaName).Replace("YYYY", tmpGenerate.charaName);
			Managers.mn.uiMN.GoLogText(log);
		}

		private IEnumerator Perform()
		{
			Managers.mn.sound.GoVoice(this.Girl.voiceID, "damage", this.Girl.transform.position);
			yield return this.Performer.Perform(ActionType.DeliveryIdle, 20f);
			if (!this.CanContinue())
				yield break;
			
			Managers.mn.sound.GoVoice(this.Girl.voiceID, "finish", this.Girl.transform.position);
			yield return this.Performer.Perform(ActionType.DeliveryLoop, 10f);
			if (!this.CanContinue())
				yield break;

			Managers.mn.sound.GoVoice(this.Girl.voiceID, "faint", this.Girl.transform.position);
			yield return this.Performer.Perform(ActionType.DeliveryEnd);
			if (!this.CanContinue())
				yield break;
		}


		private bool OcuppyPlace()
		{
			if (this.WorkPlace != null)
			{
				if (this.WorkPlace.users[0] != null)
					return false;

				this.WorkPlace.users[0] = this.Girl.gameObject;
			}

			if (this.SexPlace != null)
			{
				if (this.SexPlace.user != null)
					return false;

				this.SexPlace.user = this.Girl.gameObject;
			}

			return true;
		}

		private void FreePlace()
		{
			if (this.WorkPlace != null)
				this.WorkPlace.users[0] = null;

			if (this.SexPlace != null)
				this.SexPlace.user = null;
		}

		public IEnumerator Run()
		{
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError("Delivery.Performer is null");
				yield break;
			}

			this.Anim = this.Girl.anim;

			yield return this.StopGirl();
			
			NPCMove girlMove = this.Girl.nMove;
			float tmpSearchAngle = girlMove.searchAngle;

			girlMove.searchAngle = 0f;
			if (this.TargetPosObject != null)
				yield return this.MoveToPosition();

			if (!this.CanContinue())
				yield break;

			if (!this.OcuppyPlace())
				yield break;

			girlMove.Rot(this.Girl.transform.position + Vector3.right);
			girlMove.RBState(false);

			this.DisableLiveGirl();

			yield return this.Perform();
			
			this.FreePlace();

			girlMove.searchAngle = tmpSearchAngle;
			this.EnableLiveGirl();
		}

		public bool CanContinue()
		{
			return this.Girl.nMove.actType == NPCMove.ActType.Wait && !this.Destroyed;
		}

		public void Destroy()
		{
			this.Destroyed = true;
		}

		public string GetName()
		{
			return Delivery.Name;
		}

		public CommonStates[] GetActors()
		{
			return [this.Girl];
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.Anim;
		}

		public string ExpandAnimationName(string originalName)
		{
			return originalName;
		}

		public SexPerformer GetPerformer()
		{
			return this.Performer;
		}
	}
}
