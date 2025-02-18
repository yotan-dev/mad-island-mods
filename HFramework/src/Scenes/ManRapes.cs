using System.Collections;
using HFramework.Handlers;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;

namespace HFramework.Scenes
{
	public class ManRapes : BaseScene
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

		public readonly double InitialLife;

		public readonly double InitialFaint;

		private GameObject TmpSex;

		private GameObject TmpSexScale;

		private int Stage = 0;

		public ManRapes(CommonStates girl, CommonStates man)
			: base(Name)
		{
			this.Man = man;
			this.Girl = girl;
			this.InitialLife = girl.life;
			this.InitialFaint = girl.faint;
		}

		private void PrepareGirl(CommonStates npc)
		{
			if (npc.nMove.actType != NPCMove.ActType.Dead)
				npc.nMove.actType = NPCMove.ActType.Wait;
		}

		private void DisableLiveGirl(CommonStates npc)
		{
			npc.nMove.RBState(false);

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
			else if (npc.anim.HasAnimation("A_dead_idle"))
				npc.anim.state.SetAnimation(0, "A_dead_idle", true);

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

			this.CommonAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			
			if (this.Girl.npcID == NpcID.Yona && string.IsNullOrEmpty(this.Girl.equip[3].itemKey) && this.CommonAnim.skeleton.FindSlot("Panty") != null)
				this.CommonAnim.skeleton.SetAttachment("Panty", null);

			this.TmpSexScale = null;
			if (this.Man.pMove.scale.transform.localScale.x == -1f)
			{
				this.TmpSexScale = this.TmpSex.transform.Find("Scale").gameObject;
				this.TmpSexScale.transform.localScale = new Vector3(-1f, 1f, 1f);
			}

			this.PrepareGirl(this.Girl);
			this.DisableLiveGirl(this.Girl);
			this.DisableLiveMan(this.Man);

			return true;
		}

		private IEnumerator PerformGrapple()
		{
			yield return new ManPlayerGrapples(this, this.CommonAnim, this.Man, this.Girl).Handle();
		}

		private IEnumerator PerformBattle()
		{
			yield return this.Performer.Perform(ActionType.Battle);
			yield return this.PerformGrapple();
		}

		private IEnumerator PerformGiveup()
		{
			yield return this.Performer.PerformBg(ActionType.Defeat);
			yield return new WaitForSeconds(0.5f);

			Managers.mn.uiMN.ControlTextActive(true, Managers.mn.textMN.texts[24] + "/n" + Managers.mn.textMN.texts[25]);

			yield return this.Controller.WaitForInput();

			Managers.mn.uiMN.ControlTextActive(false, "");
		}

		private IEnumerator Insert()
		{
			yield return this.Performer.Perform(ActionType.Insert);
		}

		private IEnumerator Speed1()
		{
			yield return this.Performer.PerformBg(ActionType.Speed1);
			yield return this.Controller.WaitForInput();
		}

		private IEnumerator Speed2()
		{
			yield return this.Performer.PerformBg(ActionType.Speed2);
			yield return this.Controller.WaitForInput();
		}

		private IEnumerator Finish()
		{
			yield return this.Performer.Perform(ActionType.Finish);

			this.Stage = 7;

			yield return this.Performer.PerformBg(ActionType.FinishIdle);
			yield return this.Controller.WaitForInput();
		}

		private IEnumerator PerformRape()
		{
			this.Stage = 3;

			yield return this.RunStep(StepNames.Insert, this.Insert);
			if (!this.CanContinue())
				yield break;

			this.Stage = 4;

			yield return this.RunStep(StepNames.Speed1, this.Speed1);
			if (!this.CanContinue())
				yield break;

			this.Stage = 5;

			yield return this.RunStep(StepNames.Speed2, this.Speed2);
			if (!this.CanContinue())
				yield break;

			this.Stage = 6;

			yield return this.RunStep(StepNames.Finish, this.Finish);
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

		public override IEnumerator Run()
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

			// If we don't put the prepare here, Shino animation gets messed up
			// and they do a roll at the beggining due to the yields for before Main and before Battle
			// other scenes would work fine as far as I can tell, but since this is the "original code"
			// let's make it for everyone.
			if (this.Girl.faint != 0)
				this.Performer.PreparePerform(ActionType.Battle);
			else
				this.Performer.PreparePerform(ActionType.Defeat);

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield return this.Teardown();
				yield break;
			}

			if (this.Girl.faint != 0)
			{
				yield return this.RunStep(StepNames.Battle, this.PerformBattle);
				if (!this.CanContinue())
				{
					yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
					yield return this.Teardown();
					yield break;
				}
			}

			this.Stage = 2;

			yield return this.RunStep(StepNames.Giveup, this.PerformGiveup);
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

		public override bool CanContinue()
		{
			if (Input.GetKeyDown(KeyCode.R))
				this.Destroyed = true;

			if (this.Destroyed)
				return false;

			if (this.Stage == 1)
			{
				// For some reason, Shino and Yona are killed once the battle ends...
				// but before Stage 1 ends. so bypass the life check in this case.
				if ((this.Girl.npcID == NpcID.Yona || this.Girl.npcID == NpcID.Shino) && this.Girl.faint == 0f)
					return this.TmpSex != null && this.Man.life > 0.0 && !Input.GetKeyDown(KeyCode.R);

				return this.Girl.life > 0.0 && this.TmpSex != null && this.Man.life > 0.0 && !Input.GetKeyDown(KeyCode.R);
			}

			return !Input.GetKeyDown(KeyCode.R) && this.Man.life > 0;
		}

		public override CommonStates[] GetActors()
		{
			return [this.Man, this.Girl];
		}

		public override string ExpandAnimationName(string originalName)
		{
			var missingLegs = (this.Girl.dissect[4] == 1 && this.Girl.dissect[5] == 1);

			return originalName
				.Replace("[Tits]", this.Girl.parameters[6].ToString("00"))
				.Replace("[DisLeg]", missingLegs ? "DisLeg_" : "");
		}
	}
}
