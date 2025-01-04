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
	public class Daruma : IScene, IScene2
	{
		public static readonly string Name = "Daruma";

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpDaruma;

		private GameObject SexObj;

		private GameObject DarumaObj;

		private bool InsertCounted = false;

		private SkeletonAnimation CommonAnim;

		private ISceneController Controller;

		private SexPerformer Performer;

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
			this.Controller.SetScene(this);
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

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnBusted(this.Player, this.Npc, 0))
					yield return x;
			}

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

		public IEnumerator Run()
		{
			// Scene setup
			if (!this.SetupScene())
				yield break;

			Managers.mn.uiMN.MainCanvasView(false);
			Managers.mn.gameMN.Controlable(false, true);
			Managers.mn.gameMN.pMove.PlayerVisible(false);

			yield return this.Performer.Perform(ActionType.StartIdle);

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

		public string GetName()
		{
			return Daruma.Name;
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
			return originalName.Replace("<Tits>", this.Npc.parameters[6].ToString("00"));
		}

		public SexPerformer GetPerformer()
		{
			return this.Performer;
		}
	}
}
