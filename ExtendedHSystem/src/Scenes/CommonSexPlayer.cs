using System;
using System.Collections;
using System.Collections.Generic;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using YotanModCore;
using YotanModCore.Consts;

namespace ExtendedHSystem.Scenes
{
	public class CommonSexPlayer : IScene
	{
		public readonly CommonStates Player;

		public readonly CommonStates Npc;

		public readonly int Type;

		private Vector3 Position;

		private Image SexMeter;

		private NPCMove NpcMove;

		private string SexType;

		private GameObject TmpSex = null;

		private int TmpCommonState = 0;

		private int TmpCommonSub = 0;

		private int TmpSexCountType = 0;

		private float NpcAngle;

		private SkeletonAnimation CommonAnim;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		private readonly CommonSexPlayerMenuPanel MenuPanel;

		public CommonSexPlayer(CommonStates playerCommon, CommonStates npcCommon, Vector3 pos, int sexType)
		{
			this.Player = playerCommon;
			this.Npc = npcCommon;
			this.Type = sexType;
			this.Position = pos;

			this.MenuPanel = new CommonSexPlayerMenuPanel();
			this.MenuPanel.OnCaressSelected += this.OnCaress;
			this.MenuPanel.OnInsertSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnInsert(sender, e));
			};
			this.MenuPanel.OnSpeedSelected += this.OnSpeed;
			this.MenuPanel.OnPose2Selected += this.OnPose2;
			this.MenuPanel.OnFinishSelected += (object sender, int e) =>
			{
				Managers.mn.sexMN.StartCoroutine(this.OnFinish());
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

		private bool AreActorsAlive()
		{
			return this.Player.life > 0.0 && this.Npc.life > 0.0;
		}

		private void GetSceneInfo(out GameObject scene, out string sexType)
		{
			scene = null;
			sexType = "A_";

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
			this.SexMeter = Managers.mn.sexMN.sexMeter;

			this.SexMeter.transform.parent.gameObject.transform.position = this.Position + new Vector3(1f, 1f, 0f);
			this.SexMeter.transform.parent.gameObject.SetActive(true);
			this.SexMeter.fillAmount = 0f;
			Managers.mn.uiMN.MainCanvasView(false);

			this.TmpCommonState = 0;
			this.TmpCommonSub = 0;
			this.TmpSexCountType = 0;

			this.MenuPanel.Open(this.Position);
			this.MenuPanel.ShowInitialMenu();

			this.GetSceneInfo(out GameObject scene, out this.SexType);
			if (scene == null)
				return false;

			this.TmpSex = GameObject.Instantiate<GameObject>(scene, this.Position, Quaternion.identity);
			this.SetupTmpSex();

			this.NpcMove = this.Npc.GetComponent<NPCMove>();
			this.DisableLiveNpc(this.Npc, this.NpcMove);
			this.DisableLivePlayer(this.Player);

			return true;
		}

		private void OnCaress(object sender, int e)
		{
			this.TmpCommonState = 1;
			this.TmpCommonSub = 0;
			if (this.CommonAnim != null)
			{
				if (this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Contact_01_" + this.Npc.parameters[6].ToString("00")) != null)
				{
					this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Contact_01_" + this.Npc.parameters[6].ToString("00"));
				}
				else if (this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Contact_01") != null)
				{
					this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Contact_01");
				}
				else
				{
					Debug.LogError("Animation not found");
				}
			}

			this.MenuPanel.ShowCaressMenu();
		}

		private IEnumerator OnInsert(object sender, int e)
		{
			this.TmpCommonState = 2;
			this.TmpCommonSub = 0;
			this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_01");

			bool hasAlternativePose = this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Loop_01_00") != null;
			this.MenuPanel.ShowInsertMenu(hasAlternativePose);

			if (this.TmpSexCountType == 0)
			{
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnNormalSex(this.Player, this.Npc))
						yield return x;
				}
			}
		}

		private void OnSpeed(object sender, int e)
		{
			if (this.CommonAnim.state.GetCurrent(0).Animation.Name == this.SexType + "Loop_01")
			{
				this.TmpCommonState = 3;
				this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_02");
				bool hasAlternativePose = this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Loop_02_00") != null;
				this.MenuPanel.ShowInsertMenu(hasAlternativePose);
			}
			else
			{
				this.TmpCommonState = 2;
				this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_01");
				bool hasAlternativePose = this.CommonAnim.skeleton.Data.FindAnimation(this.SexType + "Loop_01_00") != null;
				this.MenuPanel.ShowInsertMenu(hasAlternativePose);
			}
		}

		private void OnPose2(object sender, int e)
		{
			if (this.TmpCommonSub == 0)
			{
				this.TmpCommonSub = 1;
				string a = this.CommonAnim.state.GetCurrent(0).Animation.Name;
				if (!(a == "A_Loop_01") && !(a == "B_Loop_01"))
				{
					if (a == "A_Loop_02" || a == "B_Loop_02")
					{
						this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_02_" + this.Npc.parameters[6].ToString("00"));
					}
				}
				else
				{
					this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_01_" + this.Npc.parameters[6].ToString("00"));
				}
			}
			else
			{
				this.TmpCommonSub = 0;
				string[] array = this.CommonAnim.state.GetCurrent(0).Animation.Name.Split('_', StringSplitOptions.None);
				if (array.Length >= 2)
				{
					string a = array[2];
					if (!(a == "01"))
					{
						if (a == "02")
						{
							this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_02");
						}
					}
					else
					{
						this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "Loop_01");
					}
				}
			}
		}

