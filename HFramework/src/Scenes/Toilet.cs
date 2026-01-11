using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class Toilet : BaseScene
	{
		public static readonly string Name = "HF_Toilet";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Insert = "Insert";
			public const string Move = "Move";
			public const string Stop = "Stop";
			public const string Speed = "Speed";
			public const string Finish = "Finish";
			public const string Urinate = "Urinate";
			public const string StopUrinate = "StopUrinate";
			public const string Leave = "Leave";
			public const string FaceReveal = "FaceReveal";
		}

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpToilet;

		private readonly SexPlace SexPlace;

		private AudioSource Pee1Audio;

		private AudioSource Pee2Audio;

		private readonly ToiletMenuPanel MenuPanel;

		public Toilet(CommonStates player, CommonStates girl, InventorySlot tmpToilet) : base(Name)
		{
			this.Player = player;
			this.Npc = girl;
			this.TmpToilet = tmpToilet;
			this.SexPlace = this.TmpToilet.GetComponent<SexPlace>();

			this.MenuPanel = new ToiletMenuPanel();
			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Insert, this.OnInsert));
			this.MenuPanel.OnMoveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Move, this.OnMove));
			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Speed, this.OnSpeed));
			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Finish, this.OnFinish));
			this.MenuPanel.OnStopSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Stop, this.OnStop));
			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.RunStep(StepNames.Leave, this.OnLeave));

			this.MenuPanel.OnFaceRevealSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.RunStep(StepNames.FaceReveal, this.OnFaceReveal));

			this.MenuPanel.OnUrinateSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Urinate, this.OnUrinate));
			this.MenuPanel.OnStopUrinateSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.StopUrinate, this.OnStopUrinate));
		}

		private IEnumerator OnInsert()
		{
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.InsertIdle);
			if (!this.CanContinue())
				yield break;

			this.MenuPanel.ShowInsertMenu();
			this.MenuPanel.Show();
		}

		private IEnumerator OnMove()
		{
			if (this.Performer.CurrentAction != ActionType.Speed1 && this.Performer.CurrentAction != ActionType.Speed2)
			{
				this.MenuPanel.ShowMoveMenu();
				yield return this.Performer.Perform(ActionType.Speed1);
			}
		}

		private IEnumerator OnSpeed()
		{
			if (this.Performer.CurrentAction == ActionType.Speed1)
				yield return this.Performer.Perform(ActionType.Speed2);
			else if (this.Performer.CurrentAction == ActionType.Speed2)
				yield return this.Performer.Perform(ActionType.Speed1);
		}

		private IEnumerator OnStop()
		{
			this.MenuPanel.ShowStopMenu();
			yield return this.Performer.Perform(ActionType.InsertIdle);
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

		private IEnumerator OnUrinate()
		{
			if (this.Performer.CurrentAction == ActionType.StartIdle || this.Performer.CurrentAction == ActionType.FinishIdle)
			{
				yield return this.Performer.Perform(ActionType.IdlePee);
				this.MenuPanel.ChangeToStopUrinate();
			}
			else if (this.Performer.CurrentAction == ActionType.InsertIdle)
			{
				yield return this.Performer.Perform(ActionType.InsertPee);
				this.MenuPanel.ChangeToStopUrinate();
			}
		}

		private IEnumerator OnStopUrinate()
		{
			if (this.Performer.CurrentAction == ActionType.IdlePee)
			{
				yield return this.Performer.Perform(ActionType.StartIdle);
				this.MenuPanel.ChangeStopToUrinate();
			}
			else if (this.Performer.CurrentAction == ActionType.InsertPee)
			{
				yield return this.Performer.Perform(ActionType.InsertIdle);
				this.MenuPanel.ChangeStopToUrinate();
			}
		}

		private IEnumerator OnFaceReveal()
		{
			// While official checks for == 1, in-game tests has shown that usually the start value is higher, like "16f", without board,
			// so checking for == 1 would make one click "do nothing" for the next one to work. switching to > 0.99f does the trick
			if (Managers.mn.inventory.itemSlot[50].attack > 0.99f)
				Managers.mn.inventory.itemSlot[50].attack = 0f;
			else
				Managers.mn.inventory.itemSlot[50].attack = 1f;

			Managers.mn.sexMN.ToiletFaceLoad(this.CommonAnim, Managers.mn.inventory.itemSlot[50]);
			yield break;
		}

		private bool SetupScene()
		{
			this.SexPlace.user = Managers.mn.gameMN.player;

			this.Controller.SetMainCanvasVisible(false);
			this.Controller.SetGameControllable(false, false);
			this.Controller.SetPlayerVisible(false);

			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError("No performer found");
				return false;
			}

			this.CommonAnim = Managers.mn.inventory.tmpSubInventory.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim.skeleton.SetSkin("Man");
			this.CommonAnim.skeleton.SetSlotsToSetupPose();

			Managers.mn.sexMN.ToiletFaceLoad(this.CommonAnim, Managers.mn.inventory.itemSlot[50]);
			Managers.mn.randChar.SetCharacter(Managers.mn.inventory.tmpSubInventory.gameObject, this.Npc, this.Player);

			return true;
		}

		private void Teardown()
		{
			this.CommonAnim.skeleton.SetSkin("default");
			this.CommonAnim.skeleton.SetSlotsToSetupPose();
			if (this.TmpToilet != null)
				Managers.mn.sexMN.StartCoroutine(Managers.mn.gameMN.ToiletCheck(this.TmpToilet, 0));

			this.Pee1Audio?.Stop();
			this.Pee2Audio?.Stop();
			this.SexPlace.user = null;

			this.Controller.SetGameControllable(true, true);
			this.Controller.SetPlayerVisible(true);
			this.Controller.SetMainCanvasVisible(true);
		}

		public override IEnumerator Run()
		{
			if (!this.SetupScene())
				yield break;

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				this.Teardown();
				yield break;
			}

			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.Open(this.SexPlace.transform.position);
			this.MenuPanel.ShowInitialMenu();

			while (this.CanContinue())
			{
				if (this.Performer.CurrentAction == ActionType.IdlePee)
				{
					if (this.Pee1Audio == null)
						this.Pee1Audio = Managers.mn.sound.LoopAudio3D(AudioSE.Pee01, this.CommonAnim.transform.position, Managers.mn.sound.soundBaseDist);
				}
				else if (this.Performer.CurrentAction == ActionType.InsertPee)
				{
					if (this.Pee2Audio == null)
						this.Pee2Audio = Managers.mn.sound.LoopAudio3D(AudioSE.Pee02, this.CommonAnim.transform.position, Managers.mn.sound.soundBaseDist);
				}
				else
				{
					if (this.Pee1Audio != null) {
						this.Pee1Audio.Stop();
						this.Pee1Audio = null;
					}
					if (this.Pee2Audio != null) {
						this.Pee2Audio.Stop();
						this.Pee2Audio = null;
					}
				}

				yield return null;
			}

			yield return this.EndLongLivedStep();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);

			this.Teardown();
		}

		public override bool CanContinue()
		{
			// Managers.mn.uiMN.propActProgress == 1
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
