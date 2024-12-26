using System;
using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Hook;
using ExtendedHSystem.ParamContainers;
using ExtendedHSystem.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace ExtendedHSystem.Scenes
{
	public class CommonSexPlayer : IScene, IScene2
	{
		protected static Dictionary<int, Dictionary<int?, List<SexPerformerInfo>>> Performers = [];

		public static readonly string Name = "CommonSexPlayer";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Caress = "Caress";
			public const string Insert = "Insert";
			public const string Speed = "Speed";
			public const string Pose2 = "Pose2";
			public const string Finish = "Finish";
			public const string Stop = "Stop";
			public const string Leave = "Leave";
		}

		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly int Type;

		private Vector3 Position;

		private NPCMove NpcMove;

		public string SexType { get; private set; }

		private GameObject TmpSex = null;

		private int TmpCommonState = 0;

		private int TmpCommonSub = 0;

		public int TmpSexCountType { get; private set; } = 0;

		private float NpcAngle;

		public SkeletonAnimation CommonAnim { get; private set; }

		private ISceneController Controller;

		private readonly CommonSexPlayerMenuPanel MenuPanel;

		private SexPerformer Performer;

		public static void AddPerformer(SexPerformerInfo performer)
		{
			Dictionary<int?, List<SexPerformerInfo>> toPerformerList;
			if (!Performers.TryGetValue(performer.FromNpcId, out toPerformerList))
			{
				toPerformerList = new Dictionary<int?, List<SexPerformerInfo>>();
				Performers.Add(performer.FromNpcId, toPerformerList);
			}

			List<SexPerformerInfo> performerList;
			if (!toPerformerList.TryGetValue(performer.ToNpcId, out performerList))
			{
				performerList = new List<SexPerformerInfo>();
				toPerformerList.Add(performer.ToNpcId, performerList);
			}

			performerList.Add(performer);
		}

		public CommonSexPlayer(CommonStates playerCommon, CommonStates npcCommon, Vector3 pos, int sexType)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.Type = sexType;
			this.Position = pos;

			this.MenuPanel = new CommonSexPlayerMenuPanel();
			this.MenuPanel.OnCaressSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnCaress());
			this.MenuPanel.OnInsertSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnInsert());
			this.MenuPanel.OnSpeedSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnSpeed());
			this.MenuPanel.OnPose2Selected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnPose2());
			this.MenuPanel.OnFinishSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnFinish());
			this.MenuPanel.OnStopSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnStop());
			this.MenuPanel.OnLeaveSelected += (object s, int e) => Managers.mn.sexMN.StartCoroutine(this.OnLeave());
		}

		public void Init(ISceneController controller)
		{
			this.Controller = controller;
			this.Controller.SetScene(this);
		}

		public string GetName()
		{
			return CommonSexPlayer.Name;
		}

		private bool AreActorsAlive()
		{
			return this.Player.life > 0.0 && this.Npc.life > 0.0;
		}

		private void GetSceneInfo(out GameObject scene, out string sexType)
		{
			scene = null;
			sexType = "A_";

			if (Performers.TryGetValue(this.Player.npcID, out var toPerformerList))
			{
				if (toPerformerList.TryGetValue(this.Npc.npcID, out var performerList))
				{
					foreach (var performer in performerList)
					{
						if (performer.CanPlay(this))
						{
							this.Performer = new SexPerformer(performer, this.Controller);
							scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
						}
					}

				}
			}
			/*
						switch (this.Player.npcID)
						{
							case NpcID.Yona: // 0
								switch (this.Npc.npcID)
								{
									case NpcID.MaleNative: // 10
										scene = Managers.mn.sexMN.sexList[2].sexObj[4];
										break;

									case NpcID.BigNative: // 11
										scene = Managers.mn.sexMN.sexList[14].sexObj[0];
										break;

									case NpcID.SmallNative: // 12
										scene = Managers.mn.sexMN.sexList[12].sexObj[0];
										break;

									case NpcID.ElderBrotherNative: // 91
										scene = Managers.mn.sexMN.sexList[13].sexObj[0];
										break;
								}
								break;

							case NpcID.Man: // 1
								switch (this.Npc.npcID)
								{
									case NpcID.Reika: // 5
										if (this.Type == 1 && Managers.mn.story.QuestProgress("Sub_Keigo") == 2)
											scene = Managers.mn.sexMN.sexList[1].sexObj[17];
										else
											scene = Managers.mn.sexMN.sexList[1].sexObj[12];
										break;

									case NpcID.FemaleNative: // 15
										if (CommonUtils.IsPregnant(this.Npc))
											scene = Managers.mn.sexMN.sexList[1].sexObj[20];
										else
											scene = Managers.mn.sexMN.sexList[1].sexObj[1];
										break;

									case NpcID.NativeGirl: // 16
										scene = Managers.mn.sexMN.sexList[1].sexObj[3];
										break;

									case NpcID.FemaleLargeNative: // 17
										this.SexType = "Love_A_";
										if (this.Type == 0)
											scene = Managers.mn.sexMN.sexList[8].sexObj[1];
										else
											scene = Managers.mn.sexMN.sexList[1].sexObj[15];
										break;

									case NpcID.Mummy: // 42*
										scene = Managers.mn.sexMN.sexList[1].sexObj[6];
										this.TmpSexCountType = 1;
										break;

									case NpcID.UnderGroundWoman: // 44*
										scene = Managers.mn.sexMN.sexList[1].sexObj[8];
										this.SexType = "Love_A_";
										break;

									case NpcID.Mermaid: // 71*
										scene = Managers.mn.sexMN.sexList[1].sexObj[9];
										if (this.Type == 0)
											this.TmpSexCountType = 1;
										else
											this.SexType = "B_";
										break;

									case NpcID.ElderSisterNative: // 90
										scene = Managers.mn.sexMN.sexList[1].sexObj[11];
										break;

									case NpcID.Giant: // 110
										scene = Managers.mn.sexMN.sexList[1].sexObj[7];
										break;

									case NpcID.Cassie2: // 113
										scene = Managers.mn.sexMN.sexList[1].sexObj[16];
										break;

									case NpcID.Shino: // 114
										scene = Managers.mn.sexMN.sexList[1].sexObj[19];
										if (this.Type == 0)
											this.TmpSexCountType = 1;
										else
											this.SexType = "B_";
										break;

									case NpcID.Sally: // 115
										scene = Managers.mn.sexMN.sexList[11].sexObj[0];
										this.SexType = "Love_A_";
										break;

									case NpcID.Merry: // 116
										scene = Managers.mn.sexMN.sexList[1].sexObj[25];
										break;
								}
								break;
						}
			*/
		}

		private void SetupTmpSex()
		{
			if (this.Player.npcID == NpcID.Yona)
			{
				// NOTE: right know, all Yona scenes are with humans. Maybe this will change later...
				// NPC IDs (as of v0.2.3): 10, 11, 12, 91
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Player, this.Npc);
			}
			else
			{
				if (this.Npc.npcID == NpcID.Reika)
				{
					// @TODO: Probably can be simplified
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				}
				else if (this.Npc.npcID == NpcID.Mermaid)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
				}
				else if (this.Npc.npcID == NpcID.Mummy)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadMummy(this.Npc, this.TmpSex);
				}
				else if (this.Npc.npcID == NpcID.UnderGroundWoman)
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Player);
					Managers.mn.randChar.LoadGenUnder(this.Npc, this.TmpSex);
				}
				else
				{
					Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, this.Player);
				}
			}
		}

		private void DisableLiveNpc(CommonStates npc, NPCMove npcMove)
		{
			npcMove.actType = NPCMove.ActType.Wait;
			this.NpcAngle = npcMove.searchAngle;
			this.NpcMove.searchAngle = 0f;
			npcMove.RBState(false);

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = false;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = false;

			Managers.mn.randChar.HandItemHide(npc, true);
		}

		private void EnableLiveNpc(CommonStates npc, NPCMove npcMove)
		{
			Managers.mn.randChar.HandItemHide(npc, false);

			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = true;

			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;

			npcMove.actType = NPCMove.ActType.Idle;
			npcMove.searchAngle = this.NpcAngle;
			npc.gameObject.transform.position = this.Position;
			npc.sex = CommonStates.SexState.None;
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

		private bool SetupScene()
		{
			SexMeter.Instance.Init(this.Position + new Vector3(1f, 1f, 0f), 0.3f);
			SexMeter.Instance.Show();

			Managers.mn.uiMN.MainCanvasView(false);

			this.TmpCommonState = 0;
			this.TmpCommonSub = 0;
			this.TmpSexCountType = 0;

			this.MenuPanel.Open(this.Position);
			this.MenuPanel.ShowInitialMenu();

			string sexType;
			this.GetSceneInfo(out GameObject scene, out sexType);
			this.SexType = sexType;
			if (scene == null)
				return false;

			this.TmpSex = GameObject.Instantiate<GameObject>(scene, this.Position, Quaternion.identity);
			this.SetupTmpSex();

			this.NpcMove = this.Npc.GetComponent<NPCMove>();
			this.DisableLiveNpc(this.Npc, this.NpcMove);
			this.DisableLivePlayer(this.Player);

			return true;
		}

		private IEnumerator OnCaress()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Caress);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 1;
			this.TmpCommonSub = 0;
			yield return this.Performer.Perform(ActionType.Caress);
			// if (this.CommonAnim != null)
			// {
			// 	if (this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Contact_01_" + this.Npc.parameters[6].ToString("00")) != null)
			// 	{
			// 		yield return this.Controller.LoopAnimation(this.SexType + "Contact_01_" + this.Npc.parameters[6].ToString("00"));
			// 	}
			// 	else if (this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Contact_01") != null)
			// 	{
			// 		yield return this.Controller.LoopAnimation(this.SexType + "Contact_01");
			// 	}
			// 	else
			// 	{
			// 		Debug.LogError("Animation not found");
			// 	}
			// }

			this.MenuPanel.ShowCaressMenu();
			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Caress);
		}

		private IEnumerator OnInsert()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Insert);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 2;
			this.TmpCommonSub = 0;
			yield return this.Performer.Perform(ActionType.Insert);
			yield return this.Performer.Perform(ActionType.Speed1);
			// yield return this.Controller.LoopAnimation(this.SexType + "Loop_01");

			bool hasAlternativePose = this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Loop_01_00") != null;
			this.MenuPanel.ShowInsertMenu(hasAlternativePose);

			// if (this.TmpSexCountType == 0)
			// 	yield return HookManager.Instance.RunEventHook(this, EventNames.OnPenetrate, new FromToParams(this.Player, this.Npc));

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Insert);
		}

		private IEnumerator OnSpeed()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed);
			if (!this.CanContinue())
				yield break;

			if (this.TmpCommonState == 2) /* Slow */
			{
				this.TmpCommonState = 3;
				yield return this.Performer.Perform(ActionType.Speed2);
			}
			else // (this.TmpCommonState == 3 /* Fast */)
			{
				this.TmpCommonState = 2;
				yield return this.Performer.Perform(ActionType.Speed1);
			}

			this.MenuPanel.ShowInsertMenu(this.Performer.HasAlternativePose());
			// if (this.CommonAnim.state.GetCurrent(0).Animation.Name == this.SexType + "Loop_01")
			// {
			// 	this.TmpCommonState = 3;
			// 	yield return this.Controller.LoopAnimation(this.SexType + "Loop_02");
			// 	bool hasAlternativePose = this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Loop_02_00") != null;
			// 	this.MenuPanel.ShowInsertMenu(hasAlternativePose);
			// }
			// else
			// {
			// 	this.TmpCommonState = 2;
			// 	yield return this.Controller.LoopAnimation(this.SexType + "Loop_01");
			// 	bool hasAlternativePose = this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Loop_01_00") != null;
			// 	this.MenuPanel.ShowInsertMenu(hasAlternativePose);
			// }

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed);
		}

		private IEnumerator OnPose2()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Pose2);
			if (!this.CanContinue())
				yield break;

			if (this.TmpCommonSub == 0) /* Pose 1 */
			{
				this.TmpCommonSub = 1;
				yield return this.Performer.ChangePose();
				// string a = this.CommonAnim.state.GetCurrent(0).Animation.Name;
				// if ((a != "A_Loop_01") && (a != "B_Loop_01"))
				// {
				// 	if (a == "A_Loop_02" || a == "B_Loop_02")
				// 	{
				// 		yield return this.Controller.LoopAnimation(this.SexType + "Loop_02_" + this.Npc.parameters[6].ToString("00"));
				// 	}
				// }
				// else
				// {
				// 	yield return this.Controller.LoopAnimation(this.SexType + "Loop_01_" + this.Npc.parameters[6].ToString("00"));
				// }
			}
			else /* TmpCommonSub == 1 -- Pose 2 */
			{
				this.TmpCommonSub = 0;
				yield return this.Performer.ChangePose();

				// string[] array = this.CommonAnim.state.GetCurrent(0).Animation.Name.Split('_', StringSplitOptions.None);
				// if (array.Length >= 2)
				// {
				// 	string a = array[2];
				// 	if (!(a == "01"))
				// 	{
				// 		if (a == "02")
				// 		{
				// 			yield return this.Controller.LoopAnimation(this.SexType + "Loop_02");
				// 		}
				// 	}
				// 	else
				// 	{
				// 		yield return this.Controller.LoopAnimation(this.SexType + "Loop_01");
				// 	}
				// }
			}

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Pose2);
		}

		private IEnumerator OnFinish()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 0;
			this.MenuPanel.Hide();

			yield return this.Performer.Perform(ActionType.Finish);
			// if (this.TmpCommonSub == 0)
			// 	animName = this.SexType + "Finish";
			// else
			// 	animName = this.SexType + "Finish_00";

			// yield return this.Controller.PlayOnceStep_New(this, this.CommonAnim, animName);

			CommonStates from, to;
			if (CommonUtils.IsFemale(this.Player))
			{
				from = this.Npc;
				to = this.Player;
			}
			else
			{
				from = this.Player;
				to = this.Npc;
			}

			// yield return HookManager.Instance.RunEventHook(this, EventNames.OnOrgasm, new FromToParams(from, to));

			yield return this.Performer.Perform(ActionType.FinishIdle);
			// if (this.TmpCommonSub == 0)
			// 	animName = this.SexType + "Finish_idle";
			// else
			// 	animName = this.SexType + "Finish_idle_00";

			// yield return this.Controller.LoopAnimation(animName);

			// if (this.TmpSexCountType == 0)
			// 	yield return HookManager.Instance.RunEventHook(this, EventNames.OnCreampie, new FromToParams(from, to));

			this.MenuPanel.Show();
			this.MenuPanel.ShowFinishMenu();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
		}

		private IEnumerator OnStop()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Stop);
			if (!this.CanContinue())
				yield break;

			this.TmpCommonState = 0;
			this.TmpCommonSub = 0;
			yield return this.Performer.Perform(ActionType.StartIdle);
			// yield return this.Controller.LoopAnimation(this.SexType + "idle");

			this.MenuPanel.ShowStopMenu();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Stop);
		}

		private IEnumerator OnLeave()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Leave);
			if (!this.CanContinue())
				yield break;

			UnityEngine.Object.Destroy(this.TmpSex);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Leave);
		}

		private void Teardown()
		{
			this.EnableLiveNpc(this.Npc, this.NpcMove);
			this.EnableLivePlayer(this.Player);

			this.MenuPanel.Close();

			Managers.mn.uiMN.MainCanvasView(true);
			SexMeter.Instance.Hide();

			Managers.mn.uiMN.StatusChange(null);
		}

		public IEnumerator Run()
		{
			if (!this.SetupScene())
			{
				if (this.TmpSex != null)
					UnityEngine.Object.Destroy(this.TmpSex);

				Debug.LogError("sex not found");
				yield break;
			}

			this.CommonAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				this.Teardown();
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				yield break;
			}

			yield return this.Performer.Perform(ActionType.StartIdle);

			while (this.CanContinue())
			{
				switch (this.TmpCommonState)
				{
					case 1:
						if (SexMeter.Instance.FillAmount <= 0.3f)
							SexMeter.Instance.Fill(Time.deltaTime * 0.03f);
						else
							SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
						break;
					case 2:
						if (SexMeter.Instance.FillAmount <= 0.3f)
							SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
						else
							SexMeter.Instance.Fill(Time.deltaTime * 0.03f);
						break;
					case 3:
						if (SexMeter.Instance.FillAmount <= 0.3f)
							SexMeter.Instance.Fill(Time.deltaTime * 0.005f);
						else
							SexMeter.Instance.Fill(Time.deltaTime * 0.05f);
						break;
				}
				yield return null;
			}

			this.Teardown();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
		}


		public bool CanContinue()
		{
			return this.TmpSex != null && this.AreActorsAlive();
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public CommonStates[] GetActors()
		{
			if (CommonUtils.IsMale(this.Player))
				return [this.Player, this.Npc];
			else if (CommonUtils.IsMale(this.Npc))
				return [this.Npc, this.Player];

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
	}
}