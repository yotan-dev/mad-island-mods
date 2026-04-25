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
			if (!this.Context.ScriptPlace.HasObject()) {
				PLogger.LogWarning("PrepareSexObject: Sex place has no object - it can't be prepared (you probably should not have this node?)");
				return State.Failure;
			}

			if (this.Context.ScriptPlace.IsInUse()) {
				return State.Failure;
			}

			this.Context.ScriptPlace.SetUser(this.Context.Actors[this.User].Common.gameObject);

			this.Context.TmpSexAnim = this.Instantiator.GetSkeletonAnimation(this.Context.ScriptPlace.GetObject());
			this.AppearanceSetter.SetAppearance(this.Context.ScriptPlace.GetObject(), this.Context);

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
