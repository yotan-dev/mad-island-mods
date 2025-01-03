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
	public class Toilet : IScene, IScene2
	{
		public static readonly string Name = "Toilet";

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpToilet;

		private readonly SexPlace SexPlace;

		private SkeletonAnimation Anim;

		private AudioSource Pee1Audio;

		private AudioSource Pee2Audio;

		private readonly ToiletMenuPanel MenuPanel;

		private ISceneController Controller;

		private SexPerformer Performer;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		public Toilet(CommonStates player, CommonStates girl, InventorySlot tmpToilet)
		{
			this.Player = player;
			this.Npc = girl;
			this.TmpToilet = tmpToilet;
			this.SexPlace = this.TmpToilet.GetComponent<SexPlace>();

			this.MenuPanel = new ToiletMenuPanel();
			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnInsert());
			this.MenuPanel.OnMoveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnMove());
			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnSpeed());
			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnFinish());
			this.MenuPanel.OnStopSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnStop());
			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnLeave());

			this.MenuPanel.OnFaceRevealSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnFaceReveal());

			this.MenuPanel.OnUrinateSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnUrinate());
			this.MenuPanel.OnStopUrinateSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnStopUrinate());
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
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Insert);

			if (this.Npc != null)
			{
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnToilet(this.Player, this.Npc))
						yield return x;
				}
			}

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

			if (this.Npc != null)
			{
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnCreampie(this.Player, this.Npc))
						yield return x;
				}
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

		private IEnumerator OnUrinate()
		{
			if (this.Performer.CurrentAction == ActionType.StartIdle)
			{
				yield return this.Performer.Perform(ActionType.IdlePee);
				this.MenuPanel.ChangeToStopUrinate();
			}
			else if (this.Performer.CurrentAction == ActionType.StartIdle || this.Performer.CurrentAction == ActionType.FinishIdle)
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
			}
			else if (this.Performer.CurrentAction == ActionType.InsertPee)
			{
				yield return this.Performer.Perform(ActionType.InsertIdle);
				this.MenuPanel.ChangeStopToUrinate();
			}
		}


		private IEnumerator OnFaceReveal()
		{
			if (Managers.mn.inventory.itemSlot[50].attack == 1f)
				Managers.mn.inventory.itemSlot[50].attack = 0f;
			else
				Managers.mn.inventory.itemSlot[50].attack = 1f;

			Managers.mn.sexMN.ToiletFaceLoad(this.Anim, Managers.mn.inventory.itemSlot[50]);
			yield break;
		}

		private bool SetupScene()
		{
			this.SexPlace.user = Managers.mn.gameMN.player;

			Managers.mn.uiMN.MainCanvasView(false);
			Managers.mn.gameMN.Controlable(false, false);

			Managers.mn.gameMN.pMove.PlayerVisible(false);

			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError("No performer found");
				return false;
			}

			this.Anim = Managers.mn.inventory.tmpSubInventory.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.Anim.skeleton.SetSkin("Man");
			this.Anim.skeleton.SetSlotsToSetupPose();

			Managers.mn.sexMN.ToiletFaceLoad(this.Anim, Managers.mn.inventory.itemSlot[50]);
			Managers.mn.randChar.SetCharacter(Managers.mn.inventory.tmpSubInventory.gameObject, this.Npc, this.Player);

			return true;
		}

		private void Teardown()
		{
			this.Anim.skeleton.SetSkin("default");
			this.Anim.skeleton.SetSlotsToSetupPose();
			if (this.TmpToilet != null)
				Managers.mn.sexMN.StartCoroutine(Managers.mn.gameMN.ToiletCheck(this.TmpToilet, 0));

			this.Pee1Audio?.Stop();
			this.Pee2Audio?.Stop();
			this.SexPlace.user = null;

			Managers.mn.gameMN.Controlable(true, true);
			Managers.mn.gameMN.pMove.PlayerVisible(true);
			Managers.mn.uiMN.MainCanvasView(true);
		}

		public IEnumerator Run()
		{
			this.SetupScene();

			yield return this.Performer.Perform(ActionType.StartIdle);

			this.MenuPanel.Open(this.SexPlace.transform.position);
			this.MenuPanel.ShowInitialMenu();

			this.Pee1Audio = Managers.mn.sound.LoopAudio3D(AudioTrack.Pee1, this.Anim.transform.position, Managers.mn.sound.soundBaseDist);
			this.Pee1Audio.Pause();

			this.Pee2Audio = Managers.mn.sound.LoopAudio3D(AudioTrack.Pee2, this.Anim.transform.position, Managers.mn.sound.soundBaseDist);
			this.Pee2Audio.Pause();

			while (this.CanContinue())
			{
				if (this.Performer.CurrentAction == ActionType.IdlePee)
				{
					if (!this.Pee1Audio.isPlaying)
						this.Pee1Audio.UnPause();
				}
				else if (this.Performer.CurrentAction == ActionType.InsertPee)
				{
					if (!this.Pee2Audio.isPlaying)
						this.Pee2Audio.UnPause();
				}
				else
				{
					if (this.Pee1Audio.isPlaying)
						this.Pee1Audio.Pause();
					if (this.Pee2Audio.isPlaying)
						this.Pee2Audio.Pause();
				}

				yield return null;
			}

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.AfterSex(this, this.Player, this.Npc))
					yield return x;
			}

			this.Teardown();
		}

		public bool CanContinue()
		{
			// Managers.mn.uiMN.propActProgress == 1
			return PropPanelManager.Instance.IsOpen();
		}

		public void Destroy()
		{
			this.MenuPanel?.Close();
		}

		public string GetName()
		{
			return Toilet.Name;
		}

		public CommonStates[] GetActors()
		{
			return [this.Player, this.Npc];
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.Anim;
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
