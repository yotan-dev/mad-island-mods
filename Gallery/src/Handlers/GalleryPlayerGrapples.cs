using System.Collections;
using HFramework.Handlers;
using HFramework.Scenes;
using UnityEngine;

namespace Gallery.Handlers
{
	public class GalleryPlayerGrapples : BaseHandler
	{
		private readonly CommonStates Girl;

		public GalleryPlayerGrapples(IScene scene, CommonStates girl) : base(scene)
		{
			this.Girl = girl;
		}

		protected override IEnumerator Run()
		{
			var performer = this.Scene.GetPerformer();
			// string attackAnimName = sexType + "Attack_attack";
			while (this.Scene.CanContinue())
			{
				bool flag = false;
				if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
				{
					flag = true;
				}
				else if (
					performer.HasAction(HFramework.Performer.ActionType.Attack)
					&& performer.CurrentAction == HFramework.Performer.ActionType.Attack
					&& !performer.IsPerformanceRunning()
				)
				{
					performer.PerformBg(HFramework.Performer.ActionType.Battle);
				}


				if (
					performer.HasAction(HFramework.Performer.ActionType.Attack)
					&& performer.CurrentAction != HFramework.Performer.ActionType.Attack
					&& flag
				)
				{
					performer.PerformBg(HFramework.Performer.ActionType.Attack);
				}

				yield return null;
			}

			this.Girl.faint = 0;
		}
	}
}
