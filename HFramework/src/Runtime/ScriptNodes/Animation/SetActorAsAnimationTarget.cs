namespace HFramework.ScriptNodes.Animation
{
	[ScriptNode("HFramework", "Animation/Set Actor as Animation Target")]
	public class SetActorAsAnimationTarget : Action
	{
		[ActorIndex]
		public int ActorIndex = 0;

		protected override void OnStart() {

		}

		protected override State OnUpdate() {
			this.Context.TmpSexAnim = this.Context.Actors[this.ActorIndex].Common.anim;

			return State.Success;
		}

		protected override void OnStop() {

		}
	}
}
