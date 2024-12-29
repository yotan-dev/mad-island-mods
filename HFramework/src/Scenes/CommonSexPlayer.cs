using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes
{
	public class CommonSexPlayer : IScene, IScene2
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

		public SkeletonAnimation CommonAnim { get; private set; }

		private ISceneController Controller;

		private readonly CommonSexPlayerMenuPanel MenuPanel;

		private SexPerformer Performer;

		public CommonSexPlayer(CommonStates playerCommon, CommonStates npcCommon, Vector3 pos, int sexType)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.Type = sexType;
			this.Position = pos;
			this.Actors = Utils.SortActors(playerCommon, npcCommon);

			this.MenuPanel = new CommonSexPlayerMenuPanel();
			this.MenuPanel.OnCaressSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnCaress());
			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnInsert());
			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnSpeed());
			this.MenuPanel.OnPose2Selected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnPose2());
			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnFinish());
			this.MenuPanel.OnStopSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnStop());
			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnLeave());
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			this.Controller.SetScene(this);
		}

		public string GetName()
		{
			return CommonSexPlayer.Name;
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
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Caress);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 1;
			yield return this.Performer.Perform(ActionType.Caress);

			this.MenuPanel.ShowCaressMenu(this.Performer.GetAlternativePoseName());
			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Caress);
		}

		private IEnumerator OnInsert()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Insert);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 2;
			yield return this.Performer.Perform(ActionType.Insert);
			yield return this.Performer.Perform(ActionType.Speed1);

			this.MenuPanel.ShowInsertMenu(this.Performer.GetAlternativePoseName());

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
		}

		private IEnumerator OnSpeed()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed);
			if (!this.CanContinue())
				yield break;

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

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed);
		}

		private IEnumerator OnPose2()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Pose2);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.ChangePose();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Pose2);
		}

		private IEnumerator OnFinish()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 0;
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Finish);

			yield return this.Performer.Perform(ActionType.FinishIdle);

			this.MenuPanel.Show();
			this.MenuPanel.ShowFinishMenu(this.Performer.GetAlternativePoseName());

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
		}

		private IEnumerator OnStop()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Stop);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 0;
			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.ShowStopMenu(this.Performer.GetAlternativePoseName());

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Stop);
		}

		private IEnumerator OnLeave()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Leave);
			if (!this.CanContinue())
				yield break;

			UnityEngine.Object.Destroy(this.TmpSex);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Leave);
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

		public IEnumerator Run()
		{
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
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


		public bool CanContinue()
		{
			return this.TmpSex != null && this.AreActorsAlive();
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public CommonStates[] GetActors()
		{
			return this.Actors;
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.CommonAnim;
		}

		public string ExpandAnimationName(string originalName)
		{
			return originalName.Replace("<Tits>", this.Npc.parameters[6].ToString("00"));
		}

		public SexPerformer GetPerformer()
		{
			return this.Performer;
		}
	}
}
