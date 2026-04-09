using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using YotanModCore;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class AssWall : BaseScene
	{
		public static readonly string Name = "HF_AssWall";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Stop = "Stop";
			public const string Insert = "Insert";
			public const string Speed = "Speed";
			public const string Finish = "Finish";
			public const string Leave = "Leave";
		}

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpWall;

		private readonly AssWallMenuPanel MenuPanel;

		public AssWall(CommonStates playerCommon, CommonStates npcCommon, InventorySlot tmpWall = null)
			: base(AssWall.Name)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.TmpWall = tmpWall;

			this.MenuPanel = new AssWallMenuPanel();
			this.MenuPanel.OnInsertSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Insert, this.OnInsert));
			};
			this.MenuPanel.OnSpeedSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Speed, this.OnSpeed));
			};
			this.MenuPanel.OnFinishSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Finish, this.OnFinish));
			};
			this.MenuPanel.OnStopSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.StartLongLivedStep(StepNames.Stop, this.OnStop));
			};
			this.MenuPanel.OnLeaveSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.RunStep(StepNames.Leave, this.OnLeave));
			};
		}

		private IEnumerator OnInsert()
		{
			this.MenuPanel.ShowInsertMenu();

			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Speed1);
			if (!this.CanContinue())
				yield break;
		}

		private IEnumerator OnSpeed()
		{
			if (!this.CanContinue())
				yield break;

			if (this.Performer.CurrentAction == ActionType.Speed1)
				yield return this.Performer.Perform(ActionType.Speed2);
			else
				yield return this.Performer.Perform(ActionType.Speed1);
		}

		private IEnumerator OnStop()
		{
			if (!this.CanContinue())
				yield break;

			this.MenuPanel.ShowStopMenu();
			yield return this.Performer.Perform(ActionType.StartIdle);
		}

		private IEnumerator OnFinish()
		{
			if (!this.CanContinue())
				yield break;

			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Finish);
			yield return this.Performer.Perform(ActionType.FinishIdle);
			if (!this.CanContinue())
				yield break;

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();
		}

		private IEnumerator OnLeave()
		{
			if (!this.CanContinue())
				yield break;

			this.Destroy();
		}

		public override IEnumerator Run()
		{
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError($"AssWall: Failed to get sex performer for {this.Player.npcID} x {this.Npc.npcID}");
				yield break;
			}

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield break;
			}

			// Lock place
			SexPlace sexPlace = this.TmpWall.GetComponent<SexPlace>();
			sexPlace.user = this.Player.gameObject;

			// Disabe UI
			this.Controller.SetMainCanvasVisible(false);
			this.Controller.SetGameControllable(false, false);
			this.Controller.SetPlayerVisible(false);

			// Setup scene
			this.CommonAnim = Managers.mn.inventory.tmpSubInventory.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim.skeleton.SetSkin("Man");
			this.CommonAnim.skeleton.SetSlotsToSetupPose();
			Managers.mn.randChar.SetCharacter(Managers.mn.inventory.tmpSubInventory.gameObject, null, this.Player);
			Managers.mn.randChar.SetAssWall(this.Npc, this.CommonAnim.gameObject);

			// Start animation
			yield return this.Performer.Perform(ActionType.StartIdle);

			// Show UI
			this.MenuPanel.Open(TmpWall.transform.position);
			this.MenuPanel.ShowInitialMenu();

			while (this.CanContinue())
				yield return null;

			this.CommonAnim.skeleton.SetSkin("default");
			this.CommonAnim.skeleton.SetSlotsToSetupPose();

			if (this.TmpWall != null)
				Managers.mn.sexMN.StartCoroutine(Managers.mn.gameMN.ToiletCheck(this.TmpWall, 0));

			sexPlace.user = null;

			// It is already closed by whoever called destroy
			// this.MenuPanel.Close();

			this.Controller.SetGameControllable(true, true);
			this.Controller.SetPlayerVisible(true);
			this.Controller.SetMainCanvasVisible(true);

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
	}
}