		private IEnumerator OnFinish()
		{
			this.TmpCommonState = 0;
			this.MenuPanel.Hide();

			string animName;
			if (this.TmpCommonSub == 0)
				animName = this.SexType + "Finish";
			else
				animName = this.SexType + "Finish_00";

			foreach (var x in this.Controller.PlayOnceStep(this, this.CommonAnim, animName))
				yield return x;

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

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.OnBusted(from, to, this.TmpSexCountType))
					yield return x;
			}

			if (this.TmpCommonSub == 0)
				animName = this.SexType + "Finish_idle";
			else
				animName = this.SexType + "Finish_idle_00";

			this.Controller.LoopAnimation(this, this.CommonAnim, animName);

			if (this.TmpSexCountType == 0)
			{
				foreach (var handler in this.EventHandlers)
				{
					foreach (var x in handler.OnCreampie(from, to))
						yield return x;
				}
			}

			this.MenuPanel.Show();
			this.MenuPanel.ShowFinishMenu();
		}

		private void OnStop(object sender, int e)
		{
			this.TmpCommonState = 0;
			this.TmpCommonSub = 0;
			this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "idle");

			this.MenuPanel.ShowStopMenu();
		}

		private void OnLeave(object sender, int e)
		{
			UnityEngine.Object.Destroy(this.TmpSex);
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
			this.Controller.LoopAnimation(this, this.CommonAnim, this.SexType + "idle");

			while (this.CanContinue())
			{
				switch (this.TmpCommonState)
				{
					case 1:
						if (this.SexMeter.fillAmount <= 0.3f)
							this.SexMeter.fillAmount += Time.deltaTime * 0.03f;
						else
							this.SexMeter.fillAmount += Time.deltaTime * 0.005f;
						break;
					case 2:
						if (this.SexMeter.fillAmount <= 0.3f)
							this.SexMeter.fillAmount += Time.deltaTime * 0.005f;
						else
							this.SexMeter.fillAmount += Time.deltaTime * 0.03f;
						break;
					case 3:
						if (this.SexMeter.fillAmount <= 0.3f)
							this.SexMeter.fillAmount += Time.deltaTime * 0.005f;
						else
							this.SexMeter.fillAmount += Time.deltaTime * 0.05f;
						break;
				}
				yield return null;
			}

			this.EnableLiveNpc(this.Npc, this.NpcMove);
			this.EnableLivePlayer(this.Player);

			this.MenuPanel.Close();

			Managers.mn.uiMN.MainCanvasView(true);
			this.SexMeter.transform.parent.gameObject.SetActive(false);

			foreach (var handler in this.EventHandlers)
			{
				foreach (var x in handler.AfterSex(this, this.Npc, this.Player))
					yield return x;
			}

			Managers.mn.uiMN.StatusChange(null);
		}


		public bool CanContinue()
		{
			return this.TmpSex != null && this.AreActorsAlive();
		}

		public float GetSexMeterFillAmount()
		{
			return this.SexMeter.fillAmount;
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}
	}
}