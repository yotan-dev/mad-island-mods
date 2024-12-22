using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;
using YotanModCore.PropPanels;

namespace ExtendedHSystem.Scenes
{
	public class Toilet : IScene
	{
		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly InventorySlot TmpToilet;

		private readonly SexPlace SexPlace;

		private SkeletonAnimation Anim;

		private AudioSource Pee1Audio;

		private AudioSource Pee2Audio;

		private readonly ToiletMenuPanel MenuPanel;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		public Toilet(CommonStates player, CommonStates girl, InventorySlot tmpToilet)
		{
			this.Player = player;
			this.Npc = girl;
			this.TmpToilet = tmpToilet;
			this.SexPlace = this.TmpToilet.GetComponent<SexPlace>();

			this.MenuPanel = new ToiletMenuPanel();
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

			this.MenuPanel.OnFaceRevealSelected += this.OnFaceReveal;

			this.MenuPanel.OnUrinateSelected += this.OnUrinate;
			this.MenuPanel.OnStopUrinateSelected += this.OnStopUrinate;
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

			foreach (var x in this.Controller.PlayOnceStep(this, this.Anim, "A_Start"))
				yield return x;

			if (!this.CanContinue())
				yield break;

			if (this.Npc != null)
			{
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnToilet(this.Player, this.Npc))
						yield return x;
				}
			}

			this.Controller.LoopAnimation(this, this.Anim, "A_Start_idle");
			this.MenuPanel.ShowInsertMenu();
			this.MenuPanel.Show();
		}

		private void OnMove(object sender, int e)
		{
			if (this.Anim.state.GetCurrent(0).Animation.Name != "A_Loop_01" && this.Anim.state.GetCurrent(0).Animation.Name != "A_Loop_02")
			{
				this.MenuPanel.ShowMoveMenu();
				this.Controller.LoopAnimation(this, this.Anim, "A_Loop_01");
			}
		}

		private void OnSpeed(object sender, int e)
		{
			if (this.Anim.GetCurrentAnimName() == "A_Loop_01")
				this.Controller.LoopAnimation(this, this.Anim, "A_Loop_02");
			else if (this.Anim.GetCurrentAnimName() == "A_Loop_02")
				this.Controller.LoopAnimation(this, this.Anim, "A_Loop_01");
		}

		private void OnStop(object sender, int e)
		{
			this.MenuPanel.ShowStopMenu();
			this.Anim.state.SetAnimation(0, "A_Start_idle", true);
		}

		private IEnumerator OnFinish(object sender, int e)
		{
			this.MenuPanel.Hide();

			foreach (var x in this.Controller.PlayOnceStep(this, this.Anim, "A_Finish"))
				yield return x;

			if (!this.CanContinue())
				yield break;

			if (this.Npc != null)
			{
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnCreampie(this.Player, this.Npc))
						yield return x;
				}
			}

			this.Controller.LoopAnimation(this, this.Anim, "A_Finish_idle");

			this.MenuPanel.ShowFinishMenu();
			this.MenuPanel.Show();
		}

		private void OnLeave(object sender, int e)
		{
			this.Destroy();
		}

		private void OnUrinate(object sender, int e)
		{
			string currentAnim = this.Anim.GetCurrentAnimName();
			if (currentAnim == "A_Start_idle")
			{
				this.Controller.LoopAnimation(this, this.Anim, "A_Start_pee");
				this.MenuPanel.ChangeToStopUrinate();
			}
			else if (currentAnim == "A_idle2" || currentAnim == "A_Finish_idle")
			{
				this.Controller.LoopAnimation(this, this.Anim, "A_idle_pee");
				this.MenuPanel.ChangeToStopUrinate();
			}
		}

		private void OnStopUrinate(object sender, int e)
		{
			string currentAnim = this.Anim.GetCurrentAnimName();
			if (currentAnim == "A_idle_pee")
			{
				this.Controller.LoopAnimation(this, this.Anim, "A_idle2");
				this.MenuPanel.ChangeStopToUrinate();
			}
			else if (currentAnim == "A_Start_pee")
			{
				this.Controller.LoopAnimation(this, this.Anim, "A_Start_idle");
				this.MenuPanel.ChangeStopToUrinate();
			}
		}


		private void OnFaceReveal(object sender, int e)
		{
			if (Managers.mn.inventory.itemSlot[50].attack == 1f)
				Managers.mn.inventory.itemSlot[50].attack = 0f;
			else
				Managers.mn.inventory.itemSlot[50].attack = 1f;

			Managers.mn.sexMN.ToiletFaceLoad(this.Anim, Managers.mn.inventory.itemSlot[50]);
		}

		private bool SetupScene()
		{
			this.SexPlace.user = Managers.mn.gameMN.player;

			Managers.mn.uiMN.MainCanvasView(false);
			Managers.mn.gameMN.Controlable(false, false);

			Managers.mn.gameMN.pMove.PlayerVisible(false);

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

			this.Controller.LoopAnimation(this, this.Anim, "A_idle2");

			this.MenuPanel.Open(this.SexPlace.transform.position);
			this.MenuPanel.ShowInitialMenu();

			this.Pee1Audio = Managers.mn.sound.LoopAudio3D(AudioTrack.Pee1, this.Anim.transform.position, Managers.mn.sound.soundBaseDist);
			this.Pee1Audio.Pause();

			this.Pee2Audio = Managers.mn.sound.LoopAudio3D(AudioTrack.Pee2, this.Anim.transform.position, Managers.mn.sound.soundBaseDist);
			this.Pee2Audio.Pause();

			while (this.CanContinue())
			{
				string currentAnim = this.Anim.GetCurrentAnimName();
				if (currentAnim == "A_idle_pee")
				{
					if (!this.Pee1Audio.isPlaying)
						this.Pee1Audio.UnPause();
				}
				else if (currentAnim == "A_Start_pee")
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
	}
}