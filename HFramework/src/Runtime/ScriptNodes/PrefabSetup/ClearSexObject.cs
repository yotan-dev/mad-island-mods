using UnityEngine;
using HFramework.ScriptNodes.PrefabSetup.SexObjectClearers;

namespace HFramework.ScriptNodes.PrefabSetup
{
	/// <summary>
	/// Clears a sex object previously set for a sex interaction (removes user, resets skin and slots, calls clearer).
	/// </summary>
	[ScriptNode("HFramework", "Prefab Setup/Clear Sex Object")]
	public class ClearSexObject : Action
	{
		public string Skin = "default";

		[SerializeReference, Subclass]
		public SexObjectClearer Clearer = new NoOpClearer();

		protected override void OnStart() {
		}

		protected override State OnUpdate() {
			if (this.Context.ScriptPlace.IsGround()) {
				PLogger.LogWarning("PrepareSexObject: Sex place is ground - no need to prepare");
				return State.Failure;
			}

			this.Context.ScriptPlace.ClearUser();

			this.Context.TmpSexAnim.skeleton.SetSkin(this.Skin);
			this.Context.TmpSexAnim.skeleton.SetSlotsToSetupPose();

			this.Clearer.Clear(this.Context);

			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
