using UnityEngine;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[ScriptNode("HFramework", "Prefab Setup/Prepare Sex Object")]
	public class PrepareSexObject : Action
	{
		[SerializeReference, Subclass]
		public PrefabInstantiator Instantiator;

		[SerializeReference, Subclass]
		public AppearanceSetter AppearanceSetter;

		[ActorIndex]
		public int User;

		protected override void OnStart() {
		}

		protected override State OnUpdate() {
			if (this.Context.SexPlace == null) {
				PLogger.LogWarning("PrepareSexObject: Sex place is null");
				return State.Failure;
			}

			if (this.Context.SexPlace.user != null) {
				return State.Failure;
			}

			this.Context.SexPlace.user = this.Context.Actors[this.User].Common.gameObject;

			this.Context.TmpSexAnim = this.Instantiator.GetSkeletonAnimation(this.Context.SexPlace.gameObject);
			this.AppearanceSetter.SetAppearance(this.Context.SexPlace.gameObject, this.Context);

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
