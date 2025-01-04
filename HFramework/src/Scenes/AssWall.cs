using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using YotanModCore;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class AssWall : IScene
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

		private SkeletonAnimation CommonAnim;

		private ISceneController Controller;

		private SexPerformer Performer;

		private readonly AssWallMenuPanel MenuPanel;

		public AssWall(CommonStates playerCommon, CommonStates npcCommon, InventorySlot tmpWall = null)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.TmpWall = tmpWall;

			this.MenuPanel = new AssWallMenuPanel();
			this.MenuPanel.OnInsertSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnInsert(sender, e));
			};
			this.MenuPanel.OnSpeedSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnSpeed(sender, e));
			};
			this.MenuPanel.OnFinishSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnFinish(sender, e));
			};
			this.MenuPanel.OnStopSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnStop(sender, e));
			};
			this.MenuPanel.OnLeaveSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnLeave(sender, e));
			};
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			controller.SetScene(this);
		}

		private IEnumerator OnInsert(object sender, int e)
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Insert);
			if (!this.CanContinue())
				yield break;

			this.MenuPanel.ShowInsertMenu();

			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.Speed1);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
				yield break;
			}

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
		}

		private IEnumerator OnSpeed(object sender, int e)
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed);
				yield break;
			}

			if (this.Performer.CurrentAction == ActionType.Speed1)
				yield return this.Performer.Perform(ActionType.Speed2);
			else
				yield return this.Performer.Perform(ActionType.Speed1);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed);
		}

		private IEnumerator OnStop(object sender, int e)
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Stop);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Stop);
				yield break;
			}

			this.MenuPanel.ShowStopMenu();
			yield return this.Performer.Perform(ActionType.StartIdle);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Stop);
		}

		private IEnumerator OnFinish(object sender, int e)
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
				yield break;
			}

			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Finish);
			yield return this.Performer.Perform(ActionType.FinishIdle);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
				yield break;
			}

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
		}

		private IEnumerator OnLeave(object sender, int e)
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Leave);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Leave);
				yield break;
			}

			this.Destroy();
			
			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Leave);
		}

		public IEnumerator Run()
		{
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError($"AssWall: Failed to get sex performer for {this.Player.npcID} x {this.Npc.npcID}");
				yield break;
			}

			// Lock place
			SexPlace sexPlace = this.TmpWall.GetComponent<SexPlace>();
			sexPlace.user = Managers.mn.gameMN.player;

			// Disabe UI
			Managers.mn.uiMN.MainCanvasView(false);
			Managers.mn.gameMN.Controlable(false, false);
			Managers.mn.gameMN.pMove.PlayerVisible(false);

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

			Managers.mn.gameMN.Controlable(true, true);
			Managers.mn.gameMN.pMove.PlayerVisible(true);
			Managers.mn.uiMN.MainCanvasView(true);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
		}


		public bool CanContinue()
		{
			return PropPanelManager.Instance.IsOpen();
		}

		public void Destroy()
		{
			this.MenuPanel?.Close();
		}

		public string GetName()
		{
			return AssWall.Name;
		}

		public CommonStates[] GetActors()
		{
			return [this.Player, this.Npc];
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.CommonAnim;
		}

		public string ExpandAnimationName(string originalName)
		{
			return originalName;
		}

		public SexPerformer GetPerformer()
		{
			return this.Performer;
		}
	}
}
