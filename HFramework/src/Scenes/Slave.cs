using System.Collections;
using System.Collections.Generic;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class Slave : BaseScene
	{
		public static readonly string Name = "HF_Slave";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Insert = "Insert";
			public const string Move = "Move";
			public const string Speed = "Speed";
			public const string Finish = "Finish";
			public const string Stop = "Stop";
			public const string Leave = "Leave";
		}

		private static readonly Dictionary<string, int> ItemKeyToNpcID = new Dictionary<string, int>()
		{
			{ "slave_giant_01", NpcID.Giant /* 110 */ },
			{ "slave_shino_01", NpcID.Shino /* 114 */ },
			{ "slave_sally_01", NpcID.Sally /* 115 */ },
		};

		public readonly CommonStates Player;


		public readonly InventorySlot TmpSlave;
		
		private CommonStates Npc;

		private GameObject SexObject;

		private readonly SlaveMenuPanel MenuPanel;

		private string ItemKey;

		public Slave(CommonStates player, InventorySlot tmpSlave) : base(Name)
		{
			this.Player = player;
			this.TmpSlave = tmpSlave;

			ItemInfo component = this.TmpSlave?.GetComponent<ItemInfo>();
			this.ItemKey = component?.itemKey ?? "";
			
			this.Npc = null;
			if (ItemKeyToNpcID.TryGetValue(this.ItemKey, out var npcId))
				this.Npc = CommonUtils.MakeTempCommon(npcId);

			this.MenuPanel = new SlaveMenuPanel();
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
		}

		~Slave()
		{
			if (this.Npc != null)
			{
				Object.Destroy(this.Npc);
				this.Npc = null;
			}
		}

		private IEnumerator OnInsert()
		{
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
				yield break;

			this.MenuPanel.ShowInsertMenu();

			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.ShowInsertMenu();
			this.MenuPanel.Show();
		}

		private IEnumerator OnMove()
		{
			this.MenuPanel.ShowMoveMenu();
			yield return this.Performer.Perform(ActionType.Speed1);
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

		private bool SetupScene()
		{
			if (this.TmpSlave == null)
				return true;

			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError("No performer found");
				return false;
			}
			
			var scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
			if (scene == null)
				return false;

			Vector3 position = this.TmpSlave.transform.position;
			if (this.ItemKey == "slave_sally_01")
				position = this.TmpSlave.transform.Find("Anim").gameObject.transform.position;

			this.SexObject = GameObject.Instantiate(scene, position, Quaternion.identity);
			if (this.SexObject == null)
				return false;

			this.CommonAnim = this.SexObject.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

			Managers.mn.randChar.SetCharacter(this.SexObject, null, this.Player);

			if (this.ItemKey == "slave_giant_01")
			{
				this.CommonAnim.skeleton.SetAttachment("slave_ring", "slave_ring");
				this.CommonAnim.skeleton.SetAttachment("slave_chain", "slave_chain");
				this.CommonAnim.skeleton.SetAttachment("slave_stone", "slave_stone");
			}

			this.TmpSlave.gameObject.SetActive(false);

			return true;
		}

		private void DisablePlayer()
		{
			Managers.mn.gameMN.Controlable(false, true);
			Managers.mn.gameMN.pMove.PlayerVisible(false);
		}

		private void EnablePlayer()
		{
			Managers.mn.gameMN.Controlable(true, true);
			Managers.mn.gameMN.pMove.PlayerVisible(true);
		}

		private void Teardown()
		{
			if (this.SexObject != null)
			{
				Object.Destroy(this.SexObject);
				this.SexObject = null;
			}

			if (this.TmpSlave?.gameObject != null)
				this.TmpSlave.gameObject.SetActive(true);

			if (this.Npc != null)
			{
				Object.Destroy(this.Npc);
				this.Npc = null;
			}
		}

		public override IEnumerator Run()
		{
			if (!this.SetupScene())
			{
				this.Teardown();
				yield break;
			}

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				this.Teardown();
				yield break;
			}

			Managers.mn.uiMN.MainCanvasView(false);
			this.DisablePlayer();

			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.Open(this.TmpSlave.transform.position);
			this.MenuPanel.ShowInitialMenu();

			while (this.CanContinue())
				yield return null;

			yield return this.EndLongLivedStep();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
			this.Teardown();

			this.EnablePlayer();
			Managers.mn.uiMN.MainCanvasView(true);
		}

		public override bool CanContinue()
		{
			// Managers.mn.uiMN.propActProgress == 5
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
