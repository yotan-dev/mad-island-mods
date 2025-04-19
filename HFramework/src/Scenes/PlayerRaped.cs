using System.Collections;
using HFramework.Handlers;
using HFramework.Hook;
using HFramework.ParamContainers;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes
{
	public class PlayerRaped : BaseScene
	{
		public static readonly string Name = "HF_PlayerRaped";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Battle = "Battle";
			public const string Defeat = "Defeat";
			public const string Insert = "Insert";
			public const string Speed1 = "Speed";
			public const string Speed2 = "Speed2";
			public const string Finish = "Finish";
		}

		public readonly CommonStates Player;

		public readonly CommonStates Rapist;

		public bool Skipped { get { return Managers.mn.uiMN.skip; } }

		private NPCMove RapistMove;

		private GameObject TmpSex;

		public PlayerRaped(CommonStates player, CommonStates rapist) : base(Name)
		{
			this.Player = player;
			this.Rapist = rapist;
		}

		private void PrepareNpc(CommonStates npc, out NPCMove npcMove)
		{
			npcMove = npc.GetComponent<NPCMove>();
			npcMove.actType = NPCMove.ActType.Wait;
		}

		private void DisableLiveNpc(CommonStates npc, NPCMove npcMove)
		{
			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (coll != null)
				coll.enabled = false;
			if (mesh != null)
				mesh.enabled = false;

			Managers.mn.randChar.HandItemHide(npc, true);
		}

		private void EnableLiveNpc(CommonStates npc)
		{
			if (npc == null)
				return;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = true;

			Managers.mn.randChar.HandItemHide(npc, false);
		}

		private void DisableLivePlayer(CommonStates player)
		{
			CapsuleCollider toColl = player.GetComponent<CapsuleCollider>();
			MeshRenderer toMesh = player.anim.GetComponent<MeshRenderer>();
			toColl.enabled = false;
			toMesh.enabled = false;

			Managers.mn.randChar.HandItemHide(player, true);
		}

		private void EnableLivePlayer(CommonStates player, bool isRespawn)
		{
			CapsuleCollider toColl = player.GetComponent<CapsuleCollider>();
			toColl.enabled = true;

			// We can't restore the mesh here for respawn or we will have 2 sprites...
			// but we need to do it if the player simply escaped the grapple.
			if (!isRespawn)
			{
				MeshRenderer toMesh = player.anim.GetComponent<MeshRenderer>();
				toMesh.enabled = true;
			}

			Managers.mn.randChar.HandItemHide(player, true);
		}

		private GameObject GetScene(PerformerScope scope)
		{
			GameObject scene = null;
			// For Rape, order of actors matters, and Rapist always comes first, even if they are a girl.
			this.Performer = ScenesManager.Instance.GetPerformer(this, scope, this.Controller, [this.Rapist, this.Player]);
			if (this.Performer != null)
				scene = this.Performer.Info.SexPrefabSelector.GetPrefab();

			return scene;
		}

		private void SetCharacter()
		{
			// Setup TmpSex
			if (this.Player.npcID == NpcID.Yona)
			{
				// npcID - 10 > 1 -- as far as I can tell, only monsters remain
				// Maybe (IsHuman() ?)
				if (this.Rapist.npcID == NpcID.MaleNative || this.Rapist.npcID == NpcID.BigNative)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Rapist);
				}
				else
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, null);
				}
			}
			else
			{ // Man
				if (this.Rapist.npcID == NpcID.Mummy)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadMummy(this.Rapist, this.TmpSex);
				}
				else
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Rapist, this.Player);
				}
			}
		}

		private bool SetupFightScene()
		{
			var scene = this.GetScene(PerformerScope.Battle);
			if (scene == null)
				return false;

			this.TmpSex = Object.Instantiate<GameObject>(scene, this.Player.gameObject.transform.position, Quaternion.identity);

			this.SetCharacter();

			this.PrepareNpc(this.Rapist, out this.RapistMove);
			this.DisableLiveNpc(this.Rapist, this.RapistMove);

			this.DisableLivePlayer(this.Player);

			return true;
		}

		private void SetupSexScene()
		{
			var scene = this.GetScene(PerformerScope.Sex);
			if (scene == null)
				return;

			if (this.TmpSex != null)
				Object.Destroy(this.TmpSex);

			this.TmpSex = Object.Instantiate<GameObject>(scene, this.Player.gameObject.transform.position, Quaternion.identity);

			// This is a made up logic. originally SetupSexScene only happens for Man x Large Female
			this.SetCharacter();
		}

		private IEnumerator PerformGrapple()
		{
			yield return new PlayerGrappled(this, this.CommonAnim, this.Player).Handle();
		}

		private IEnumerator Battle()
		{
			var sexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim = sexAnim;

			yield return this.Performer.Perform(ActionType.Battle);
			yield return this.PerformGrapple();

			if (this.Player.faint > 0f)
				this.Destroy();
		}

		private IEnumerator Defeat()
		{
			yield return this.Performer.Perform(ActionType.Defeat);
			yield return HookManager.Instance.RunEventHook(this, EventNames.OnPlayerDefeated, new FromToParams(this.Rapist, this.Player));
			if (!this.CanContinue())
				yield break;

			if (!this.Skipped)
				yield return this.Controller.WaitForInput();
		}

		private IEnumerator PerformBattle()
		{
			yield return this.RunStep(StepNames.Battle, this.Battle);
			if (!this.CanContinue())
				yield break;

			yield return this.RunStep(StepNames.Defeat, this.Defeat);
		}

		private IEnumerator Insert()
		{
			yield return this.Performer.Perform(ActionType.Insert, new PerformModifiers() { Silent = this.Skipped });
		}

		private IEnumerator Speed1()
		{
			yield return this.Performer.Perform(ActionType.Speed1, new PerformModifiers() { Silent = this.Skipped });
			if (!this.Skipped)
				yield return this.Controller.WaitForInput();
		}

		private IEnumerator Speed2()
		{
			yield return this.Performer.Perform(ActionType.Speed2, new PerformModifiers() { Silent = this.Skipped });
			if (!this.Skipped)
				yield return this.Controller.WaitForInput();
		}

		private IEnumerator Finish()
		{
			yield return this.Performer.Perform(ActionType.Finish, new PerformModifiers() { Silent = this.Skipped });
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.FinishIdle, new PerformModifiers() { Silent = this.Skipped });
			if (!this.Skipped)
				yield return this.Controller.WaitForInput();

		}

		private IEnumerator PerformSex()
		{
			this.SetupSexScene();
			SkeletonAnimation sexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim = sexAnim;

			yield return this.RunStep(StepNames.Insert, this.Insert);
			if (!this.CanContinue())
				yield break;

			yield return this.RunStep(StepNames.Speed1, this.Speed1);
			if (!this.CanContinue())
				yield break;

			yield return this.RunStep(StepNames.Speed2, this.Speed2);
			if (!this.CanContinue())
				yield break;

			yield return this.RunStep(StepNames.Finish, this.Finish);
		}

		private IEnumerator StartRespawn()
		{
			Managers.mn.uiMN.SkipView(false);
			yield return Managers.mn.eventMN.FadeOut(1f);
		}

		private void Respawn()
		{
			this.Player.life = (int)(this.Player.maxLife * 0.1);
			this.Player.CommonLifeChange(0.0, 0);
			this.Player.faint = (int)(this.Player.maxFaint * 0.2);
			Managers.mn.gameMN.FaintImageChange();

			Managers.mn.sexMN.StartCoroutine(Managers.mn.sexMN.ReviveToNearPoint(this.Rapist.npcID));
		}

		public override IEnumerator Run()
		{
			// The original logic is weird, but we need to reset this ourselves or things will go crazy.
			Managers.mn.uiMN.skip = false;

			if (!this.SetupFightScene())
			{
				if (this.TmpSex != null)
					Object.Destroy(this.TmpSex);
				yield break;
			}

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				Object.Destroy(this.TmpSex);
				this.EnableLiveNpc(this.Rapist);
				this.EnableLivePlayer(this.Player, false);
				this.RapistMove.actType = NPCMove.ActType.Interval;
				yield break;
			}

			yield return this.PerformBattle();
			if (!this.CanContinue())
			{
				Object.Destroy(this.TmpSex);
				this.EnableLiveNpc(this.Rapist);
				this.EnableLivePlayer(this.Player, false);
				this.RapistMove.actType = NPCMove.ActType.Interval;
				yield break;
			}

			this.Player.sex = CommonStates.SexState.GameOver;
			yield return this.PerformSex();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);

			yield return this.StartRespawn();

			Object.Destroy(this.TmpSex);

			this.EnableLiveNpc(this.Rapist);

			this.EnableLivePlayer(this.Player, true);

			this.Respawn();

			this.RapistMove.actType = NPCMove.ActType.Interval;
		}

		public override bool CanContinue()
		{
			return !this.Destroyed && this.TmpSex != null;
		}

		public override void Destroy()
		{
			this.Destroyed = true;
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public override CommonStates[] GetActors()
		{
			if (CommonUtils.IsMale(this.Player))
				return [this.Player, this.Rapist];
			else if (CommonUtils.IsMale(this.Rapist))
				return [this.Rapist, this.Player];

			return [this.Player, this.Rapist];
		}

		public override string ExpandAnimationName(string originalName)
		{
			var act = GetActors()[1];
			return originalName.Replace("[Tits]", act.parameters[6].ToString("00"));
		}
	}
}

