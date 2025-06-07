using System.Collections;
using HFramework.Handlers;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Scenes
{
	public class CommonSexNPC : BaseScene
	{
		public static readonly string Name = "HF_CommonSexNPC";

		public static class StepNames
		{
			public const string Main = "Main";
		}

		/// <summary>
		/// First NPC in the Sex Scene.
		/// If a non-female NPC is involved, this is it.
		/// </summary>
		public readonly CommonStates Npc1;

		/// <summary>
		/// Second NPC in the Sex Scene.
		/// Usually, a female NPC
		/// </summary>
		public readonly CommonStates Npc2;

		public readonly SexPlace Place;

		public readonly SexManager.SexCountState Type;

		private GameObject TmpSex;

		private float AAngle;

		private float BAngle;

		private CommonStates NpcA;

		private CommonStates NpcB;

		public bool Success { get; private set; } = false;

		public CommonSexNPC(CommonStates npcA, CommonStates npcB, SexPlace sexPlace, SexManager.SexCountState sexType)
			: base(Name)
		{
			this.Place = sexPlace;
			this.Type = sexType;
			this.NpcA = npcA;
			this.NpcB = npcB;
			var actors = Utils.SortActors(npcA, npcB);

			this.Npc1 = actors[0];
			this.Npc2 = actors[1];
		}

		private void ResetNpc(CommonStates npc)
		{
			npc.sex = CommonStates.SexState.None;
			npc.nMove.actType = NPCMove.ActType.Travel;
		}

		private GameObject SetEmotion(CommonStates npc)
		{
			return Managers.mn.fxMN.GoEmotion(0, npc.gameObject, Vector3.zero);
		}

		private bool AreActorsAlive()
		{
			return this.Npc2.dead == 0 && this.Npc1.dead == 0;
		}

		private bool AreActorsWaiting()
		{
			return this.Npc1.nMove.actType == NPCMove.ActType.Wait && this.Npc2.nMove.actType == NPCMove.ActType.Wait;
		}

		private bool IsPlaceFree()
		{
			return this.Place.user == null;
		}

		private void SetupTmpSex()
		{
			if (this.Npc2.npcID == this.Npc1.npcID)
			{ // Same NPC (usually girl x girl)
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc2, null);
				CommonStates component2 = this.TmpSex.GetComponent<CommonStates>();
				if (component2 != null)
				{
					Managers.mn.randChar.CopyParams(this.Npc1, component2);
					Managers.mn.randChar.LoadGenGirl(this.TmpSex, false, RandomCharacter.LoadType.G);
				}
			}
			else
			{ // Basic case, usually girl x men
				Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc2, this.Npc1);
			}
		}

		private void DisableLiveNpc(CommonStates npc)
		{
			npc.nMove.RBState(false);
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
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
			{
				PLogger.LogError($"Failed to get performer for {this.Npc1.npcID} and {this.Npc2.npcID}");
				return false;
			}

			var scene = this.Performer.Info.SexPrefabSelector.GetPrefab();
			if (scene == null)
			{
				PLogger.LogError($"Failed to get prefab for {this.Npc1.npcID} and {this.Npc2.npcID}");
				return false;
			}

			var pos = this.Place.transform.position;
			this.TmpSex = GameObject.Instantiate<GameObject>(scene, pos, Quaternion.identity);

			if (!this.IsPlaceFree() || this.TmpSex == null)
				return false;

			// @TODO: Check if we really need to those here or we can move to SetupTmpSex
			if (this.Npc2.npcID == NpcID.UnderGroundWoman && this.Npc1.npcID == NpcID.YoungMan)
			{
				Managers.mn.randChar.SetCharacter(this.TmpSex, null, this.Npc1);
				Managers.mn.randChar.LoadGenUnder(this.Npc2, this.TmpSex);
			}
			// Removed in v0.4.2/v0.4.3 -- needs confirmation if it works for prev version
			// else if (this.Npc2.npcID == NpcID.ElderSisterNative && this.Npc1.npcID == NpcID.YoungMan)
			// {
			// 	Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc2, this.Npc1);
			// }

			this.Place.user = this.TmpSex;
			this.TmpSex.transform.position += new Vector3(0f, 0f, 0.02f);
			this.AAngle = this.Npc1.nMove.searchAngle;
			this.BAngle = this.Npc2.nMove.searchAngle;
			this.Npc1.nMove.searchAngle = 180f;
			this.Npc2.nMove.searchAngle = 180f;
			this.Npc2.gameObject.transform.position = pos;
			this.Npc1.gameObject.transform.position = pos;

			this.SetupTmpSex();
			this.DisableLiveNpc(this.Npc2);
			this.DisableLiveNpc(this.Npc1);

			return true;
		}

		private IEnumerator Perform()
		{
			if (!this.CanContinue())
				yield break;

			this.CommonAnim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();
			yield return this.Performer.Perform(ActionType.Insert);
			if (!this.CanContinue())
				yield break;
			
			yield return this.Performer.Perform(ActionType.Speed1, new PerformModifiers() { Duration = 20f });
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Speed2, new PerformModifiers() { Duration = 10f });
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Finish);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.FinishIdle, new PerformModifiers() { Duration = 8f });
			if (!this.CanContinue())
				yield break;

			this.Success = true;
		}

		public override IEnumerator Run()
		{
			if (this.Place == null)
			{
				Debug.LogError($"CommonSexNPC - Place is null");
				yield break;
			}

			Transform transform = this.Place.transform.Find("pos");
			if (transform == null)
			{
				Debug.LogError(this.Place.gameObject.name);
				yield break;
			}

			// Npc A (the one that initiated the Sex) must have their actType forced, but not waited for,
			// as they will be locked in the NPCMove.Live process.
			// But we should wait for Npc B to switch to Wait or it may switch state in the middle of the
			// scene and fall through the world.
			this.NpcA.nMove.actType = NPCMove.ActType.Wait;
			this.NpcA.sex = CommonStates.SexState.Playing;

			this.NpcB.sex = CommonStates.SexState.Playing;
			yield return new MakeNpcWait(this, [this.NpcB]).Handle();

			GameObject emotionA = this.SetEmotion(this.Npc2);
			GameObject emotionB = this.SetEmotion(this.Npc1);

			yield return new MoveToPlace(this, [this.Npc2, this.Npc1], transform.position, this.Place).Handle();

			emotionA.SetActive(false);
			emotionB.SetActive(false);
			
			// Check for destroyed only, as CanContinue is still false at this point as the scene is not ready.
			if (this.Destroyed)
			{
				this.ResetNpc(this.Npc2);
				this.ResetNpc(this.Npc1);
				yield break;
			}

			if (!this.SetupScene())
			{
				this.Npc2.sex = CommonStates.SexState.None;
				this.Npc1.sex = CommonStates.SexState.None;
				if (this.TmpSex != null)
					Object.Destroy(this.TmpSex);
				this.Npc1.nMove.actType = NPCMove.ActType.Travel;
				this.Npc2.nMove.actType = NPCMove.ActType.Travel;
				yield break;
			}

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);

			yield return this.Perform();

			// Teardown
			if (this.TmpSex != null)
			{
				Object.Destroy(this.TmpSex);
				this.TmpSex = null;
			}

			this.Place.user = null;
			this.EnableLiveNpc(this.Npc2);
			this.EnableLiveNpc(this.Npc1);

			if (this.Npc1.nMove.actType == NPCMove.ActType.Wait && this.Npc2.nMove.actType == NPCMove.ActType.Wait)
			{
				this.Npc1.nMove.actType = NPCMove.ActType.Travel;
				this.Npc2.nMove.actType = NPCMove.ActType.Travel;
			}
			else
			{
				this.Npc1.nMove.actType = NPCMove.ActType.Interval;
				this.Npc2.nMove.actType = NPCMove.ActType.Interval;
			}
			this.Npc1.nMove.searchAngle = this.AAngle;
			this.Npc2.nMove.searchAngle = this.BAngle;
			this.Npc2.sex = CommonStates.SexState.None;
			this.Npc1.sex = CommonStates.SexState.None;

			if (this.Success)
			{
				if (this.Npc2.debuff.perfume <= 0f)
				{
					this.Npc2.libido -= 20f;
				}
				if (this.Npc1.debuff.perfume <= 0f)
				{
					this.Npc1.libido -= 20f;
				}
				this.Npc2.MoralChange(3f, null, NPCManager.MoralCause.None);
				this.Npc1.MoralChange(3f, null, NPCManager.MoralCause.None);
			}

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
		}

		public override bool CanContinue()
		{
			return !this.Destroyed && this.TmpSex != null && this.AreActorsWaiting() && this.AreActorsAlive();
		}

		public override void Destroy()
		{
			// Re-enable the collider ASAP or the NPC *may* fall through the world
			// For some reason, the game AI sometimes simply breaks the current play and forces
			// the Rigidbody to switch gravity to on in the middle of the scene.
			// But as far as I can tell, it always causes the scene Destroy before they start falling
			// so while we deviate from the original process, this will ensure characters don't fall
			// out of the world.
			var col1 = this.Npc1.GetComponent<CapsuleCollider>();
			if (col1 != null)
				col1.enabled = true;
			
			var col2 = this.Npc2.GetComponent<CapsuleCollider>();
			if (col2 != null)
				col2.enabled = true;

			this.Destroyed = true;
			if (this.TmpSex != null)
			{
				GameObject.Destroy(this.TmpSex);
				this.TmpSex = null;
			}
		}

		public override CommonStates[] GetActors()
		{
			return [this.Npc1, this.Npc2];
		}
	}
}
