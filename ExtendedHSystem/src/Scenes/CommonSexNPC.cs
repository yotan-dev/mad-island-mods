using System.Collections;
using System.Collections.Generic;
using ExtendedHSystem.Performer;
using Mono.Cecil;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace ExtendedHSystem.Scenes
{
	public class CommonSexNPC : IScene, IScene2
	{
		protected static Dictionary<int, Dictionary<int?, List<SexPerformerInfo>>> Performers = [];

		public static readonly string Name = "CommonSexNPC";

		/// <summary>
		/// First NPC in the Sex Scene.
		/// If a female NPC is involved, this is the female one.
		/// </summary>
		public readonly CommonStates NpcA;

		/// <summary>
		/// Second NPC in the Sex Scene.
		/// </summary>
		public readonly CommonStates NpcB;

		public readonly SexPlace Place;

		public readonly SexManager.SexCountState Type;

		private NPCMove AMove, BMove;

		private bool Pregable;

		private string SexType;

		private GameObject TmpSex;

		private float AAngle;

		private float BAngle;

		private ISceneController Controller;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		private SexPerformer Performer;

		private SkeletonAnimation SexAnim;

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

		public CommonSexNPC(CommonStates npcA, CommonStates npcB, SexPlace sexPlace, SexManager.SexCountState sexType)
		{
			this.Place = sexPlace;
			this.Type = sexType;

			this.NpcA = npcA;
			this.NpcB = npcB;
			if (!CommonUtils.IsFemale(npcA) && CommonUtils.IsFemale(npcB))
			{
				this.NpcA = npcB;
				this.NpcB = npcA;
			}
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

		private void PrepareNpc(CommonStates npc, out NPCMove npcMove)
		{
			npcMove = npc.GetComponent<NPCMove>();
			npcMove.actType = NPCMove.ActType.Wait;
			npc.sex = CommonStates.SexState.Playing;
		}

		private void ResetNpc(CommonStates npc, NPCMove npcMove)
		{
			npc.sex = CommonStates.SexState.None;
			npcMove.actType = NPCMove.ActType.Travel;
		}

		private GameObject SetEmotion(CommonStates npc)
		{
			return Managers.mn.fxMN.GoEmotion(0, npc.gameObject, Vector3.zero);
		}

		private bool AreActorsAlive()
		{
			return this.NpcA.dead == 0 && this.NpcB.dead == 0;
		}

		private bool AreActorsWaiting()
		{
			return this.AMove.actType == NPCMove.ActType.Wait && this.BMove.actType == NPCMove.ActType.Wait;
		}

		private bool IsPlaceFree()
		{
			return this.Place.user == null;
		}

		private void NpcAtPosCheck(CommonStates npc, NPCMove move, Vector3 pos, ref bool reached)
		{
			if (reached)
				return;


			if (Vector3.Distance(npc.gameObject.transform.position, pos) > 1f)
			{
				if (npc.anim.state.GetCurrent(0).Animation.Name != "A_walk")
					npc.anim.state.SetAnimation(0, "A_walk", true);

				return;
			}

			reached = true;
			if (move.common.anim.state.GetCurrent(0).Animation.Name != "A_idle")
				move.common.anim.state.SetAnimation(0, "A_idle", true);
		}

		private void GetSceneInfo(out GameObject scene, out bool pregable, out string sexType)
		{
			pregable = false;
			scene = null;
			sexType = "A_";

			if (Performers.TryGetValue(this.NpcB.npcID, out var toPerformerList))
			{
				if (toPerformerList.TryGetValue(this.NpcA.npcID, out var performerList))
				{
					foreach (var performer in performerList)
					{
						if (performer.CanPlay(this, PerformerScope.Sex))
						{
							this.Performer = new SexPerformer(performer, this.Controller);
							scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
						}
					}

				}
			}
			/*
						switch (this.NpcA.npcID)
						{
							case NpcID.FemaleNative: // 15
								switch (this.NpcB.npcID)
								{
									case NpcID.MaleNative: // 10
										if (this.NpcA.pregnant[0] == -1)
										{
											pregable = true;
											scene = Managers.mn.sexMN.sexList[2].sexObj[0];
										}
										break;

									case NpcID.FemaleNative: // 15
										if (this.NpcA.pregnant[0] == -1 && this.NpcB.pregnant[0] == -1)
										{
											scene = Managers.mn.sexMN.sexList[9].sexObj[0];
										}
										break;

									case NpcID.YoungMan: // 89
										if (this.NpcA.pregnant[0] == -1)
										{
											scene = Managers.mn.sexMN.sexList[1].sexObj[1];
											pregable = true;
										}
										else
										{
											scene = Managers.mn.sexMN.sexList[1].sexObj[20];
										}
										break;
								}
								break;

							case NpcID.NativeGirl: // 16
								switch (this.NpcB.npcID)
								{
									case NpcID.MaleNative: // 10
										if (this.NpcA.pregnant[0] == -1)
										{
											scene = Managers.mn.sexMN.sexList[2].sexObj[1];
											pregable = true;
										}
										break;

									case NpcID.NativeGirl: // 16
										if (this.NpcA.pregnant[0] == -1 && this.NpcB.pregnant[0] == -1)
										{
											scene = Managers.mn.sexMN.sexList[10].sexObj[0];
										}
										break;

									case NpcID.YoungMan: // 89
										if (this.NpcA.pregnant[0] == -1)
										{
											scene = Managers.mn.sexMN.sexList[1].sexObj[3];
											pregable = true;
										}
										break;
								}
								break;

							case NpcID.FemaleLargeNative: // 17
								switch (this.NpcB.npcID)
								{
									case NpcID.YoungMan: // 89
										scene = Managers.mn.sexMN.sexList[1].sexObj[15];
										sexType = "Love_A_";
										break;
								}
								break;

							case NpcID.UnderGroundWoman: // 44
								switch (this.NpcB.npcID)
								{
									case NpcID.YoungMan: // 89
										scene = Managers.mn.sexMN.sexList[1].sexObj[8];
										sexType = "Love_A_";
										break;
								}
								break;

							case NpcID.ElderSisterNative: // 90
								switch (this.NpcB.npcID)
								{
									case NpcID.YoungMan: // 89
										scene = Managers.mn.sexMN.sexList[1].sexObj[11];
										break;
								}
								break;
						}
			*/
		}

		private void SetupTmpSex()
		{
			if (this.NpcA.npcID == this.NpcB.npcID)
			{ // Same NPC (usually girl x girl)
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.NpcA, null);
				CommonStates component2 = this.TmpSex.GetComponent<CommonStates>();
				if (component2 != null)
				{
					Managers.mn.randChar.CopyParams(this.NpcB, component2);
					Managers.mn.randChar.LoadGenGirl(this.TmpSex, false, RandomCharacter.LoadType.G);
				}
			}
			else
			{ // Basic case, usually girl x men
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.NpcA, this.NpcB);
			}
		}

		private void DisableLiveNpc(CommonStates npc, NPCMove npcMove)
		{
			npcMove.RBState(false);
			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();
			mesh.enabled = false;
			if (coll != null)
				coll.enabled = false;
		}

		private void EnableLiveNpc(CommonStates npc)
		{
			CapsuleCollider coll = npc.GetComponent<CapsuleCollider>();
			MeshRenderer mesh = npc.anim.GetComponent<MeshRenderer>();

			if (coll != null)
				coll.enabled = true;

			mesh.enabled = true;
		}

		private bool SetupScene()
		{
			this.GetSceneInfo(out GameObject scene, out this.Pregable, out this.SexType);
			if (scene == null)
				return false;

			var pos = this.Place.transform.position;
			this.TmpSex = GameObject.Instantiate<GameObject>(scene, pos, Quaternion.identity);

			if (!this.IsPlaceFree() || this.TmpSex == null)
				return false;

			// @TODO: Check if we really need to those here or we can move to SetupTmpSex
			if (this.NpcA.npcID == NpcID.UnderGroundWoman && this.NpcB.npcID == NpcID.YoungMan)
			{
				Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.NpcB);
				Managers.mn.randChar.LoadGenUnder(this.NpcA, this.TmpSex);
			}
			else if (this.NpcA.npcID == NpcID.ElderSisterNative && this.NpcB.npcID == NpcID.YoungMan)
			{
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.NpcA, this.NpcB);
			}

			this.Place.user = this.TmpSex;
			this.TmpSex.transform.position += new Vector3(0f, 0f, 0.02f);
			this.AAngle = this.AMove.searchAngle;
			this.BAngle = this.BMove.searchAngle;
			this.AMove.searchAngle = 180f;
			this.BMove.searchAngle = 180f;
			this.NpcA.gameObject.transform.position = pos;
			this.NpcB.gameObject.transform.position = pos;

			this.SetupTmpSex();
			this.DisableLiveNpc(this.NpcA, this.AMove);
			this.DisableLiveNpc(this.NpcB, this.BMove);

			return true;
		}

		private IEnumerable MoveToPlace(Transform place)
		{
			Vector3 pos = place.position;
			float animTime = 30f;

			Managers.mn.sexMN.StartCoroutine(Managers.mn.story.MovePosition(this.NpcA.gameObject, pos, 2f, "A_walk", true, false, 0.1f, 40f));
			Managers.mn.sexMN.StartCoroutine(Managers.mn.story.MovePosition(this.NpcB.gameObject, pos, 2f, "A_walk", true, false, 0.1f, 40f));

			bool aReached = false;
			bool bReached = false;
			while (
				animTime > 0f
				&& this.AreActorsWaiting()
				&& (!aReached || !bReached)
				&& this.IsPlaceFree()
				&& this.AreActorsAlive()
			)
			{
				animTime -= Time.deltaTime;
				this.NpcAtPosCheck(this.NpcA, this.AMove, pos, ref aReached);
				this.NpcAtPosCheck(this.NpcB, this.BMove, pos, ref bReached);
				yield return false;
			}

			yield return animTime > 0f;
		}

		// private IEnumerable CallTypeEventHandlers(SexManager.SexCountState type)
		// {
		// 	switch (type)
		// 	{
		// 		case SexManager.SexCountState.Normal:
		// 			foreach (var handler in this.EventHandlers)
		// 			{
		// 				foreach (var x in handler.OnNormalSex(this.NpcA, this.NpcB))
		// 					yield return x;
		// 			}
		// 			break;

		// 		// case SexManager.SexCountState.Rapes:
		// 		// 	foreach (var handler in this.EventHandlers)
		// 		// 	{
		// 		// 		foreach (var x in handler.OnRape(this.NpcB, this.NpcA))
		// 		// 			yield return x;
		// 		// 	}
		// 		// 	break;

		// 		// case SexManager.SexCountState.Toilet:
		// 		// 	foreach (var handler in this.EventHandlers)
		// 		// 	{
		// 		// 		foreach (var x in handler.OnToilet(this.NpcA, this.NpcB))
		// 		// 			yield return x;
		// 		// 	}
		// 		// 	break;

		// 		case SexManager.SexCountState.Creampie:
		// 			foreach (var handler in this.EventHandlers)
		// 			{
		// 				foreach (var x in handler.OnCreampie(this.NpcB, this.NpcA))
		// 					yield return x;
		// 			}
		// 			break;

		// 		default:
		// 			PLogger.LogError("CommonSexNPC::CallTypeEventHandlers: Unexpected sex type: " + this.Type);
		// 			break;
		// 	}
		// }

		private IEnumerator Perform()
		{
			// foreach (var x in this.CallTypeEventHandlers(this.Type))
			// 	yield return x;

			this.SexAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			yield return this.Performer.Perform(ActionType.Insert);
			yield return this.Performer.Perform(ActionType.Speed1, 20f);
			yield return this.Performer.Perform(ActionType.Speed2, 10f);
			yield return this.Performer.Perform(ActionType.Finish);
			// foreach (var x in this.Controller.PlayTimedStep(this, this.SexAnim, this.SexType + "Loop_01", 20f))
			// 	yield return x;

			// foreach (var x in this.Controller.PlayTimedStep(this, this.SexAnim, this.SexType + "Loop_02", 10f))
			// 	yield return x;

			// bool hasCompleted = false;
			// foreach (var x in this.Controller.PlayOnceStep(this, this.SexAnim, this.SexType + "Finish"))
			// {
			// 	hasCompleted = (bool)x;
			// 	yield return x;
			// }

			// if (hasCompleted)
			// {
				// if (this.Pregable)
				// {
				// 	foreach (var x in this.CallTypeEventHandlers(SexManager.SexCountState.Creampie))
				// 		yield return x;
				// }

				// foreach (var handler in this.EventHandlers)
				// {
				// 	foreach (var x in handler.AfterSex(this, this.NpcA, this.NpcB))
				// 		yield return x;
				// }
			// }

			yield return this.Performer.Perform(ActionType.FinishIdle, 8f);
			// foreach (var x in this.Controller.PlayTimedStep(this, this.SexAnim, this.SexType + "Finish_idle", 8f))
			// 	yield return x;
		}

		public IEnumerator Run()
		{
			if (this.Place == null)
				yield break;

			Transform transform = this.Place.transform.Find("pos");
			if (transform == null)
			{
				Debug.LogError(this.Place.gameObject.name);
				yield break;
			}

			this.PrepareNpc(this.NpcA, out this.AMove);
			this.PrepareNpc(this.NpcB, out this.BMove);

			GameObject emotionA = this.SetEmotion(this.NpcA);
			GameObject emotionB = this.SetEmotion(this.NpcB);

			bool isAtPlace = false;
			foreach (var x in this.MoveToPlace(transform))
			{
				isAtPlace = (bool)x;
				yield return x;
			}

			emotionA.SetActive(false);
			emotionB.SetActive(false);
			if (!isAtPlace)
			{
				this.ResetNpc(this.NpcA, this.AMove);
				this.ResetNpc(this.NpcB, this.BMove);
				yield break;
			}

			if (!this.SetupScene())
			{
				this.NpcA.sex = CommonStates.SexState.None;
				this.NpcB.sex = CommonStates.SexState.None;
				if (this.TmpSex != null)
					Object.Destroy(this.TmpSex);
				this.AMove.actType = NPCMove.ActType.Travel;
				this.BMove.actType = NPCMove.ActType.Travel;
				yield break;
			}

			yield return this.Perform();

			// Teardown
			if (this.TmpSex != null)
				Object.Destroy(this.TmpSex);

			this.Place.user = null;
			this.EnableLiveNpc(this.NpcA);
			this.EnableLiveNpc(this.NpcB);

			if (this.AMove.actType == NPCMove.ActType.Wait && this.BMove.actType == NPCMove.ActType.Wait)
			{
				this.AMove.actType = NPCMove.ActType.Travel;
				this.BMove.actType = NPCMove.ActType.Travel;
			}
			else
			{
				this.AMove.actType = NPCMove.ActType.Interval;
				this.BMove.actType = NPCMove.ActType.Interval;
			}
			this.AMove.searchAngle = this.AAngle;
			this.BMove.searchAngle = this.BAngle;
			this.NpcA.sex = CommonStates.SexState.None;
			this.NpcB.sex = CommonStates.SexState.None;
			if (this.NpcA.debuff.perfume <= 0f)
			{
				this.NpcA.libido -= 20f;
			}
			if (this.NpcB.debuff.perfume <= 0f)
			{
				this.NpcB.libido -= 20f;
			}
			this.NpcA.MoralChange(3f, null, NPCManager.MoralCause.None);
			this.NpcB.MoralChange(3f, null, NPCManager.MoralCause.None);

			yield break;
		}

		public bool CanContinue()
		{
			return this.TmpSex != null && this.AreActorsWaiting() && this.AreActorsAlive();
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public string GetName()
		{
			return CommonSexNPC.Name;
		}

		public CommonStates[] GetActors()
		{
			// @TODO: Invert NPC A/B so male comes first
			return [this.NpcB, this.NpcA];
		}

		public SkeletonAnimation GetSkelAnimation()
		{
			return this.SexAnim;
		}

		public string ExpandAnimationName(string originalName)
		{
			return originalName; // @TODO:
		}
	}
}