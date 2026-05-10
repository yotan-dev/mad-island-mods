namespace HFramework.ScriptNodes.PrefabSetup
{
	[ScriptNode("HFramework", "Prefab Setup/Set Script Place User")]
	public class SetScriptPlaceUser : Action
	{
		[ActorIndex]
		public int UserActor;

		protected override void OnStart() {
		}

		protected override State OnUpdate() {
			if (this.Context.ScriptPlace.IsInUse()) {
				return State.Failure;
			}

			this.Context.ScriptPlace.SetUser(this.Context.Actors[this.UserActor].Common.gameObject);
			this.Context.Actors[this.UserActor].Common.transform.position = this.Context.ScriptPlace.GetCharacterPosition();
			return State.Success;
		}

		protected override void OnStop() {
		}
	}
}
