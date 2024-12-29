using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class Daruma : IScene
	{
		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpDaruma;

		private GameObject SexObj;

		private GameObject DarumaObj;

		private string Loop01Anim;

		private string Loop02Anim;

		private string FinishAnim;

		private string FinishIdleAnim;

		private bool InsertCounted = false;

		private SkeletonAnimation CommonAnim;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		private readonly DarumaMenuPanel MenuPanel;

		public Daruma(CommonStates playerCommon, CommonStates npcCommon, InventorySlot tmpDaruma = null)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.TmpDaruma = tmpDaruma;

			this.MenuPanel = new DarumaMenuPanel();
			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnInsert());
			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnSpeed());
			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnFinish());
			this.MenuPanel.OnStopSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnStop());
			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnLeave());
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
		}

		public void AddEventHandler(SceneEventHandler handler)
		{
			this.EventHandlers.Add(handler);
		}

		private IEnumerator OnInsert()
		{
			// Note: v0.2.3 counts many times, but this seems inconsistent with other scenes, so we will count once.
			if (!this.InsertCounted)
			{
				this.InsertCounted = true;

				// Note: This assumes the player is always a male and the NPC is always a female,
				// which is true as of beta 0.2.3
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnRape(this.Player, this.Npc))
						yield return x;
				}
			}

			this.MenuPanel.ShowInsertMenu();

			if (this.CommonAnim.skeleton.Data.FindAnimation(this.Loop01Anim) != null)
				yield return this.Controller.LoopAnimation(this, this.CommonAnim, this.Loop01Anim);
			else
				PLogger.LogError("no loop01 animation");
		}

		private IEnumerator OnSpeed()
		{
			var currentAnim = this.CommonAnim.state.GetCurrent(0).Animation.Name;
			var newAnim = currentAnim == this.Loop01Anim ? this.Loop02Anim : this.Loop01Anim;

			if (this.CommonAnim.skeleton.Data.FindAnimation(newAnim) != null)
				yield return this.Controller.LoopAnimation(this, this.CommonAnim, newAnim);
			else
				PLogger.LogError($"no '{newAnim}' animation");
		}

		private IEnumerator OnStop()
		{
			this.MenuPanel.ShowStopMenu();
			yield return this.Controller.LoopAnimation(this, this.CommonAnim, "A_idle");
		}

		private IEnumerator OnFinish()
		{
			this.MenuPanel.Hide();

			IEnumerable animController = null;
			if (this.CommonAnim.skeleton.Data.FindAnimation(this.FinishAnim) != null)
				animController = this.Controller.PlayOnceStep(this, this.CommonAnim, this.FinishAnim);
			else if (this.CommonAnim.skeleton.Data.FindAnimation("A_Finish") != null)
				animController = this.Controller.PlayOnceStep(this, this.CommonAnim, "A_Finish");
			else
				PLogger.LogError("no Finish animation");

			if (animController != null)
			{
				foreach (var x in animController)
					yield return x;
			}

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnBusted(this.Player, this.Npc, 0))
					yield return x;
			}

			if (!this.CanContinue())
				yield break;

			if (this.CommonAnim.skeleton.Data.FindAnimation(this.FinishIdleAnim) != null)
				yield return this.Controller.LoopAnimation(this, this.CommonAnim, this.FinishIdleAnim);
			else if (this.CommonAnim.skeleton.Data.FindAnimation("A_Finish_idle") != null)
				yield return this.Controller.LoopAnimation(this, this.CommonAnim, "A_Finish_idle");
			else
				PLogger.LogError("no finish idle animation");

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();
		}

		private IEnumerator OnLeave()
		{
			this.Destroy();
			yield break;
		}

		private GameObject GetScene()
		{
			GameObject scene = null;

			switch (this.Npc.npcID)
			{
				case NpcID.FemaleNative:
					scene = Managers.mn.sexMN.sexList[1].sexObj[13];
					break;

				case NpcID.NativeGirl:
					scene = Managers.mn.sexMN.sexList[1].sexObj[14];
					break;
			}

			return scene;
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

			string suffix = this.Npc.parameters[6].ToString("00");
			this.Loop01Anim = "A_Loop_01_" + suffix;
			this.Loop02Anim = "A_Loop_02_" + suffix;
			this.FinishAnim = "A_Finish_" + suffix;
			this.FinishIdleAnim = "A_Finish_idle_" + suffix;

			var scene = this.GetScene();
			if (scene == null)
				return false;

			this.SexObj = GameObject.Instantiate(scene, this.DarumaObj.transform.position, Quaternion.identity);
			this.CommonAnim = SexObj.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			Managers.mn.randChar.SetCharacter(this.SexObj, this.Npc, Managers.mn.gameMN.playerCommons[1]);

			return true;
		}

		// int state, InventorySlot tmpDaruma = null
		public IEnumerator Run()
		{
			// Scene setup
			if (!this.SetupScene())
				yield break;

			Managers.mn.uiMN.MainCanvasView(false);
			Managers.mn.gameMN.Controlable(false, true);
			Managers.mn.gameMN.pMove.PlayerVisible(false);

			yield return this.Controller.LoopAnimation(this, this.CommonAnim, "A_idle");

			this.MenuPanel.Open(this.TmpDaruma.transform.position);
			this.MenuPanel.ShowInitialMenu();

			while (this.CanContinue())
				yield return null;

			if (this.SexObj != null)
				Object.Destroy(this.SexObj);

			if (this.DarumaObj != null)
				this.DarumaObj.SetActive(true);

			foreach (var handler in this.EventHandlers) {
				foreach (var x in handler.AfterSex(this, this.Player, this.Npc))
					yield return x;
			}

			Managers.mn.gameMN.Controlable(true, true);
			Managers.mn.gameMN.pMove.PlayerVisible(true);
			Managers.mn.uiMN.MainCanvasView(true);
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
