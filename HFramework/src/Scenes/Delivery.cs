using System.Collections;
using HFramework.Handlers;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;

namespace HFramework.Scenes
{
	public class Delivery : IScene
	{
		public static readonly string Name = "HF_Delivery";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Idle = "Idle";
			public const string Loop = "Loop";
			public const string Finish = "Finish";
		}

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

		private IEnumerator Perform()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Idle);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Idle);
				yield break;
			}

			Managers.mn.sound.GoVoice(this.Girl.voiceID, "damage", this.Girl.transform.position);
			yield return this.Performer.Perform(ActionType.DeliveryIdle, new PerformModifiers() { Duration = 20f });

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Idle);
			if (!this.CanContinue())
				yield break;
			

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Loop);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Loop);
				yield break;
			}

			Managers.mn.sound.GoVoice(this.Girl.voiceID, "finish", this.Girl.transform.position);
			yield return this.Performer.Perform(ActionType.DeliveryLoop, new PerformModifiers() { Duration = 10f });

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Loop);
			if (!this.CanContinue())
				yield break;


			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
				yield break;
			}

			Managers.mn.sound.GoVoice(this.Girl.voiceID, "faint", this.Girl.transform.position);
			yield return this.Performer.Perform(ActionType.DeliveryEnd);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
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
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Delivery, this.Controller);
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
				yield return new MoveToPlace(this, [this.Girl], this.TargetPosObject.transform.position, this.SexPlace);

			if (!this.CanContinue())
				yield break;

			if (!this.OcuppyPlace())
				yield break;

			girlMove.Rot(this.Girl.transform.position + Vector3.right);
			girlMove.RBState(false);

			this.DisableLiveGirl();

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield break;
			}

			yield return this.Perform();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
			
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
