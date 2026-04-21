using UnityEngine;
using HFramework.ScriptNodes.PrefabSetup.SexObjectClearers;

namespace HFramework.ScriptNodes.PrefabSetup
{
	[ScriptNode("HFramework", "Prefab Setup/Clear Sex Object")]
	public class ClearSexObject : Action
	{
		public string Skin = "default";

		[SerializeReference, Subclass]
		public SexObjectClearer Clearer = new NoOpClearer();

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

			this.Context.SexPlace.user = null;

			this.Context.TmpSexAnim.skeleton.SetSkin(this.Skin);
			this.Context.TmpSexAnim.skeleton.SetSlotsToSetupPose();

			this.Clearer.Clear(this.Context);

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
