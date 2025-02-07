using System.Collections;
using System.Collections.Generic;
using HFramework.Performer;
using HFramework.Scenes;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;

namespace HFramework.Handlers
{
	/// <summary>
	/// Sets animation "name" to loop indefinetely in "animation"
	/// </summary>
	public class ManPlayerGrapples : BaseHandler
	{
		private readonly SkeletonAnimation Anim;

		private readonly CommonStates Player;

		private readonly CommonStates Girl;

		private string LoopAnimationName;

		private string AttackAnimationName;

		public ManPlayerGrapples(IScene scene, SkeletonAnimation animation, CommonStates player, CommonStates girl) : base(scene)
		{
			this.Anim = animation;
			this.Player = player;
			this.Girl = girl;
		}

		private void GetBattleStatus(CommonStates man, CommonStates girl, out float limitTime, out float autoTime, out float tmpDamage)
		{
			float num = 1;
			switch (Managers.mn.skillMN.skills[9])
			{
				case 0:
					num = 1f;
					tmpDamage = 1f;
					autoTime = 100f;
					break;

				case 1:
					tmpDamage = 2f;
					autoTime = 0.5f;
					break;

				case 2:
					num = 1.5f;
					tmpDamage = 2f;
					autoTime = 0.4f;
					break;

				case 3:
					num = 1.5f;
					tmpDamage = 3f;
					autoTime = 0.3f;
					break;

				case 4:
					num = 2f;
					tmpDamage = 3f;
					autoTime = 0.2f;
					break;

				default:
					num = 2f;
					tmpDamage = (float)(4 + Managers.mn.skillMN.skills[9] - 5);
					autoTime = 0.1f;
					break;
			}

			float num2 = 10f * (float)Mathf.Clamp(man.level - girl.level, 0, 10) / 10f * num;
			limitTime = 5f + num2;
		}

		private void SetMix(SexPerformer performer)
		{
			var loopAction = performer.CurrentSet.Actions.GetValueOrDefault(new ActionKey(ActionType.Battle, performer.CurrentPose), null);
			var attackAction = performer.CurrentSet.Actions.GetValueOrDefault(new ActionKey(ActionType.Attack, performer.CurrentPose), null);

			if (loopAction?.AnimationName != null)
				this.LoopAnimationName = this.Scene.ExpandAnimationName(loopAction.AnimationName);

			if (attackAction?.AnimationName != null)
				this.AttackAnimationName = this.Scene.ExpandAnimationName(attackAction.AnimationName);

			if (loopAction?.AnimationName != null && attackAction?.AnimationName != null)
			{
				this.Anim.state.Data.SetMix(
					this.LoopAnimationName,
					this.AttackAnimationName,
					0f
				);
			}
		}

		protected override IEnumerator Run()
		{
			this.GetBattleStatus(this.Player, this.Girl, out var limitTime, out var autoTime, out var tmpDamage);

			float tmpAutoTime = autoTime;
			float animTime = limitTime;
			Image breakTime = GameObject.Find("UIFXPool").transform.Find("TimeCircleBack/TimeCircle").GetComponent<Image>();
			breakTime.gameObject.transform.parent.transform.position = this.Player.transform.position + Vector3.up * 2f;
			breakTime.gameObject.transform.parent.gameObject.SetActive(true);
			breakTime.fillAmount = 1f;

			var performer = this.Scene.GetPerformer();
			// string this.AttackAnimationName = sexType + "Attack_attack";
			this.SetMix(performer);

			while (animTime > 0f && this.Girl.faint > 0.0 && this.Scene.CanContinue())
			{
				bool shouldAttack = false;
				animTime -= Time.deltaTime;
				breakTime.fillAmount = animTime / limitTime;
				if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
					tmpAutoTime -= Time.deltaTime;

				if (tmpAutoTime <= 0f)
				{
					shouldAttack = true;
					tmpAutoTime = autoTime;
					this.Girl.StunDamage(this.Player, tmpDamage, true);
				}
				
				// Unfortunately we simply can't use Perfomer.PerformBg here, yielding will make the animation stutter...
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
				{
					shouldAttack = true;
					this.Girl.StunDamage(this.Player, tmpDamage, true);
				}
				else if (this.Anim.skeleton.Data.FindAnimation(this.AttackAnimationName) != null && this.Anim.AnimationName == this.AttackAnimationName && this.Anim.AnimationState.GetCurrent(0).TrackTime >= this.Anim.state.GetCurrent(0).AnimationEnd)
				{
					this.Anim.state.SetAnimation(0, this.LoopAnimationName, true);
				}


				if (this.Anim.skeleton.Data.FindAnimation(this.AttackAnimationName) != null && this.Anim.AnimationName != this.AttackAnimationName && shouldAttack)
				{
					this.Anim.state.SetAnimation(0, this.AttackAnimationName, false);
				}

				yield return null;
			}

			breakTime.gameObject.transform.parent.gameObject.SetActive(false);

			if (animTime <= 0f || this.Player.life <= 0.0 || Input.GetKeyDown(KeyCode.R))
				this.ShouldStop = true;
		}
	}
}
