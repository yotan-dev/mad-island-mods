using System.Collections;

namespace ExtendedHSystem.Performer
{
	public class SexPerformer
	{
		public readonly SexPerformerInfo Info;

		private readonly ISceneController Controller;

		public ActionType CurrentAction { get; private set; } = ActionType.StartIdle;

		public int CurrentPose { get; private set; } = 1;

		public SexPerformer(SexPerformerInfo info, ISceneController controller)
		{
			this.Info = info;
			this.Controller = controller;
		}

		public IEnumerator Perform(ActionType action)
		{
			this.CurrentAction = action;
			if (!this.Info.Actions.TryGetValue(new ActionKey(action, this.CurrentPose), out var value))
			{
				if (!this.Info.Actions.TryGetValue(new ActionKey(action, 1), out value))
				{
					PLogger.LogError($"No info found for action {action} / Pose {this.CurrentPose} (also not found for pose 1)");
					yield break;
				}

				this.CurrentPose = 1;
			}

			switch (value.PlayType)
			{
				case PlayType.Loop:
					yield return this.Controller.LoopAnimation(value.AnimationName);
					break;

				case PlayType.Once:
					yield return this.Controller.PlayOnceStep_New(value.AnimationName);
					break;

				default:
					PLogger.LogError("Unknown play type " + value.PlayType);
					break;
			}
		}

		public bool HasAlternativePose()
		{
			var newPose = this.CurrentPose == 1 ? 2 : 1;
			return this.Info.Actions.ContainsKey(new ActionKey(this.CurrentAction, newPose));
		}

		public IEnumerator ChangePose()
		{
			this.CurrentPose = this.CurrentPose == 1 ? 2 : 1;
			yield return this.Perform(this.CurrentAction);
		}
	}
}