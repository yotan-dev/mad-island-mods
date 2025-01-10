#nullable enable

using System.Collections;
using HFramework.Scenes;
using UnityEngine;
using YotanModCore;
using YotanModCore.Extensions;

namespace HFramework.Handlers
{
	/// <summary>
	/// Makes actors move to place in time or fail
	/// </summary>
	public class MoveToPlace : BaseHandler
	{
		private readonly CommonStates[] Actors;

		private readonly Vector3 Position;

		private readonly SexPlace? Place;

		public MoveToPlace(
			IScene scene,
			CommonStates[] actors,
			Vector3 position,
			SexPlace? place
		) : base(scene)
		{
			this.Actors = actors;
			this.Position = position;
			this.Place = place;
		}

		private bool AreActorsAlive()
		{
			var alive = true;
			foreach (var actor in this.Actors)
				alive = alive && actor.dead == 0;

			return alive;
		}

		private bool AreActorsWaiting()
		{
			var player = CommonUtils.GetActivePlayer();
			bool waiting = true;
			foreach (var actor in this.Actors)
			{
				if (actor != player)
				{
					// Official code only checks for Interval in delivery, but it looks like
					// Interval is set in MovePosition , so it would be safer to check here too..
					waiting = waiting && (
						actor.nMove.actType == NPCMove.ActType.Wait
						|| actor.nMove.actType == NPCMove.ActType.Interval
					);
				}
			}

			return waiting;
		}

		private bool IsPlaceFree()
		{
			return this.Place?.user == null;
		}

		private bool IsNpcAtPos(CommonStates npc)
		{
			// Delivery checks for 0.5 instead of 1, but 1 for everything should be good enough.
			if (Vector3.Distance(npc.gameObject.transform.position, this.Position) > 1f)
			{
				if (npc.anim.GetCurrentAnimName() != "A_walk" && npc.nMove.actType != NPCMove.ActType.Interval)
					npc.anim.state.SetAnimation(0, "A_walk", true);

				return false;
			}

			if (npc.nMove.common.anim.GetCurrentAnimName() != "A_idle")
				npc.nMove.common.anim.state.SetAnimation(0, "A_idle", true);

			return true;
		}

		private bool DidActorsReachPos()
		{
			var reached = true;
			foreach (var actor in this.Actors)
				reached = reached && this.IsNpcAtPos(actor);

			return reached;
		}

		protected override IEnumerator Run()
		{
			float animTime = 30f;

			foreach (var actor in this.Actors)
			{
				Managers.mn.sexMN.StartCoroutine(
					Managers.mn.story.MovePosition(actor.gameObject, this.Position, 2f, "A_walk", true, false, 0.1f, 40f)
				);
			}

			while (
				animTime > 0f
				&& this.AreActorsWaiting()
				&& !this.DidActorsReachPos()
				&& this.IsPlaceFree()
				&& this.AreActorsAlive()
			)
			{
				animTime -= Time.deltaTime;
				yield return null;
			}

			if (animTime <= 0f)
			{
				this.ShouldStop = true;
				yield break;
			}

			// Force the position so StoryManager.MovePosition properly finishes
			foreach (var actor in this.Actors)
				actor.gameObject.transform.position = this.Position;

			// Let StoryManager finish its work.
			yield return null;
		}
	}
}
