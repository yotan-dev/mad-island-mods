using System.Collections;
using System.Collections.Generic;
using HFramework.Hook;
using HFramework.Performer;
using Spine.Unity;
using UnityEngine;
using YotanModCore;
using YotanModCore.Consts;
using YotanModCore.Extensions;

namespace HFramework.Scenes
{
	/// <summary>
	/// Onani stands for Masturbation
	/// </summary>
	public class OnaniNPC : IScene, IScene2
	{
		public static readonly string Name = "OnaniNPC";

		public static class StepNames
		{
			public const string Main = "Main";
			public const string Speed1 = "Speed1";
			public const string Speed2 = "Speed2";
			public const string Finish = "Finish";
		}

		public readonly CommonStates Npc;

		public readonly SexPlace Place;

		public readonly float UpMoral;

		private float? SearchAngle;

		private GameObject TmpSex;

		private ISceneController Controller;

		private SkeletonAnimation Anim;

		private SexPerformer Performer;

		private readonly List<SceneEventHandler> EventHandlers = new List<SceneEventHandler>();

		public OnaniNPC(CommonStates npc, SexPlace sexPlace, float upMoral)
		{
			this.Place = sexPlace;
			this.Npc = npc;
			this.UpMoral = upMoral;
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

		private bool IsActorAlive()
		{
			return this.Npc.dead == 0;
		}

		private bool IsActorWaiting()
		{
			return this.Npc.nMove.actType == NPCMove.ActType.Wait;
		}

		private bool IsPlaceFree()
		{
			return this.Place?.user == null;
		}

		private bool IsNpcAtPos(CommonStates npc, Vector3 pos)
		{
			if (Vector3.Distance(npc.gameObject.transform.position, pos) > 1f)
			{
				if (npc.anim.GetCurrentAnimName() != "A_walk")
					npc.anim.state.SetAnimation(0, "A_walk", true);

				return false;
			}

			if (npc.nMove.common.anim.GetCurrentAnimName() != "A_idle")
				npc.nMove.common.anim.state.SetAnimation(0, "A_idle", true);

			return true;
		}

		private void DisableLiveNpc()
		{
			this.Npc.nMove.RBState(false);

			MeshRenderer mesh = this.Npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = false;

			CapsuleCollider coll = this.Npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = false;
		}

		private void EnableLiveNpc()
		{
			CapsuleCollider coll = this.Npc.GetComponent<CapsuleCollider>();
			if (coll != null)
				coll.enabled = true;
			
			MeshRenderer mesh = this.Npc.anim.GetComponent<MeshRenderer>();
			if (mesh != null)
				mesh.enabled = true;
		}

		private bool SetupScene()
		{
			this.Performer = ScenesManager.Instance.GetPerformer(this, PerformerScope.Sex, this.Controller);
			if (this.Performer == null)
				return false;

			var sexPos = this.Place?.transform?.position ?? this.Npc.transform.position;
			this.TmpSex = GameObject.Instantiate<GameObject>(this.Performer.Info.SexPrefabSelector.GetPrefab(), sexPos, Quaternion.identity);
			if (this.TmpSex == null)
				return false;

			this.Anim = this.TmpSex.transform.Find("Scale/Anim").gameObject.GetComponent<SkeletonAnimation>();

			this.Npc.transform.position = sexPos;
			this.SearchAngle = this.Npc.nMove.searchAngle;
			this.Npc.nMove.searchAngle = 180f;

			if (this.Place != null)
			{
				this.Place.user = this.Npc.gameObject;
				var placePos = this.Place.transform.Find("pos").position;
				if (placePos != null)
				{
					this.TmpSex.transform.position = placePos;
				}
				else
				{
					Debug.LogError($"OnaniNPC: Place is missing 'Pos'. PlaceName: ${this.Place.gameObject.name}");
					this.TmpSex.transform.position = sexPos;
				}
			}

			this.DisableLiveNpc();

			if (this.Npc.sexInfo.Length > 3)
				this.Npc.sexInfo[SexInfoIndex.Masturbation]++;

			Managers.mn.randChar.SetCharacter(this.TmpSex, this.Npc, null);

			return true;
		}

		private IEnumerator MoveToPlace(Vector3 pos)
		{
			float animTime = 30f;

			Managers.mn.sexMN.StartCoroutine(Managers.mn.story.MovePosition(this.Npc.gameObject, pos, 2f, "A_walk", true, false, 0.1f, 40f));

			bool reached = false;
			while (
				animTime > 0f
				&& this.IsActorWaiting()
				&& !reached
				&& this.IsPlaceFree()
				&& this.IsActorAlive()
			)
			{
				animTime -= Time.deltaTime;
				reached = this.IsNpcAtPos(this.Npc, pos);
				yield return false;
			}

			if (animTime <= 0f)
				this.Destroy();
		}

		private void Teardown()
		{
			this.Npc.sex = CommonStates.SexState.None;
			if (this.Npc.nMove.actType == NPCMove.ActType.Wait)
				this.Npc.nMove.actType = NPCMove.ActType.Travel;
			else
				this.Npc.nMove.actType = NPCMove.ActType.Interval;

			if (this.Place != null)
				this.Place.user = null;

			this.EnableLiveNpc();

			if (this.TmpSex != null)
			{
				GameObject.Destroy(this.TmpSex);
				this.TmpSex = null;
			}

			if (this.SearchAngle != null)
				this.Npc.nMove.searchAngle = this.SearchAngle.Value;
		}

		private IEnumerator Perform()
		{
			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed1);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Speed1);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed1);
			if (!this.CanContinue())
				yield break;


			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Speed2);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Speed2);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Speed2);
			if (!this.CanContinue())
				yield break;


			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Finish);
			if (!this.CanContinue())
				yield break;

			yield return this.Performer.Perform(ActionType.Finish);
			if (!this.CanContinue())
				yield break;

			if (this.Npc.debuff.perfume <= 0f)
			{
				this.Npc.libido -= 20f;
				this.Npc.MoralChange(this.UpMoral, null, NPCManager.MoralCause.None);
			}

			yield return this.Performer.Perform(ActionType.FinishIdle);

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Finish);
			if (!this.CanContinue())
				yield break;

			foreach (var handler in this.EventHandlers) {
				foreach (var x in handler.AfterMasturbate(this, this.Npc))
					yield return x;
			}
		}

		public IEnumerator Run()
		{
			NPCMove nMove = this.Npc.nMove;
			Vector3 pos = this.Npc.transform.position;
			if (this.Place != null)
				pos = this.Place.transform.position;

			nMove.actType = NPCMove.ActType.Wait;
			this.Npc.sex = CommonStates.SexState.Playing;

			GameObject emoA = Managers.mn.fxMN.GoEmotion(13, this.Npc.gameObject, Vector3.zero);

			var reachedPos = false;
			yield return this.MoveToPlace(pos);
			if (!this.CanContinue())
				yield break;

			emoA.SetActive(false);

			if (!reachedPos || !this.IsPlaceFree())
			{
				this.Teardown();
				yield break;
			}

			if (!this.SetupScene())
			{
				this.Teardown();
				yield break;
			}

			yield return HookManager.Instance.RunStepStartHook(this, StepNames.Main);
			if (!this.CanContinue())
			{
				yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);
				this.Teardown();
				yield break;
			}

			yield return this.Perform();

			yield return HookManager.Instance.RunStepEndHook(this, StepNames.Main);

			this.Teardown();
		}

		public bool CanContinue()
		{
			return this.TmpSex != null && this.IsActorWaiting() && this.IsActorAlive();
		}

		public void Destroy()
		{
			GameObject.Destroy(this.TmpSex);
			this.TmpSex = null;
		}

		public string GetName()
		{
			return OnaniNPC.Name;
		}

		public CommonStates[] GetActors()
		{
			return [this.Npc];
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
