using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace ExtendedHSystem.Scenes
{
	public class ManRapesSleep : IScene
	{
		public readonly CommonStates Girl;

		public readonly CommonStates Man;

		private float NoticeTime = 0f;

		private bool Aborted = false;

		private GameObject TmpSex = null;

		private ManRapeSleepState TmpCommonState = ManRapeSleepState.None;

		private int TmpCommonSub = 0;

		private SkeletonAnimation CommonAnim;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		private readonly ManRapesSleepMenuPanel MenuPanel;

		public ManRapesSleep(CommonStates girl, CommonStates man)
		{
			this.Girl = girl;
			this.Man = man;

			this.MenuPanel = new ManRapesSleepMenuPanel();
			this.MenuPanel.OnForcefullyRapeSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnForcefullyRape(sender, e));
			};
			this.MenuPanel.OnGentlyRapeSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnGentlyRape(sender, e));
			};
			this.MenuPanel.OnUseSleepPowderSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnUseSleepPowder(sender, e));
			};

			this.MenuPanel.OnInsertSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnInsert(sender, e));
			};

			this.MenuPanel.OnSpeedSelected += this.OnSpeed;
			this.MenuPanel.OnSpeed2Selected += this.OnSpeed2;

			this.MenuPanel.OnFinishSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnFinish());
			};

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

		private GameObject GetScene()
		{
			switch (this.Girl.npcID)
			{
				case NpcID.FemaleNative: // 15
					return Managers.mn.sexMN.sexList[1].sexObj[4];

				case NpcID.NativeGirl: // 16
					return Managers.mn.sexMN.sexList[1].sexObj[5];
			}

			return null;
		}
		private void DisableLiveNpc(CommonStates npc)
		{
			NPCMove nMove = npc.nMove;
			nMove.actType = NPCMove.ActType.Wait;
			nMove.movable = false;
			nMove.RBState(false);
			var girlColl = npc.GetComponent<CapsuleCollider>();
			if (girlColl != null)
				girlColl.enabled = false;

			var girlMesh = npc.anim.GetComponent<MeshRenderer>();
			if (girlMesh != null)
				girlMesh.enabled = false;

			Managers.mn.randChar.HandItemHide(npc, true);
		}

		private void EnableLiveNpc(CommonStates npc)
		{
			Managers.mn.randChar.HandItemHide(npc, false);

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = true;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			npc.nMove.movable = true;
		}

		private void SetFinalNpcState(CommonStates npc)
		{
			switch (this.TmpCommonState)
			{
				case ManRapeSleepState.ForcefullRape: // 1
					if (npc.faint > 0.0)
					{
						if (npc.nMove.npcType == NPCMove.NPCType.Friend)
						{
							this.Girl.LoveChange(this.Man, -5f, false);
						}
						else
						{
							npc.nMove.tmpEnemy = this.Man.gameObject;
							npc.nMove.actType = NPCMove.ActType.Chase;
						}
					}
					else
					{
						npc.nMove.common.anim.state.SetAnimation(0, "A_sleep_raped", true);
						npc.nMove.actType = NPCMove.ActType.Dead;
					}
					break;
				case ManRapeSleepState.SleepPowder: // 2
					npc.nMove.actType = NPCMove.ActType.Sleep;
					break;
				case ManRapeSleepState.GentlyRape: // 3
					if (this.TmpCommonSub == 5)
					{
						if (npc.nMove.npcType == NPCMove.NPCType.Friend)
						{
							this.Girl.LoveChange(this.Man, -10f, false);
						}
						else
						{
							npc.nMove.tmpEnemy = this.Man.gameObject;
							npc.nMove.actType = NPCMove.ActType.Chase;
						}
					}
					else
					{
						npc.nMove.actType = NPCMove.ActType.Sleep;
					}
					break;
				default:
					npc.nMove.actType = NPCMove.ActType.Sleep;
					break;
			}
		}

		private void DisableLivePlayer(CommonStates player)
		{
			MeshRenderer playerMesh = player.anim.GetComponent<MeshRenderer>();
			playerMesh.enabled = false;

			Managers.mn.randChar.HandItemHide(player, true);

		}

		private void EnableLivePlayer(CommonStates player)
		{
			Managers.mn.randChar.HandItemHide(player, false);

			MeshRenderer playerMesh = player.anim.GetComponent<MeshRenderer>();
			playerMesh.enabled = true;
		}

		private bool HasSleepPowder()
		{
			return Managers.mn.inventory.HaveItemCheck("sleeppowder_01") != -1;
		}

		private void ConsumeSleepPowder()
		{
			var slotId = Managers.mn.inventory.HaveItemCheck("sleeppowder_01");
			if (slotId != -1)
				Managers.mn.inventory.ConsumeItem(slotId, 1);
		}

		private bool SetupScene()
		{
			var pos = this.Girl.transform.position;
			var scene = this.GetScene();
			if (scene == null)
				return false;

			this.TmpSex = GameObject.Instantiate(scene, pos, Quaternion.identity);
			if (this.TmpSex == null)
				return false;

			Managers.mn.randChar.SetCharacter(this.TmpSex, this.Girl, this.Man);

			this.TmpCommonState = ManRapeSleepState.None;
			this.TmpCommonSub = 0;

			Managers.mn.uiMN.MainCanvasView(false);

			this.MenuPanel.Open(pos);
			this.MenuPanel.ShowInitialMenu(this.HasSleepPowder());

			this.DisableLiveNpc(this.Girl);
			this.DisableLivePlayer(this.Man);

			return true;
		}

		private IEnumerator OnForcefullyRape(object sender, int e)
		{
			foreach (var handler in this.EventHandlers) {
				foreach (var x in handler.OnSleepRapeTypeChange(this, ManRapeSleepState.ForcefullRape))
					yield return x;
			}

			this.TmpCommonState = ManRapeSleepState.ForcefullRape; // 1
			this.Controller.LoopAnimation(this, this.CommonAnim, "A_rapes_idle");

			if (this.Girl != null)
				Managers.mn.sound.GoVoice(this.Girl.voiceID, "close", this.CommonAnim.transform.position);

			this.MenuPanel.ShowRapeMenu();
		}

		private IEnumerator OnGentlyRape(object sender, int e)
		{
			foreach (var handler in this.EventHandlers) {
				foreach (var x in handler.OnSleepRapeTypeChange(this, ManRapeSleepState.GentlyRape))
					yield return x;
			}

			this.TmpCommonState = ManRapeSleepState.GentlyRape; // 3
			this.Controller.LoopAnimation(this, this.CommonAnim, "A_sleep_idle");

			this.MenuPanel.ShowRapeMenu();
		}

		private IEnumerator OnUseSleepPowder(object sender, int e)
		{
			this.ConsumeSleepPowder();

			foreach (var handler in this.EventHandlers) {
				foreach (var x in handler.OnSleepRapeTypeChange(this, ManRapeSleepState.SleepPowder))
					yield return x;
			}

			this.TmpCommonState = ManRapeSleepState.SleepPowder; // 2
			this.Controller.LoopAnimation(this, this.CommonAnim, "A_sleep_idle");

			this.MenuPanel.ShowRapeMenu();
		}

		private IEnumerator OnInsert(object sender, int e)
		{
			this.MenuPanel.Hide();

			this.TmpCommonSub = 1;
			string animName = this.TmpCommonState == ManRapeSleepState.ForcefullRape ? "A_rapes_start" : "A_sleep_start";

			bool canContinue = false;
			foreach (var x in this.Controller.PlayOnceStep(this, this.CommonAnim, animName))
			{
				if (x is bool b)
					canContinue = b;
				yield return x;
			}

			this.Aborted = !canContinue;

			if (!this.CanContinue())
				yield break;

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnRape(this.Man, this.Girl))
					yield return x;
			}

			if (!this.CanContinue())
				yield break;

			animName = this.TmpCommonState == ManRapeSleepState.ForcefullRape ? "A_rapes_loop_01" : "A_sleep_loop_01";
			this.Controller.LoopAnimation(this, this.CommonAnim, animName);

			this.MenuPanel.Show();
			this.MenuPanel.ShowInsertMenu(this.TmpCommonState == ManRapeSleepState.SleepPowder);
		}

		private void OnSpeed(object sender, int e)
		{
			if (this.TmpCommonState == ManRapeSleepState.ForcefullRape)
			{
				if (this.CommonAnim.state.GetCurrent(0).Animation.Name != "A_rapes_loop_02")
					this.Controller.LoopAnimation(this, this.CommonAnim, "A_rapes_loop_02");
				else
					this.Controller.LoopAnimation(this, this.CommonAnim, "A_rapes_loop_01");
			}
			else if (this.CommonAnim.state.GetCurrent(0).Animation.Name != "A_sleep_loop_02")
			{
				this.Controller.LoopAnimation(this, this.CommonAnim, "A_sleep_loop_02");
			}
			else
			{
				this.Controller.LoopAnimation(this, this.CommonAnim, "A_sleep_loop_01");
			}
		}

		private void OnSpeed2(object sender, int e)
		{
			if (this.CommonAnim.state.GetCurrent(0).Animation.Name == "A_sleep_loop_03")
				this.Controller.LoopAnimation(this, this.CommonAnim, "A_sleep_loop_01");
			else
				this.Controller.LoopAnimation(this, this.CommonAnim, "A_sleep_loop_03");
		}

		private IEnumerator OnFinish()
		{
			this.MenuPanel.Hide();

			string animName;
			if (this.TmpCommonState == ManRapeSleepState.ForcefullRape)
			{
				this.Girl.faint = 0.0;
				animName = "A_rapes_finish";
			}
			else
			{
				animName = "A_sleep_finish";
			}

			bool canContinue = false;
			foreach (var x in this.Controller.PlayOnceStep(this, this.CommonAnim, animName))
			{
				if (x is bool b)
					canContinue = b;
				yield return x;
			}

			this.Aborted = !canContinue;
			if (!this.CanContinue())
				yield break;

			if (this.TmpCommonState == ManRapeSleepState.ForcefullRape)
				animName = "A_rapes_finish_idle";
			else
				animName = "A_sleep_finish_idle";

			this.Controller.LoopAnimation(this, this.CommonAnim, animName);

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnCreampie(this.Man, this.Girl))
					yield return x;
			}

			this.TmpCommonSub = 0;

			this.MenuPanel.Show();
			this.MenuPanel.ShowFinishMenu();
		}

		private void OnLeave(object sender, int e)
		{
			this.Destroy();
		}

		private void WakeupCheck()
		{
			if (this.TmpCommonState == ManRapeSleepState.GentlyRape && this.TmpCommonSub >= 1)
			{
				if (this.TmpCommonSub == 2)
				{
					this.NoticeTime += Time.deltaTime * 2f;
				}
				else
				{
					this.NoticeTime += Time.deltaTime;
				}
				if (this.NoticeTime >= 5f)
				{
					if (UnityEngine.Random.Range(0, 100) <= 10)
					{
						Managers.mn.uiMN.GoLogText(Managers.mn.textMN.logTexts[10].Replace("XXXX", this.Girl.charaName));
						this.Aborted = true;
						this.TmpCommonSub = 5;
					}
					this.NoticeTime = 0f;
				}
			}
		}

		public IEnumerator Run()
		{
			if (!this.SetupScene())
				yield break;

			this.CommonAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			this.CommonAnim.state.SetAnimation(0, "A_idle", true);

			while (this.CanContinue())
			{
				this.WakeupCheck();
				yield return null;
			}

			this.EnableLiveNpc(this.Girl);
			this.EnableLivePlayer(this.Man);
			this.SetFinalNpcState(this.Girl);

			if (this.TmpSex != null)
				UnityEngine.Object.Destroy(this.TmpSex);

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.AfterManRape(this.Man, this.Girl))
					yield return x;
			}

			this.MenuPanel.Close();
			Managers.mn.uiMN.MainCanvasView(true);
		}


		public bool CanContinue()
		{
			return this.TmpSex != null && !Input.GetKeyDown(KeyCode.R) && this.Man.life > 0 && !this.Aborted;
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}
	}
}