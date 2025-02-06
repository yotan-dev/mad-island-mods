using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

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
		}

		private bool AreActorsAlive()
		{
			return this.Player.life > 0.0 && this.Npc.life > 0.0;
		}

		private void SetupTmpSex()
		{
			if (this.Player.npcID == NpcID.Yona)
			{
				// NOTE: right know, all Yona scenes are with humans. Maybe this will change later...
				// NPC IDs (as of v0.2.3): 10, 11, 12, 91
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
			}
			else
			{
				if (this.Npc.npcID == NpcID.Reika)
				{
					// @TODO: Probably can be simplified
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				}
				else if (this.Npc.npcID == NpcID.Mermaid)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
				}
				else if (this.Npc.npcID == NpcID.Mummy)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadMummy(this.Npc, this.TmpSex);
				}
				else if (this.Npc.npcID == NpcID.UnderGroundWoman)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadGenUnder(this.Npc, this.TmpSex);
				}
				else
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				}
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
			SexMeter.Instance.Init(this.Position + new Vector3(1f, 1f, 0f), 0.3f);
			SexMeter.Instance.Show();

			Managers.mn.uiMN.MainCanvasView(false);

			this.TmpCommonState = 0;
			this.TmpSexCountType = 0;

			var scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
			if (scene == null)
				return false;

			this.TmpSex = GameObject.Instantiate<GameObject>(scene, this.Position, Quaternion.identity);
			this.SetupTmpSex();

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

		private void Teardown()
		{
			this.EnableLiveNpc(this.Npc, this.NpcMove);
			this.EnableLivePlayer(this.Player);

			this.MenuPanel.Close();

			Managers.mn.uiMN.MainCanvasView(true);
			SexMeter.Instance.Hide();

			Managers.mn.uiMN.StatusChange(null);
		}

		protected virtual SexPerformer SelectPerformer()
		{
			return ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
		}

		public override IEnumerator Run()
		{
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

				Debug.LogError("sex not found");
				yield break;
			}

			this.CommonAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

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
