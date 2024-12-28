#nullable enable

using System.Collections;
using ExtendedHSystem.Hook;
using ExtendedHSystem.ParamContainers;

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

		public ActionValue? GetActionValue(ActionType action)
		{
			if (!this.Info.Actions.TryGetValue(new ActionKey(action, this.CurrentPose), out var value))
			{
				if (!this.Info.Actions.TryGetValue(new ActionKey(action, 1), out value))
				{
					PLogger.LogError($"No info found for action {action} / Pose {this.CurrentPose} (also not found for pose 1)");
					return null;
				}

				this.CurrentPose = 1;
			}

			return value;
		}

		public IEnumerator Perform(ActionType action, float? loopTime = null)
		{
			this.CurrentAction = action;
			var value = this.GetActionValue(action);
			if (value == null)
				yield break;

			switch (value.PlayType)
			{
				case PlayType.Loop:
					if (loopTime.HasValue)
						yield return this.Controller.PlayTimedStep(value.AnimationName, loopTime.Value);
					else
						yield return this.Controller.LoopAnimation(value.AnimationName);
					break;

				case PlayType.Once:
					yield return this.Controller.PlayOnceStep(value.AnimationName);
					break;

				default:
					PLogger.LogError("Unknown play type " + value.PlayType);
					break;
			}

			var actors = this.Controller.GetScene().GetActors();
			var scene = this.Controller.GetScene();
			var from = actors.Length >= 1 ? actors[0] : null;
			var to = actors.Length >= 2 ? actors[1] : null;
			foreach (var eventName in value.Events)
				yield return HookManager.Instance.RunEventHook(scene, eventName, new FromToParams(from, to));
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
