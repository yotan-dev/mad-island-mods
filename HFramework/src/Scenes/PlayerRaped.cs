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
	public class PlayerRaped : IScene, IScene2
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

		private NPCMove RapistMove;

		private GameObject TmpSex;

		private ISceneController Controller;

		private SexPerformer Performer;

		private SkeletonAnimation CommonAnim;

		public PlayerRaped(CommonStates player, CommonStates rapist)
		{
			this.Player = player;
			this.Rapist = rapist;
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			this.Controller.SetScene(this);
		}

		public string GetName()
		{
			return PlayerRaped.Name;
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
		}

		private void DisableLivePlayer(CommonStates player)
		{
			CapsuleCollider toColl = player.GetComponent<CapsuleCollider>();
			MeshRenderer toMesh = player.anim.GetComponent<MeshRenderer>();
			toColl.enabled = false;
			toMesh.enabled = false;
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
		}

		private GameObject GetScene(PerformerScope scope)
		{
			GameObject scene = null;
			this.Performer = ScenesManager.Instance.GetPerformer(this, scope, this.Controller);
			if (this.Performer != null)
				scene = this.Performer.Info.SexPrefabSelector.GetPrefab();

			return scene;
		}

		private bool SetupFightScene()
		{
			var scene = this.GetScene(PerformerScope.Battle);
			if (scene == null)
				return false;

			this.TmpSex = Object.Instantiate<GameObject>(scene, this.Player.gameObject.transform.position, Quaternion.identity);

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
					Managers.mn.randChar.LoadMummy(this.Player, this.TmpSex);
				}
				else
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Rapist, this.Player);
				}
			}

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
			if (this.Player.npcID == NpcID.Man)
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Rapist, this.Player);
			else
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Rapist);
		}

		private IEnumerator PerformGrapple()
		{
			yield return new PlayerGrappled(this, this.CommonAnim, this.Player).Handle();
		}

		private IEnumerator PerformBattle()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Battle);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Battle);
				yield break;
			}

			var sexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim = sexAnim;

			yield return this.Performer.Perform(ActionType.Battle);
			yield return this.PerformGrapple();
			if (!this.CanContinue() || this.Player.faint > 0.0)
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Battle);
				yield break;
			}

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Battle);
			if (!this.CanContinue())
				yield break;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Defeat);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Defeat);
				yield break;
			}

			this.Player.sex = CommonStates.SexState.GameOver;

			yield return this.Performer.Perform(ActionType.Defeat);
			yield return HookManager.Instance.RunEventHook(this, EventNames.OnPlayerDefeated, new FromToParams(this.Player, this.Rapist));
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Defeat);
				yield break;
			}

			yield return this.Controller.WaitForInput();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Defeat);
		}

		private IEnumerator PerformSex()
		{
			this.SetupSexScene();
			SkeletonAnimation sexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim = sexAnim;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Insert);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Insert);
			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
			if (!this.CanContinue())
				yield break;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed1);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed1);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Speed1);
			yield return this.Controller.WaitForInput();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed1);
			if (!this.CanContinue())
				yield break;


			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed2);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed2);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Speed2);
			yield return this.Controller.WaitForInput();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed2);
			if (!this.CanContinue())
				yield break;


			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Finish);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.FinishIdle);
			yield return this.Controller.WaitForInput();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
		}

		private IEnumerator FadeOut()
		{
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

		public IEnumerator Run()
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

			yield return this.PerformSex();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);

			Managers.mn.uiMN.SkipView(false);
			yield return this.FadeOut();

			Object.Destroy(this.TmpSex);

			this.EnableLiveNpc(this.Rapist);

			this.EnableLivePlayer(this.Player, true);

			this.Respawn();

			this.RapistMove.actType = NPCMove.ActType.Interval;
		}

		public bool CanContinue()
		{
			return this.TmpSex != null && !Managers.mn.uiMN.skip;
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public CommonStates[] GetActors()
		{
			if (CommonUtils.IsMale(this.Player))
				return [this.Player, this.Rapist];
			else if (CommonUtils.IsMale(this.Rapist))
				return [this.Rapist, this.Player];

			return [this.Player, this.Rapist];
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.CommonAnim;
		}

		public string ExpandAnimationName(string originalName)
		{
			var act = GetActors()[1];
			return originalName.Replace("<Tits>", act.parameters[6].ToString("00"));
		}

		public SexPerformer GetPerformer()
		{
			return this.Performer;
		}
	}
}

