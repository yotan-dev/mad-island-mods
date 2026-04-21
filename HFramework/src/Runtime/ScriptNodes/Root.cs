namespace HFramework.ScriptNodes
{
	[Experimental]
	[ScriptNode("HFramework", "Flow/Root")]
	public class Root : ScriptNode
	{
		public ScriptNode Child;

		public ScriptNode TeardownNode;

		private State MainFinalState;

		private bool MainFinished;

		protected override void OnStart() {
			MainFinalState = State.Running;
			MainFinished = false;
		}

		protected override void OnStop() {

		}

		protected override State OnUpdate() {
			if (!MainFinished) {
				var mainState = Child?.Update() ?? State.Success;
				this.Context.MainNodeState = mainState;
				if (mainState == State.Running) {
					return State.Running;
				}

				MainFinalState = mainState;
				MainFinished = true;
			}

			if (TeardownNode != null) {
				var teardownState = TeardownNode.Update();
				if (teardownState == State.Running) {
					return State.Running;
				}
			}

			return MainFinalState;
		}

		public override void Terminate(bool fromOutside = true) {
			Child?.Terminate(fromOutside);
			TeardownNode?.Terminate(fromOutside);
			base.Terminate(fromOutside);
		}

		public override ScriptNode Clone(CommonContext context) {
			var node = Instantiate(this);
			node.Context = context;
			node.Child = Child != null ? Child.Clone(context) : null;
			node.TeardownNode = TeardownNode != null ? TeardownNode.Clone(context) : null;

			return node;
		}
	}
}
