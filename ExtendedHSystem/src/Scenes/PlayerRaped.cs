using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace ExtendedHSystem.Scenes
{
	public class PlayerRaped : IScene
	{
		public readonly CommonStates Player;

		public readonly CommonStates Rapist;

		private NPCMove RapistMove;

		private GameObject TmpSex;

		private ISceneController Controller;

		private List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		public PlayerRaped(CommonStates player, CommonStates rapist)
		{
			this.Player = player;
			this.Rapist = rapist;
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
		}

		public void AddEventHandler(SceneEventHandler handler)
		{
			this.EventHandlers.Add(handler);
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

		private void EnableLivePlayer(CommonStates player)
		{
			CapsuleCollider toColl = player.GetComponent<CapsuleCollider>();
			toColl.enabled = true;

			// We can't restore the mesh here or the respawn will have 2 sprites...
			// MeshRenderer toMesh = player.anim.GetComponent<MeshRenderer>();
			// toMesh.enabled = true;
		}

		private GameObject GetFightScene()
		{
			GameObject scene = null;
			switch (this.Player.npcID)
			{
				case NpcID.Yona: // 0
					scene = Managers.mn.sexMN.sexObj[this.Rapist.npcID];
					break;

				case NpcID.Man: // 1
					switch (this.Rapist.npcID)
					{
						case NpcID.FemaleLargeNative: // 17
							scene = Managers.mn.sexMN.sexList[8].sexObj[0];
							break;

						case NpcID.Mummy: // 42
							scene = Managers.mn.sexMN.sexList[1].sexObj[6];
							break;

						default:
							PLogger.LogError("PlayerRaped#GetScene: Unexpected Man rapist npcID: " + this.Rapist.npcID);
							break;
					}
					break;

				default:
					PLogger.LogError("PlayerRaped#GetScene: Unexpected player npcID: " + this.Player.npcID);
					break;
			}

			return scene;
		}

		private GameObject GetSexScene()
		{
			GameObject scene = null;
			switch (this.Player.npcID)
			{
				case NpcID.Man: // 1
					switch (this.Rapist.npcID)
					{
						case NpcID.FemaleLargeNative: // 17
							scene = Managers.mn.sexMN.sexList[8].sexObj[1];
							break;
					}
					break;
			}

			return scene;
		}

		private bool SetupFightScene()
		{
			var scene = this.GetFightScene();
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
			var scene = this.GetSexScene();
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

		private IEnumerable PerformBattle()
		{
			var sexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			if (sexAnim.skeleton.Data.FindAnimation("A_Attack_loop") != null)
			{
				sexAnim.state.SetAnimation(0, "A_Attack_loop", true);
			}

			bool hasFainted = false;
			foreach (var x in this.Controller.PlayPlayerGrappledStep(this, sexAnim, "A_Attack_loop", this.Player))
			{
				if (x is bool v)
					hasFainted = v;
				yield return x;
			}

			if (!hasFainted)
			{
				Object.Destroy(this.TmpSex);
				this.EnableLiveNpc(this.Rapist);
				this.EnableLivePlayer(this.Player);
				this.RapistMove.actType = NPCMove.ActType.Interval;
				yield break;
			}

			if (this.Player.faint > 0.0)
				yield break;

			this.Player.sex = CommonStates.SexState.GameOver;
			var defeatStepControl = this.Controller.PlayUntilInputStep(this, sexAnim, "A_Attack_giveup");

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.PlayerDefeated())
					yield return x;
			}

			// Wait until the animation completes
			foreach (var x in defeatStepControl)
				yield return x;
		}

		private IEnumerable PerformSex()
		{
			if (this.CanContinue())
			{
				this.SetupSexScene();
				SkeletonAnimation sexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

				foreach (var x in this.Controller.PlayOnceStep(this, sexAnim, "A_AttackToSex"))
					yield return x;

				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.PlayerRaped(this.Player, this.Rapist, false))
						yield return x;
				}

				if (this.CanContinue())
				{
					foreach (var x in this.Controller.PlayUntilInputStep(this, sexAnim, "A_Loop_01"))
						yield return x;

					if (this.CanContinue())
					{
						foreach (var x in this.Controller.PlayUntilInputStep(this, sexAnim, "A_Loop_02"))
							yield return x;

						if (this.CanContinue())
						{
							foreach (var x in this.Controller.PlayOnceStep(this, sexAnim, "A_Finish"))
								yield return x;

							if (this.CanContinue())
							{
								foreach (var x in this.Controller.PlayUntilInputStep(this, sexAnim, "A_Finish_idle"))
									yield return x;
							}
						}
					}
				}
			}
			else
			{
				if (Managers.mn.uiMN.skip)
				{
					foreach (var handler in this.EventHandlers)
					{
						foreach (var x in handler.PlayerRaped(this.Player, this.Rapist, true))
							yield return x;
					}
				}
			}
		}

		public IEnumerator Run()
		{
			if (!this.SetupFightScene())
			{
				if (this.TmpSex != null)
					Object.Destroy(this.TmpSex);
				yield break;
			}

			foreach (var x in this.PerformBattle())
				yield return x;

			foreach (var x in this.PerformSex())
				yield return x;

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.BeforeRespawn())
					yield return x;
			}

			Object.Destroy(this.TmpSex);

			this.EnableLiveNpc(this.Rapist);

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.AfterRape(this.Player, this.Rapist))
					yield return x;
			}

			this.EnableLivePlayer(this.Player);

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.Respawn(this.Player, this.Rapist))
					yield return x;
			}

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
	}
}