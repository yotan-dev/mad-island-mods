using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class Daruma : BaseScene
	{
		public static readonly string Name = "HF_Daruma";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Insert = "Insert";
			public const string Speed = "Speed";
			public const string Finish = "Finish";
			public const string Stop = "Stop";
			public const string Leave = "Leave";
		}

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpDaruma;

		private GameObject SexObj;

		private GameObject DarumaObj;

		private readonly DarumaMenuPanel MenuPanel;

		public Daruma(CommonStates playerCommon, CommonStates npcCommon, InventorySlot tmpDaruma = null)
			: base(Name)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.TmpDaruma = tmpDaruma;

			this.MenuPanel = new DarumaMenuPanel();
			this.MenuPanel.OnInsertSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Insert, this.OnInsert));
			this.MenuPanel.OnSpeedSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Speed, this.OnSpeed));
			this.MenuPanel.OnFinishSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Finish, this.OnFinish));
			this.MenuPanel.OnStopSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Stop, this.OnStop));
			this.MenuPanel.OnLeaveSelected += (object s, int e) =>
				Managers.mn.sexMN.StartCoroutine(this.RunStep(StepNames.Leave, this.OnLeave));
		}

		private IEnumerator OnInsert()
		{
			this.MenuPanel.ShowInsertMenu();

			yield return this.Performer.Perform(ActionType.Speed1);
		}

		private IEnumerator OnSpeed()
		{
			if (this.Performer.CurrentAction == ActionType.Speed1)
				yield return this.Performer.Perform(ActionType.Speed2);
			else
				yield return this.Performer.Perform(ActionType.Speed1);
		}

		private IEnumerator OnStop()
		{
			this.MenuPanel.ShowStopMenu();
			yield return this.Performer.Perform(ActionType.StartIdle);
		}

		private IEnumerator OnFinish()
		{
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Finish);

			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.FinishIdle);

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();
		}

		private IEnumerator OnLeave()
		{
			this.Destroy();
			yield break;
		}

		private bool SetupScene()
		{
			Transform transform = this.TmpDaruma.transform.Find("pos_00");
			this.DarumaObj = transform?.transform?.GetChild(0)?.gameObject ?? null;

			if (this.DarumaObj == null || this.Npc == null)
			{
				PLogger.LogError("Daruma object not found or is empty.");
				return false;
			}

			this.DarumaObj.SetActive(false);

			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError("Performer not found.");
				return false;
			}

			var scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
			if (scene == null)
				return false;

			this.SexObj = GameObject.Instantiate(scene, this.DarumaObj.transform.position, Quaternion.identity);
			this.CommonAnim = SexObj.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			Managers.mn.randChar.SetCharacter(this.SexObj, this.Npc, Managers.mn.gameMN.playerCommons[1]);

			return true;
		}

		public override IEnumerator Run()
		{
			// Scene setup
			if (!this.SetupScene())
				yield break;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				this.Destroy();
				yield break;
			}

			Managers.mn.uiMN.MainCanvasView(false);
			Managers.mn.gameMN.Controlable(false, true);
			Managers.mn.gameMN.pMove.PlayerVisible(false);

			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.Open(this.TmpDaruma.transform.position);
			this.MenuPanel.ShowInitialMenu();

			while (this.CanContinue())
				yield return null;

			yield return this.EndLongLivedStep();

			if (this.SexObj != null)
				Object.Destroy(this.SexObj);

			if (this.DarumaObj != null)
				this.DarumaObj.SetActive(true);

			Managers.mn.gameMN.Controlable(true, true);
			Managers.mn.gameMN.pMove.PlayerVisible(true);
			Managers.mn.uiMN.MainCanvasView(true);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
		}

		public override bool CanContinue()
		{
			return !this.Destroyed && PropPanelManager.Instance.IsOpen();
		}

		public override void Destroy()
		{
			this.Destroyed = true;
			this.MenuPanel?.Close();
		}

		public override CommonStates[] GetActors()
		{
			return [this.Player, this.Npc];
		}

		public override string ExpandAnimationName(string originalName)
		{
			return originalName.Replace("<Tits>", this.Npc.parameters[6].ToString("00"));
		}
	}
}
