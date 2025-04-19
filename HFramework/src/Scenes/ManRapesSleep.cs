using System.Collections;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;

namespace HFramework.Scenes
{
	public class ManRapesSleep : BaseScene
	{
		public static readonly string Name = "HF_ManRapesSleep";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string ForceRape = "ForceRape";
			public const string GentlyRape = "GentlyRape";
			public const string PowderRape = "PowderRape";
			public const string Insert = "Insert";
			public const string Speed = "Speed";
			public const string Speed2 = "Speed2";
			public const string Finish = "Finish";
			public const string Leave = "Leave";
		}

		public readonly CommonStates Girl;

		public readonly CommonStates Man;

		private float NoticeTime = 0f;

		private GameObject TmpSex = null;

		private ManRapeSleepState TmpCommonState = ManRapeSleepState.None;

		private int TmpCommonSub = 0;

		private readonly ManRapesSleepMenuPanel MenuPanel;

		public ManRapesSleep(CommonStates girl, CommonStates man)
			: base(Name)
		{
			this.Girl = girl;
			this.Man = man;

			this.MenuPanel = new ManRapesSleepMenuPanel();
			this.MenuPanel.OnForcefullyRapeSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.ForceRape, this.OnForcefullyRape));
			this.MenuPanel.OnGentlyRapeSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.GentlyRape, this.OnGentlyRape));
			this.MenuPanel.OnUseSleepPowderSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.PowderRape, this.OnUseSleepPowder));

			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Insert, this.OnInsert));

			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Speed, this.OnSpeed));
			this.MenuPanel.OnSpeed2Selected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Speed2, this.OnSpeed2));

			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.StartLongLivedStep(StepNames.Finish, this.OnFinish));

			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(
				this.RunStep(StepNames.Leave, this.OnLeave));
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

			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			var scene = this.Performer?.Info?.SexPrefabSelector?.GetPrefab() ?? null;
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
			var shouldShowSleepPowderMenu = this.HasSleepPowder() && this.Performer.HasSet("PowderRape");
			this.MenuPanel.ShowInitialMenu(shouldShowSleepPowderMenu);

			this.DisableLiveNpc(this.Girl);
			this.DisableLivePlayer(this.Man);

			return true;
		}

		private IEnumerator OnForcefullyRape()
		{
			this.TmpCommonState = ManRapeSleepState.ForcefullRape; // 1
			
			yield return this.Performer.ChangeSet("ForceRape");

			if (this.Girl != null)
				Managers.mn.sound.GoVoice(this.Girl.voiceID, "close", this.CommonAnim.transform.position);

			this.MenuPanel.ShowRapeMenu();
		}

		private IEnumerator OnGentlyRape()
		{
			this.TmpCommonState = ManRapeSleepState.GentlyRape; // 3
			yield return this.Performer.ChangeSet("GentlyRape");

			this.MenuPanel.ShowRapeMenu();
		}

		private IEnumerator OnUseSleepPowder()
		{
			this.ConsumeSleepPowder();
			this.TmpCommonState = ManRapeSleepState.SleepPowder; // 2

			yield return this.Performer.ChangeSet("PowderRape");

			this.MenuPanel.ShowRapeMenu();
		}

		private IEnumerator OnInsert()
		{
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Speed1);
			this.TmpCommonSub = 1;

			this.MenuPanel.Show();
			this.MenuPanel.ShowInsertMenu(this.TmpCommonState == ManRapeSleepState.SleepPowder);
		}

		private IEnumerator OnSpeed()
		{
			if (this.Performer.CurrentAction == ActionType.Speed2) {
				if (this.TmpCommonState != ManRapeSleepState.ForcefullRape)
					this.TmpCommonSub = 2;
				yield return this.Performer.Perform(ActionType.Speed1);
			} else {
				if (this.TmpCommonState != ManRapeSleepState.ForcefullRape)
					this.TmpCommonSub = 1;
				yield return this.Performer.Perform(ActionType.Speed2);
			}
		}

		private IEnumerator OnSpeed2()
		{
			if (this.Performer.HasAction(ActionType.Speed3))
				yield return this.Performer.Perform(ActionType.Speed3);
			else
				yield return this.Performer.Perform(ActionType.Speed1);
		}

		private IEnumerator OnFinish()
		{
			this.MenuPanel.Hide();
			this.TmpCommonSub = 0;

			if (this.TmpCommonState == ManRapeSleepState.ForcefullRape)
				this.Girl.faint = 0.0;

			yield return this.Performer.Perform(ActionType.Finish);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.FinishIdle);

			if (this.TmpCommonSub != 5) {
				this.MenuPanel.Show();
				this.MenuPanel.ShowFinishMenu();
			}
		}

		private IEnumerator OnLeave()
		{
			this.Destroy();
			yield break;
		}

		private void WakeupCheck()
		{
			if (this.TmpCommonState == ManRapeSleepState.GentlyRape && this.TmpCommonSub >= 1)
			{
				if (this.TmpCommonSub == 2)
				{
					this.NoticeTime += Time.deltaTime * 1.5f;
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
						this.Destroyed = true;
						this.TmpCommonSub = 5;
					}
					this.NoticeTime = 0f;
				}
			}
		}

		public override IEnumerator Run()
		{
			if (!this.SetupScene())
				yield break;

			this.CommonAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.StartIdle);

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

			this.MenuPanel.Close();
			Managers.mn.uiMN.MainCanvasView(true);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
		}

		public override bool CanContinue()
		{
			return this.TmpSex != null && !Input.GetKeyDown(KeyCode.R) && this.Man.life > 0 && !this.Destroyed;
		}

		public override void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public override CommonStates[] GetActors()
		{
			return [this.Man, this.Girl];
		}

		public override string ExpandAnimationName(string originalName)
		{
			return originalName.Replace("[Tits]", this.Girl.parameters[6].ToString("00"));
		}
	}
}
