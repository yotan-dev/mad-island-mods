using System;
using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;

namespace HFramework.Scenes
{
	public class CommonSexPlayer : BaseScene
	{
		public static readonly string Name = "HF_CommonSexPlayer";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Caress = "Caress";
			public const string Insert = "Insert";
			public const string Speed = "Speed";
			public const string Pose2 = "Pose2";
			public const string Finish = "Finish";
			public const string Stop = "Stop";
			public const string Leave = "Leave";
			public const string ToggleManDisplay = "ToggleManDisplay";
		}

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly int Type;

		private readonly CommonStates[] Actors;

		private Vector3 Position;

		private NPCMove NpcMove;

		private GameObject TmpSex = null;

		private int TmpCommonState = 0;

		public int TmpSexCountType { get; private set; } = 0;

		private float NpcAngle;

		private readonly CommonSexPlayerMenuPanel MenuPanel;

		public CommonSexPlayer(CommonStates playerCommon, CommonStates npcCommon, Vector3 pos, int sexType)
			: base(Name)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.Type = sexType;
			this.Position = pos;
			this.Actors = Utils.SortActors(playerCommon, npcCommon);

			this.MenuPanel = new CommonSexPlayerMenuPanel();
			this.MenuPanel.OnCaressSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Caress, this.OnCaress));
			this.MenuPanel.OnInsertSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Insert, this.OnInsert));
			this.MenuPanel.OnSpeedSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Speed, this.OnSpeed));
			this.MenuPanel.OnPose2Selected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Pose2, this.OnPose2));
			this.MenuPanel.OnFinishSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Finish, this.OnFinish));
			this.MenuPanel.OnStopSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Stop, this.OnStop));
			this.MenuPanel.OnLeaveSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.RunStep(StepNames.Leave, this.OnLeave));
			this.MenuPanel.OnToggleManDisplaySelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.RunStep(StepNames.ToggleManDisplay, this.OnToggleManDisplay));
		}

		public override int GetSexType()
		{
			return this.Type;
		}

		private bool AreActorsAlive()
		{
			return this.Player.life > 0.0 && this.Npc.life > 0.0;
		}

		private void SetupTmpSexYonaNami()
		{
			Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, null);
			CommonStates tmpSexCommon = this.TmpSex.GetComponent<CommonStates>();
			if (tmpSexCommon != null)
			{
				Managers.mn.randChar.CopyParams(this.Npc, tmpSexCommon);
				Managers.mn.randChar.LoadGenGirl(this.TmpSex, false, RandomCharacter.LoadType.G2);
				AnimationFX sexPlaceAnimFx = tmpSexCommon.anim.GetComponent<AnimationFX>();
				if (sexPlaceAnimFx != null) {
					sexPlaceAnimFx.voiceID = this.Player.voiceID;
					sexPlaceAnimFx.voice2ID = this.Npc.voiceID;
				}
			}
		}

		private void SetupTmpSexYonaNativeBoy()
		{
			Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
			AnimationFX componentInChildren = this.TmpSex.GetComponentInChildren<AnimationFX>();
			if (componentInChildren != null)
			{
				componentInChildren.voiceID = this.Npc.voiceID;
				componentInChildren.voice2ID = this.Player.voiceID;
			}
		}

		private void SetupTmpSexYonaSon()
		{
			Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
			AnimationFX componentInChildren = this.TmpSex.GetComponentInChildren<AnimationFX>();
			if (componentInChildren != null)
			{
				componentInChildren.voiceID = this.Player.voiceID;
				componentInChildren.voice2ID = this.Npc.voiceID;
			}
		}

		private void SetupTmpSexYonaUnderGroundBoy()
		{
			Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
			AnimationFX componentInChildren = this.TmpSex.GetComponentInChildren<AnimationFX>();
			if (componentInChildren != null)
			{
				componentInChildren.voiceID = this.Npc.voiceID;
			}
		}

		private void SetupTmpSex()
		{
			switch (this.Player.npcID)
			{
			case NpcID.Yona:
				// NOTE: right know, all Yona scenes are with humans. Maybe this will change later...
				//       As of v0.4.0, Nami is an exception for SetCharacter
				// NPC IDs (as of v0.2.3): 10, 11, 12, 91
				// NPC IDs (as of v0.4.0): + 6
				// NPC IDs (as of v0.4.4): + 14, 141
				// NPC IDs (as of v0.4.5): + 180
				// NPC IDs (as of v0.5.0): + 1, 7, 89
				// NPC IDs (as of v0.5.4): + 72
				if (this.Npc.npcID == NpcID.Nami || this.Npc.npcID == NpcID.SlenderYoungLady)
					this.SetupTmpSexYonaNami();
				else if (this.Npc.npcID == NpcID.NativeBoy)
					this.SetupTmpSexYonaNativeBoy(); // Note: This happen inside NativeBoy case
				else if (this.Npc.npcID == NpcID.Son)
					this.SetupTmpSexYonaSon(); // Note: This happen inside Son case
				else if (this.Npc.npcID == NpcID.UnderGroundBoy)
					this.SetupTmpSexYonaUnderGroundBoy();
				else
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
				break;

			case NpcID.Man:
				// New decomp (v0.4.2) shows it is not needed
				// @TODO: Check if this has side effects on current stable
				// if (this.Npc.npcID == NpcID.Reika)
				// {
				// 	// @TODO: Probably can be simplified
				// 	Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
				// 	Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				// }

				// v0.5.12.4 or 0.5.12.5 changed this so SetCharacter for Mermaid goes through normal flow (NPC, Player)
				if (this.Npc.npcID == NpcID.Mermaid && GameInfo.GameVersion <= GameInfo.ToVersion("0.5.12.3"))
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
				}
				else if (this.Npc.npcID == NpcID.Mummy) // 42
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadMummy(this.Npc, this.TmpSex);
				}
				else if (this.Npc.npcID == NpcID.UndergroundWoman) // 44
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadGenUnder(this.Npc, this.TmpSex);
				}
				else if (this.Npc.npcID == NpcID.Cyborg) // 88
				{
					SkeletonAnimation subSkeletonAnimation = this.TmpSex.GetComponentInChildren<SkeletonAnimation>();
					subSkeletonAnimation.skeleton.SetSkin("Man");
					subSkeletonAnimation.skeleton.SetSlotsToSetupPose();
					CyborgCommon cyborgCommon = this.Npc.GetComponent<CyborgCommon>();
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					if (cyborgCommon != null)
						Managers.mn.randChar.SetCyborg(this.TmpSex, cyborgCommon.cyborgParams, noOffset: true);
				}
				else
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				}
				break;

			case NpcID.Reika: // 5
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
				break;

			case NpcID.Takumi: // 7
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				break;
			}
		}

		private void DisableLiveNpc(CommonStates npc, NPCMove npcMove)
		{
			npcMove.actType = NPCMove.ActType.Wait;
			this.NpcAngle = npcMove.searchAngle;
			this.NpcMove.searchAngle = 0f;
			npcMove.RBState(false);

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = false;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = false;

			Managers.mn.randChar.HandItemHide(npc, true);
		}

		private void EnableLiveNpc(CommonStates npc, NPCMove npcMove)
		{
			Managers.mn.randChar.HandItemHide(npc, false);

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = true;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			npcMove.actType = NPCMove.ActType.Idle;
			npcMove.searchAngle = this.NpcAngle;
			npc.gameObject.transform.position = this.Position;
			npc.sex = CommonStates.SexState.None;
		}


		private void DisableLivePlayer(CommonStates player)
		{
			MeshRenderer playerMesh = player.anim.GetComponent<MeshRenderer>();
			playerMesh.enabled = false;

			Managers.mn.randChar.HandItemHide(player, true);

		}

		private void EnableLivePlayer(CommonStates player)
		{
			Managers.mn.randChar.HandItemHide(player, false);

			MeshRenderer playerMesh = player.anim.GetComponent<MeshRenderer>();
			playerMesh.enabled = true;
		}

		private bool SetupScene()
		{
			this.TmpCommonState = 0;
			this.TmpSexCountType = 0;

			var scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
			if (scene == null)
				return false;

			this.TmpSex = GameObject.Instantiate<GameObject>(scene, this.Position, Quaternion.identity);
			/* TODO:
				this.tmpCommonSex = this.InstantiateFreeList(list, index1, pos, Quaternion.identity);
				if ((UnityEngine.Object) this.tmpCommonSex == (UnityEngine.Object) null && index2 >= 0)
					this.tmpCommonSex = this.InstantiateFreeList(list, index2, pos, Quaternion.identity);
			*/

			this.SetupTmpSex();

			SexMeter.Instance.Init(this.Position + new Vector3(1f, 1f, 0f), 0.3f);
			SexMeter.Instance.Show();

			Managers.mn.uiMN.MainCanvasView(false);

			this.NpcMove = this.Npc.GetComponent<NPCMove>();
			this.DisableLiveNpc(this.Npc, this.NpcMove);
			this.DisableLivePlayer(this.Player);

			return true;
		}

		public IEnumerator Idle()
		{
			this.MenuPanel.ShowInitialMenu(this.Performer.GetAlternativePoseName());
			this.MenuPanel.Show();

			this.TmpCommonState = 0;

			// TODO: FreeInteractionSource.Observe(this.tmpCommonSex, pCommon, nCommon, this.tmpCommonAnim);
			yield return this.Performer.Perform(ActionType.StartIdle);
		}

		private IEnumerator OnCaress()
		{
			this.TmpCommonState = 1;
			yield return this.Performer.Perform(ActionType.Caress);

			this.MenuPanel.ShowCaressMenu(this.Performer.GetAlternativePoseName());
		}

		private IEnumerator OnInsert()
		{
			this.TmpCommonState = 2;
			yield return this.Performer.Perform(ActionType.Insert);
			yield return this.Performer.Perform(ActionType.Speed1);

			this.MenuPanel.ShowInsertMenu(this.Performer.GetAlternativePoseName());
		}

		private IEnumerator OnSpeed()
		{
			if (this.TmpCommonState == 2) /* Slow */
			{
				this.TmpCommonState = 3;
				yield return this.Performer.Perform(ActionType.Speed2);
			}
			else // (this.TmpCommonState == 3 /* Fast */)
			{
				this.TmpCommonState = 2;
				yield return this.Performer.Perform(ActionType.Speed1);
			}

			this.MenuPanel.ShowInsertMenu(this.Performer.GetAlternativePoseName());
		}

		private IEnumerator OnPose2()
		{
			yield return this.Performer.ChangePose();
		}

		private IEnumerator OnFinish()
		{
			this.TmpCommonState = 0;
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Finish);

			/* TODO:
			actionObject = this.tmpCommonSex;
			actionAnimation = this.tmpCommonAnim;
			yield return (object) new WaitForSeconds(this.tmpCommonAnim.state.GetCurrent(0).AnimationEnd);
			if ((UnityEngine.Object) actionObject == (UnityEngine.Object) null || (UnityEngine.Object) actionObject != (UnityEngine.Object) this.tmpCommonSex || (UnityEngine.Object) actionAnimation == (UnityEngine.Object) null || (UnityEngine.Object) actionAnimation != (UnityEngine.Object) this.tmpCommonAnim || (UnityEngine.Object) pCommon == (UnityEngine.Object) null || (UnityEngine.Object) nCommon == (UnityEngine.Object) null || (UnityEngine.Object) pCommon != (UnityEngine.Object) this.tmpCommonPlayer || (UnityEngine.Object) nCommon != (UnityEngine.Object) this.tmpCommonGirl)
				yield break;
			*/

			yield return this.Performer.Perform(ActionType.FinishIdle);

			this.MenuPanel.Show();
			this.MenuPanel.ShowFinishMenu(this.Performer.GetAlternativePoseName());
		}

		private IEnumerator OnStop()
		{
			this.TmpCommonState = 0;
			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.ShowStopMenu(this.Performer.GetAlternativePoseName());
		}

		private IEnumerator OnLeave()
		{
			UnityEngine.Object.Destroy(this.TmpSex);
			yield break;
		}

		private IEnumerator OnToggleManDisplay()
		{
			if (this.CommonAnim.state.GetCurrent(10)?.Animation != null)
				this.CommonAnim.state.SetEmptyAnimation(10, 0);
			else
				this.CommonAnim.state.SetAnimation(10, "noman", true);
			yield break;
		}

		private void Teardown()
		{
			this.EnableLiveNpc(this.Npc, this.NpcMove);
			this.EnableLivePlayer(this.Player);

			this.MenuPanel.Close();

			Managers.mn.uiMN.MainCanvasView(true);
			SexMeter.Instance.Hide();

			/* TODO:
			if ((UnityEngine.Object) this.tmpCommonPlayer == (UnityEngine.Object) pCommon)
				this.tmpCommonPlayer = (CommonStates) null;
			*/

			Managers.mn.uiMN.StatusChange(null);
		}

		protected virtual SexPerformer SelectPerformer()
		{
			return ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
		}

		public override IEnumerator Run()
		{
			/*
			TODO:

				if (state == 0)
				{
				if ((UnityEngine.Object) pCommon == (UnityEngine.Object) null || (UnityEngine.Object) nCommon == (UnityEngine.Object) null || (UnityEngine.Object) pCommon == (UnityEngine.Object) nCommon)
					yield break;
				this.tmpCommonPlayer = pCommon;
				}
				else
				{
				pCommon = this.tmpCommonPlayer;
				nCommon = this.tmpCommonGirl;
				if ((UnityEngine.Object) pCommon == (UnityEngine.Object) null || (UnityEngine.Object) nCommon == (UnityEngine.Object) null || (UnityEngine.Object) this.tmpCommonSex == (UnityEngine.Object) null || (UnityEngine.Object) this.tmpCommonAnim == (UnityEngine.Object) null)
					yield break;
				}
			*/
			this.Performer = this.SelectPerformer();
			if (this.Performer == null)
			{
				PLogger.LogError("No performer found");
				yield break;
			}

			if (!this.SetupScene())
			{
				if (this.TmpSex != null)
					UnityEngine.Object.Destroy(this.TmpSex);

				PLogger.LogError($"CommonSexPlayer: Failed to create sex scene. Player NPC ID: {this.Player.npcID} / Target NPC ID: {this.Npc.npcID}");
				yield break;
			}

			// v0.5.8.0 now has "scale" too
			this.CommonAnim = (this.TmpSex.transform.Find("Scale/Anim") ?? this.TmpSex.transform.Find("scale/Anim"))?.gameObject?.GetComponent<SkeletonAnimation>();
			if (this.CommonAnim == null) {
				PLogger.LogError("CommonAnim not found");

				/* TODO:

				if ((UnityEngine.Object) nCommon != (UnityEngine.Object) null && nCommon.sex == CommonStates.SexState.Playing)
				{
					nCommon.sex = CommonStates.SexState.None;
					if ((UnityEngine.Object) nCommon.nMove != (UnityEngine.Object) null && nCommon.nMove.actType == NPCMove.ActType.Wait)
					nCommon.nMove.actType = NPCMove.ActType.Idle;
				}
				if (!((UnityEngine.Object) this.tmpCommonPlayer == (UnityEngine.Object) pCommon))
					yield break;
				this.tmpCommonPlayer = (CommonStates) null;
				*/

				yield break;
			}

			this.MenuPanel.CanToggleManDisplay = this.CommonAnim.HasAnimation("noman");

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				this.Teardown();
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield break;
			}

			this.MenuPanel.Open(this.Position);
			yield return this.Idle();

			while (this.CanContinue())
			{
				switch (this.TmpCommonState)
				{
					case 1:
						if (SexMeter.Instance.FillAmount <= 0.3f)
							SexMeter.Instance.Fill(Time.deltaTime * 0.03f);
						else
							SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
						break;
					case 2:
						if (SexMeter.Instance.FillAmount <= 0.3f)
							SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
						else
							SexMeter.Instance.Fill(Time.deltaTime * 0.03f);
						break;
					case 3:
						if (SexMeter.Instance.FillAmount <= 0.3f)
							SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
						else
							SexMeter.Instance.Fill(Time.deltaTime * 0.05f);
						break;
				}
				yield return null;
			}

			this.Teardown();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
		}


		public override bool CanContinue()
		{
			return !this.Destroyed && this.TmpSex != null && this.AreActorsAlive();
		}

		public override void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public override CommonStates[] GetActors()
		{
			return this.Actors;
		}

		public override string ExpandAnimationName(string originalName)
		{
			return originalName.Replace("[Tits]", this.Npc.parameters[6].ToString("00"));
		}
	}
}
