using System.Collections;
using System.Collections.Generic;
using Spine;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes
{
	public class ManRapes : IScene
	{
		public readonly CommonStates Man;

		public readonly CommonStates Girl;

		private NPCMove GirlMove;

		private GameObject TmpSex;

		private SkeletonAnimation TmpSexAnim;

		private GameObject TmpSexScale;

		private ISceneController Controller;

		private List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		public bool LoveChange { get; private set; } = true;

		private string SexType = "A_";

		private bool Pregable = true;

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
		}

		public void AddEventHandler(SceneEventHandler handler)
		{
			this.EventHandlers.Add(handler);
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
				yield return this.Controller.LoopAnimation(this, this.TmpSexAnim, "A_dead_idle");

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

		private GameObject GetScene()
		{
			GameObject scene = null;

			switch (this.Girl.npcID)
			{
				case NpcID.Yona: // 0
					if (this.Girl.faint > 0.0 || this.Girl.life > 0.0)
						break;

					scene = Managers.mn.sexMN.sexObj[23];
					this.LoveChange = false;
					break;

				case NpcID.FemaleNative: // 15
					if (this.Girl.faint <= 0.0 || this.Girl.life <= 0.0)
					{
						scene = Managers.mn.sexMN.sexList[1].sexObj[21];
						this.LoveChange = false;
					}
					else if (!CommonUtils.IsPregnant(this.Girl))
					{
						if (this.Girl.dissect[4] == 1 && this.Girl.dissect[5] == 1)
							this.SexType = "DisLeg_A_";

						scene = Managers.mn.sexMN.sexList[1].sexObj[0];
					}
					else
					{
						scene = Managers.mn.sexMN.sexList[1].sexObj[24];
					}
					break;

				case NpcID.NativeGirl: // 16
					if (this.Girl.faint <= 0.0 || this.Girl.life <= 0.0)
					{
						scene = Managers.mn.sexMN.sexList[1].sexObj[22];
						this.LoveChange = false;
					}
					else
					{
						scene = Managers.mn.sexMN.sexList[1].sexObj[2];
					}
					break;

				case NpcID.FemaleLargeNative: // 17
					scene = Managers.mn.sexMN.sexList[1].sexObj[15];
					break;

				case NpcID.OldManNative: // 18 ??
					break;

				case NpcID.OldWomanNative: // 19
					scene = Managers.mn.sexMN.sexList[1].sexObj[10];
					break;

				case NpcID.Mummy: // 42
					scene = Managers.mn.sexMN.sexList[1].sexObj[6];

					this.SexType = "Rape_A_";
					// this.SexCountType = 1; // official code sets it but does not use
					this.Pregable = false;
					break;

				case NpcID.UnderGroundWoman: // 44
					scene = Managers.mn.sexMN.sexList[1].sexObj[8];
					break;

				case NpcID.ElderSisterNative: // 90
					scene = Managers.mn.sexMN.sexList[1].sexObj[18];
					break;

				case NpcID.Shino: // 114
					scene = Managers.mn.sexMN.sexList[1].sexObj[19];
					this.SexType = "Rape_A_";
					break;
			}

			return scene;
		}

		private bool SetupScene()
		{
			var scene = this.GetScene();
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

		private IEnumerable PerformBattle()
		{
			string text = this.SexType + "Attack_loop";
			if (this.TmpSexAnim.skeleton.Data.FindAnimation(text) != null)
			{
				this.TmpSexAnim.state.SetAnimation(0, text, true);
				this.TmpSexAnim.state.Data.SetMix(text, this.SexType + "Attack_attack", 0f);
			}

			bool shouldContinue = false;
			foreach (var x in this.Controller.PlayPlayerGrapplesStep(this, this.TmpSexAnim, this.SexType, this.Man, this.Girl))
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			this.Aborted = !shouldContinue;
		}

		private IEnumerable PerformGiveup()
		{
			var giveupAnimTracker = this.Controller.PlayUntilInputStep(this, this.TmpSexAnim, "Attack_giveup");
			yield return new WaitForSeconds(0.5f);

			Managers.mn.uiMN.ControlTextActive(true, Managers.mn.textMN.texts[24] + "/n" + Managers.mn.textMN.texts[25]);

			bool shouldContinue = false;
			foreach (var x in giveupAnimTracker)
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			Managers.mn.uiMN.ControlTextActive(false, "");

			this.Aborted = !shouldContinue;
		}

		private IEnumerable PerformAttackToSex()
		{
			bool shouldContinue = false;
			foreach (var x in this.Controller.PlayOnceStep(this, this.TmpSexAnim, this.SexType + "AttackToSex"))
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			this.Aborted = !shouldContinue;
		}

		private IEnumerable PerformSexLoop1()
		{
			bool shouldContinue = false;
			foreach (var x in this.Controller.PlayUntilInputStep(this, this.TmpSexAnim, this.SexType + "Loop_01"))
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			this.Aborted = !shouldContinue;
		}

		private IEnumerable PerformSexLoop2()
		{
			bool shouldContinue = false;
			foreach (var x in this.Controller.PlayUntilInputStep(this, this.TmpSexAnim, this.SexType + "Loop_02"))
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			this.Aborted = !shouldContinue;
		}

		private IEnumerable PerformFinish()
		{
			bool shouldContinue = false;
			foreach (var x in this.Controller.PlayOnceStep(this, this.TmpSexAnim, this.SexType + "Finish", true))
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			this.Aborted = !shouldContinue;

			if (this.Aborted || !this.Pregable)
				yield break;

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnCreampie(this.Man, this.Girl))
					yield return x;
			}
		}

		private IEnumerable PerformFinishIdle()
		{
			bool shouldContinue = false;
			foreach (var x in this.Controller.PlayUntilInputStep(this, this.TmpSexAnim, this.SexType + "Finish_idle"))
			{
				if (x is bool b)
					shouldContinue = b;
				yield return x;
			}

			this.Aborted = !shouldContinue;

			if (this.Aborted || !this.Pregable)
				yield break;

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnCreampie(this.Man, this.Girl))
					yield return x;
			}
		}

		private IEnumerable PerformRape()
		{
			this.Stage = 3;

			foreach (var x in this.PerformAttackToSex())
				yield return x;

			if (!this.CanContinue())
				yield break;

			this.Stage = 4;

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnRape(this, this.Man, this.Girl))
					yield return x;
			}

			if (!this.CanContinue())
				yield break;

			this.Stage = 5;

			foreach (var x in this.PerformSexLoop1())
				yield return x;

			if (!this.CanContinue())
				yield break;

			this.Stage = 6;

			foreach (var x in this.PerformSexLoop2())
				yield return x;

			if (!this.CanContinue())
				yield break;

			this.Stage = 7;

			foreach (var x in this.PerformFinish())
				yield return x;

			if (!this.CanContinue())
				yield break;

			this.Stage = 8;

			foreach (var x in this.PerformFinishIdle())
				yield return x;
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
			if (!this.SetupScene())
				yield break;

			this.Stage = 1;

			foreach (var x in this.PerformBattle())
				yield return x;

			if (!this.CanContinue())
			{
				yield return this.Teardown();
				yield break;
			}

			this.Stage = 2;

			foreach (var x in this.PerformGiveup())
				yield return x;

			if (!this.CanContinue())
			{
				yield return this.Teardown();
				yield break;
			}

			foreach (var x in this.PerformRape())
				yield return x;

			yield return this.Teardown();
		}

		public bool CanContinue()
		{
			PLogger.LogInfo($">> Stage: {this.Stage} / Girl Faint: {this.Girl.faint} / Life: {this.Girl.life} / TmpSex: {this.TmpSex != null} / Man Life: {this.Man.life} / Input: {Input.GetKeyDown(KeyCode.R)}");
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
	}
}
