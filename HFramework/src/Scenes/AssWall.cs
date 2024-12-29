using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using YotanModCore;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class AssWall : IScene
	{
		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpWall;

		private bool InsertCounted = false;

		private SkeletonAnimation CommonAnim;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

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
		}

		public void AddEventHandler(SceneEventHandler handler)
		{
			this.EventHandlers.Add(handler);
		}

		private IEnumerator OnInsert(object sender, int e)
		{
			this.MenuPanel.ShowInsertMenu();

			string animSt = this.Npc.npcID.ToString("") + "_";
			yield return this.Controller.LoopAnimation(this, this.CommonAnim, animSt + "A_Loop_01");

			if (!this.InsertCounted)
			{
				this.InsertCounted = true;

				// Note: This assumes the player is always a male and the NPC is always a female,
				// which is true as of beta 0.2.3
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnToilet(this.Player, this.Npc))
						yield return x;
				}
			}
		}

		private IEnumerator OnSpeed(object sender, int e)
		{
			string animSt = this.Npc.npcID.ToString("") + "_";

			if (this.CommonAnim.state.GetCurrent(0).Animation.Name == animSt + "A_Loop_01")
				yield return this.Controller.LoopAnimation(this, this.CommonAnim, animSt + "A_Loop_02");
			else if (this.CommonAnim.state.GetCurrent(0).Animation.Name == animSt + "A_Loop_02")
				yield return this.Controller.LoopAnimation(this, this.CommonAnim, animSt + "A_Loop_01");
		}

		private IEnumerator OnStop(object sender, int e)
		{
			this.MenuPanel.ShowStopMenu();
			yield return this.Controller.LoopAnimation(this, this.CommonAnim, "A_idle");
		}

		private IEnumerator OnFinish(object sender, int e)
		{
			this.MenuPanel.Hide();

			string animSt = this.Npc.npcID.ToString("") + "_";

			foreach (var x in this.Controller.PlayOnceStep(this, this.CommonAnim, animSt + "A_Finish"))
				yield return x;

			if (!this.CanContinue())
				yield break;

			if (this.Npc != null)
			{
				// Note: This assumes the player is always a male and the NPC is always a female,
				// which is true as of beta 0.2.3
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnCreampie(this.Player, this.Npc))
						yield return x;
				}
			}

			yield return this.Controller.LoopAnimation(this, this.CommonAnim, animSt + "A_Finish_idle");

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();
		}

		private IEnumerator OnLeave(object sender, int e)
		{
			this.Destroy();
			yield break;
		}

		public IEnumerator Run()
		{
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
			this.CommonAnim.state.SetAnimation(0, "A_idle", true);

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

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.AfterSex(this, this.Player, this.Npc))
					yield return x;
			}

			yield break;
		}


		public bool CanContinue()
		{
			return PropPanelManager.Instance.IsOpen();
		}

		public void Destroy()
		{
			this.MenuPanel?.Close();
		}
	}
}
