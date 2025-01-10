using System.Collections;
using HFramework.Handlers;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes
{
	public class ManRapes : IScene
	{
		public static readonly string Name = "HF_ManRapes";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Battle = "Battle";
			public const string Giveup = "Giveup";
			public const string Insert = "Insert";
			public const string Speed1 = "Speed1";
			public const string Speed2 = "Speed2";
			public const string Finish = "Finish";
		}

		public readonly CommonStates Man;

		public readonly CommonStates Girl;

		private NPCMove GirlMove;

		private GameObject TmpSex;

		private SkeletonAnimation TmpSexAnim;

		private GameObject TmpSexScale;

		private ISceneController Controller;

		private SexPerformer Performer;

		private int Stage = 0;

		private bool Aborted = false;

		public ManRapes(CommonStates girl, CommonStates man)
		{
			this.Man = man;
			this.Girl = girl;
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			this.Controller.SetScene(this);
		}

		private void PrepareGirl(CommonStates npc, out NPCMove npcMove)
		{
			npcMove = npc.GetComponent<NPCMove>();
			if (npcMove.actType != NPCMove.ActType.Dead)
				npcMove.actType = NPCMove.ActType.Wait;
		}

		private void DisableLiveGirl(CommonStates npc, NPCMove npcMove)
		{
			npcMove.RBState(false);

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = false;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = false;

			Managers.mn.randChar.HandItemHide(npc, true);
		}

		private IEnumerator EnableLiveGirl(CommonStates npc)
		{
			if (npc == null)
				yield break;

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = true;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			if (npc.faint > 0 && npc.life > 0)
				npc.nMove.actType = NPCMove.ActType.Interval;
			else
				yield return this.Controller.LoopAnimation("A_dead_idle");

			if (npc.nMove.npcType == NPCMove.NPCType.Follow || npc.nMove.npcType == NPCMove.NPCType.Friend)
				npc.nMove.tmpEnemy = null;

			Managers.mn.randChar.HandItemHide(npc, false);
		}

		private void DisableLiveMan(CommonStates player)
		{
			MeshRenderer toMesh = player.anim.GetComponent<MeshRenderer>();
			toMesh.enabled = false;

			Managers.mn.randChar.HandItemHide(player, true);
		}

		private void EnableLiveMan(CommonStates player)
		{
			MeshRenderer toMesh = player.anim.GetComponent<MeshRenderer>();
			toMesh.enabled = true;
			Managers.mn.randChar.HandItemHide(player, false);
		}

		private bool SetupScene()
		{
			var scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
			if (scene == null)
				return false;

			this.TmpSex = Object.Instantiate<GameObject>(scene, this.Man.gameObject.transform.position, Quaternion.identity);
			if (this.TmpSex == null)
				return false;

			// Setup TmpSex
			if (this.Girl.npcID == NpcID.Mummy)
			{
				Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Man);
				Managers.mn.randChar.LoadMummy(this.Girl, this.TmpSex);
			}
			else if (this.Girl.npcID == NpcID.UnderGroundWoman)
			{
				Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Man);
				Managers.mn.randChar.LoadGenUnder(this.Girl, this.TmpSex);
			}
			else if (this.Girl.npcID != NpcID.OldManNative)
			{
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Girl, this.Man);
			}

			this.TmpSexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

			if (this.Girl.npcID == NpcID.Yona && string.IsNullOrEmpty(this.Girl.equip[3].itemKey) && this.TmpSexAnim.skeleton.FindSlot("Panty") != null)
				this.TmpSexAnim.skeleton.SetAttachment("Panty", null);

			this.TmpSexScale = null;
			if (this.Man.pMove.scale.transform.localScale.x == -1f)
			{
				this.TmpSexScale = this.TmpSex.transform.Find("Scale").gameObject;
				this.TmpSexScale.transform.localScale = new Vector3(-1f, 1f, 1f);
			}

			this.PrepareGirl(this.Girl, out this.GirlMove);
			this.DisableLiveGirl(this.Girl, this.GirlMove);
			this.DisableLiveMan(this.Man);

			return true;
		}

		private IEnumerator PerformGrapple()
		{
			yield return new ManPlayerGrapples(this, this.TmpSexAnim, this.Man, this.Girl).Handle();
		}

		private IEnumerator PerformBattle()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Battle);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Battle);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Battle);
			// string text = this.SexType + "Attack_loop";
			// if (this.TmpSexAnim.skeleton.Data.FindAnimation(text) != null)
			// {
			// 	this.TmpSexAnim.state.SetAnimation(0, text, true);
			// 	this.TmpSexAnim.state.Data.SetMix(text, this.SexType + "Attack_attack", 0f);
			// }

			yield return this.PerformGrapple();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Battle);
		}

		private IEnumerator PerformGiveup()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Giveup);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Giveup);
				yield break;
			}

			yield return this.Performer.PerformBg(ActionType.Defeat);
			yield return new WaitForSeconds(0.5f);

			Managers.mn.uiMN.ControlTextActive(true, Managers.mn.textMN.texts[24] + "/n" + Managers.mn.textMN.texts[25]);

			yield return this.Controller.WaitForInput();

			Managers.mn.uiMN.ControlTextActive(false, "");

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Giveup);
		}

		private IEnumerator PerformRape()
		{
			this.Stage = 3;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Insert);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
				yield break;

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);

			this.Stage = 4;

			if (!this.CanContinue())
				yield break;

			this.Stage = 5;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed1);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed1);
				yield break;
			}

			yield return this.Performer.PerformBg(ActionType.Speed1);
			yield return this.Controller.WaitForInput();
			if (!this.CanContinue())
				yield break;

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed1);
			
			this.Stage = 6;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed2);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed2);
				yield break;
			}

			yield return this.Performer.PerformBg(ActionType.Speed2);
			yield return this.Controller.WaitForInput();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed2);

			if (!this.CanContinue())
				yield break;

			this.Stage = 7;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Finish);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
				yield break;
			}

			this.Stage = 8;

			yield return this.Performer.PerformBg(ActionType.FinishIdle);
			yield return this.Controller.WaitForInput();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
		}

		private IEnumerator Teardown()
		{
			Vector3 rot = this.TmpSexScale == null ? Vector3.left : Vector3.right;
			this.Girl.nMove.Rot(this.Girl.transform.position + rot);
			this.Girl.transform.position = this.Man.transform.position + new Vector3(0f, 0.01f, -0.01f);

			yield return this.EnableLiveGirl(this.Girl);
			this.EnableLiveMan(this.Man);

			Object.Destroy(this.TmpSex);
		}

		public IEnumerator Run()
		{
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError("No performer found");
				yield break;
			}

			if (!this.SetupScene())
				yield break;

			this.Stage = 1;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield return this.Teardown();
				yield break;
			}

			yield return this.PerformBattle();
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield return this.Teardown();
				yield break;
			}

			this.Stage = 2;

			yield return this.PerformGiveup();
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield return this.Teardown();
				yield break;
			}

			yield return this.PerformRape();
			
			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
			yield return this.Teardown();
		}

		public bool CanContinue()
		{
			PLogger.LogDebug($">> Stage: {this.Stage} / Girl Faint: {this.Girl.faint} / Life: {this.Girl.life} / TmpSex: {this.TmpSex != null} / Man Life: {this.Man.life} / Input: {Input.GetKeyDown(KeyCode.R)}");
			if (Input.GetKeyDown(KeyCode.R))
				this.Aborted = true;

			if (this.Aborted)
				return false;

			if (this.Stage == 1)
				return this.Girl.life > 0.0 && this.TmpSex != null && this.Man.life > 0.0 && !Input.GetKeyDown(KeyCode.R);

			return !Input.GetKeyDown(KeyCode.R) && this.Man.life > 0;
		}

		public void Destroy()
		{
			this.Aborted = true;
		}

		public string GetName()
		{
			return ManRapes.Name;
		}

		public CommonStates[] GetActors()
		{
			return [this.Man, this.Girl];
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.TmpSexAnim;
		}

		public string ExpandAnimationName(string originalName)
		{
			var missingLegs = (this.Girl.dissect[4] == 1 && this.Girl.dissect[5] == 1);

			return originalName
				.Replace("<Tits>", this.Girl.parameters[6].ToString("00"))
				.Replace("<DisLeg>", missingLegs ? "DisLeg_" : "");
		}

		public SexPerformer GetPerformer()
		{
			return this.Performer;
		}
	}
}
