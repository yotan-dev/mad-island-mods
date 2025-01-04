using System.Collections;
using System.Collections.Generic;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class Slave : IScene, IScene2
	{
		public static readonly string Name = "Slave";

		private static readonly Dictionary<string, int> ItemKeyToNpcID = new Dictionary<string, int>()
		{
			{ "slave_giant_01", NpcID.Giant /* 110 */ },
			{ "slave_shino_01", NpcID.Shino /* 114 */ },
			{ "slave_sally_01", NpcID.Sally /* 115 */ },
		};

		public readonly CommonStates Player;


		public readonly InventorySlot TmpSlave;
		
		private CommonStates Npc;

		private SkeletonAnimation CommonAnim;

		private GameObject SexObject;

		private readonly SlaveMenuPanel MenuPanel;

		private ISceneController Controller;

		private SexPerformer Performer;

		private string ItemKey;

		public Slave(CommonStates player, InventorySlot tmpSlave)
		{
			this.Player = player;
			this.TmpSlave = tmpSlave;

			ItemInfo component = this.TmpSlave?.GetComponent<ItemInfo>();
			this.ItemKey = component?.itemKey ?? "";
			
			this.Npc = null;
			if (ItemKeyToNpcID.TryGetValue(this.ItemKey, out var npcId))
				this.Npc = CommonUtils.MakeTempCommon(npcId);

			this.MenuPanel = new SlaveMenuPanel();
			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnInsert());
			this.MenuPanel.OnMoveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnMove());
			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnSpeed());
			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnFinish());
			this.MenuPanel.OnStopSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnStop());
			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnLeave());
		}

		~Slave()
		{
			if (this.Npc != null)
			{
				Object.Destroy(this.Npc);
				this.Npc = null;
			}
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			this.Controller.SetScene(this);
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

		public IEnumerator Run()
		{
			if (!this.SetupScene())
			{
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

			this.Teardown();

			this.EnablePlayer();
			Managers.mn.uiMN.MainCanvasView(true);
		}

		public bool CanContinue()
		{
			// Managers.mn.uiMN.propActProgress == 5
			return PropPanelManager.Instance.IsOpen();
		}

		public void Destroy()
		{
			this.MenuPanel?.Close();
		}

		public string GetName()
		{
			return Slave.Name;
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
