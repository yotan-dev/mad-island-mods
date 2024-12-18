using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Extensions;
using YotanModCore.PropPanels;

namespace ExtendedHSystem.Scenes
{
	public class Slave : IScene
	{
		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpSlave;

		private string TmpSexType = "";

		private int TmpCommonState = 0;

		private SkeletonAnimation CommonAnim;

		private GameObject SexObject;

		private bool RapeCounted = false;

		private readonly SlaveMenuPanel MenuPanel;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		public Slave(CommonStates player, InventorySlot tmpSlave)
		{
			this.Player = player;
			this.TmpSlave = tmpSlave;

			this.MenuPanel = new SlaveMenuPanel();
			this.MenuPanel.OnInsertSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnInsert(sender, e));
			};
			this.MenuPanel.OnMoveSelected += this.OnMove;
			this.MenuPanel.OnSpeedSelected += this.OnSpeed;
			this.MenuPanel.OnFinishSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnFinish(sender, e));
			};
			this.MenuPanel.OnStopSelected += this.OnStop;
			this.MenuPanel.OnLeaveSelected += this.OnLeave;
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
		}

		public void AddEventHandler(SceneEventHandler handler)
		{
			this.EventHandlers.Add(handler);
		}

		private IEnumerator OnInsert(object sender, int e)
		{
			this.MenuPanel.Hide();

			IEnumerable startAnimHandler = null;
			if (this.CommonAnim.skeleton.Data.FindAnimation(this.TmpSexType + "start") != null)
				startAnimHandler = this.Controller.PlayOnceStep(this, this.CommonAnim, this.TmpSexType + "start");
			else if (this.CommonAnim.skeleton.Data.FindAnimation(this.TmpSexType + "Start") != null)
				startAnimHandler = this.Controller.PlayOnceStep(this, this.CommonAnim, this.TmpSexType + "Start");

			if (startAnimHandler != null)
			{
				foreach (var x in startAnimHandler)
					yield return x;
			}

			CommonStates to = Managers.mn.npcMN.MakeTempCommon(this.TmpCommonState);

			// While original game doesn't do that, it is weird to count several times
			if (!this.RapeCounted)
			{
				foreach (var handler in this.EventHandlers)
				{
					// Note: This assumes the player is always a male and the NPC is always a female,
					// which is true as of beta 0.2.4
					foreach (var x in handler.OnRape(this, this.Player, to))
						yield return x;
				}

				this.RapeCounted = true;
			}

			if (!this.CanContinue())
				yield break;

			this.MenuPanel.ShowInsertMenu();
			if (this.CommonAnim.skeleton.Data.FindAnimation(this.TmpSexType + "start_idle") != null)
				this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "start_idle");
			else if (this.CommonAnim.skeleton.Data.FindAnimation(this.TmpSexType + "Start_idle") != null)
				this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "Start_idle");

			this.MenuPanel.ShowInsertMenu();
			this.MenuPanel.Show();

			Managers.mn.uiMN.PropPanelStateChange(0, 5, 4, true);
			Managers.mn.uiMN.PropPanelStateChange(1, 3, 3, true);
		}

		private void OnMove(object sender, int e)
		{
			this.MenuPanel.ShowMoveMenu();
			this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "Loop_01");
		}

		private void OnSpeed(object sender, int e)
		{
			if (this.CommonAnim.GetCurrentAnimName() == this.TmpSexType + "Loop_01")
				this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "Loop_02");
			else if (this.CommonAnim.GetCurrentAnimName() == this.TmpSexType + "Loop_02")
				this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "Loop_01");
		}

		private void OnStop(object sender, int e)
		{
			this.MenuPanel.ShowStopMenu();

			if (this.CommonAnim.HasAnimation(this.TmpSexType + "start_idle"))
				this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "start_idle");
			else if (this.CommonAnim.HasAnimation(this.TmpSexType + "Start_idle"))
				this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "Start_idle");
		}

		private IEnumerator OnFinish(object sender, int e)
		{
			this.MenuPanel.Hide();

			foreach (var x in this.Controller.PlayOnceStep(this, this.CommonAnim, this.TmpSexType + "Finish"))
				yield return x;

			CommonStates to = Managers.mn.npcMN.MakeTempCommon(this.TmpCommonState);
			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnBusted(this.Player, to, 0))
					yield return x;
			}

			if (!this.CanContinue())
				yield break;

			this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "Finish_idle");

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();
		}

		private void OnLeave(object sender, int e)
		{
			this.Destroy();
		}

		private bool GetScene(string itemKey, out GameObject scene, out int commonState, out string sexType)
		{
			if (itemKey == "slave_giant_01")
			{
				sexType = "Rape_A_";
				commonState = 110;
				scene = Managers.mn.sexMN.sexList[1].sexObj[7];
			}
			else if (itemKey == "slave_shino_01")
			{
				sexType = "Rape_A_";
				commonState = 114;
				scene = Managers.mn.sexMN.sexList[1].sexObj[19];
			}
			else if (itemKey == "slave_sally_01")
			{
				sexType = "Rapes2_A_";
				commonState = 115;
				scene = Managers.mn.sexMN.sexList[11].sexObj[0];
			}
			else
			{
				sexType = "";
				scene = null;
				commonState = 0;
			}

			return true;
		}

		private bool SetupScene()
		{
			if (this.TmpSlave == null)
				return true;

			ItemInfo component = this.TmpSlave.GetComponent<ItemInfo>();
			string itemKey = component.itemKey;
			if (!this.GetScene(component.itemKey, out GameObject scene, out this.TmpCommonState, out this.TmpSexType))
				return false;

			if (scene == null)
				return false;

			Vector3 position = this.TmpSlave.transform.position;
			if (itemKey == "slave_sally_01")
				position = this.TmpSlave.transform.Find("Anim").gameObject.transform.position;

			this.SexObject = GameObject.Instantiate(scene, position, Quaternion.identity);
			if (this.SexObject == null)
				return false;

			this.CommonAnim = this.SexObject.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

			Managers.mn.randChar.SetCharacter(this.SexObject, null, this.Player);

			if (itemKey == "slave_giant_01")
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

			this.Controller.LoopAnimation(this, this.CommonAnim, this.TmpSexType + "idle");

			this.MenuPanel.Open(this.TmpSlave.transform.position);
			this.MenuPanel.ShowInitialMenu();

			while (this.CanContinue())
				yield return null;

			this.Teardown();

			// MakeTempCommon is quite unsafe... but it is fine as long as it doesn't take 5 seconds to run
			var to = Managers.mn.npcMN.MakeTempCommon(this.TmpCommonState);
			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.AfterSex(this, this.Player, to))
					yield return x;
			}

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
	}
}