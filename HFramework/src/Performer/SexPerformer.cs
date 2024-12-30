#nullable enable

using System.Collections;
using HFramework.Hook;
using HFramework.ParamContainers;
using YotanModCore;
using YotanModCore.Consts;

namespace HFramework.Performer
{
	public class SexPerformer
	{
		public readonly SexPerformerInfo Info;

		private readonly ISceneController Controller;

		public ActionType CurrentAction { get; private set; } = ActionType.StartIdle;

		public int CurrentPose { get; private set; } = 1;

		public string CurrentSetName { get; private set; } = SexPerformerInfo.DefaultSet;

		public AnimationSet CurrentSet => this.Info.AnimationSets[this.CurrentSetName];

		private int LoopCount = 0;

		public SexPerformer(SexPerformerInfo info, ISceneController controller)
		{
			this.Info = info;
			this.Controller = controller;
		}

		private bool TryGetActionValue(ActionType action, out ActionValue? value, out int pose)
		{
			pose = this.CurrentPose;
			if (!this.CurrentSet.Actions.TryGetValue(new ActionKey(action, this.CurrentPose), out value))
			{
				if (!this.CurrentSet.Actions.TryGetValue(new ActionKey(action, 1), out value))
					return false;

				pose = 1;
			}

			return true;
		}

		public bool HasAction(ActionType action)
		{
			return this.TryGetActionValue(action, out _, out _);
		}

		public ActionValue? GetActionValue(ActionType action, out int pose)
		{
			if (!this.TryGetActionValue(action, out var value, out pose))
			{
				PLogger.LogError($"No info found for action {action} / Pose {this.CurrentPose} / set {this.CurrentSetName} (also not found for pose 1)");
				return null;
			}

			return value;
		}

		public IEnumerator Perform(ActionType action, float? loopTime = null)
		{
			this.LoopCount++;

			if (this.LoopCount == 10)
			{
				PLogger.LogWarning("SexPerformer possibly infinite loop detected (Perform iterated 10 times without landing into an animation). Stopping it.");
				this.LoopCount = 0;
				yield break;
			}

			this.CurrentAction = action;
			var value = this.GetActionValue(action, out var pose);
			if (value == null)
				yield break;

			if (value.AnimationName.StartsWith($"{SexPerformerInfo.AnimSetChangeName}:"))
			{
				var parts = value.AnimationName.Split(':');
				var newSetName = parts[1];
				var newPoseId = parts.Length > 2 ? int.Parse(parts[2]) : this.CurrentPose;

				yield return this.ChangeSet(newSetName, newPoseId);
				yield break;
			}

			this.CurrentPose = pose;

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

			this.LoopCount = 0;
		}

		public bool CanSwitchPose()
		{
			var action = this.GetActionValue(this.CurrentAction, out _);
			if (action != null && !action.CanChangePose)
				return false;

			var newPose = this.CurrentPose == 1 ? 2 : 1;
			return this.CurrentSet.Actions.ContainsKey(new ActionKey(this.CurrentAction, newPose));
		}

		public string? GetAlternativePoseName()
		{
			if (!this.CanSwitchPose())
				return null;

			/* Plugins can expand this */

			return Managers.mn.textMN.actButtonTexts[PropPanelConst.Text.Pose2];
		}

		public IEnumerator ChangePose()
		{
			this.CurrentPose = this.CurrentPose == 1 ? 2 : 1;
			yield return this.Perform(this.CurrentAction);
		}

		public bool HasSet(string setName)
		{
			return this.Info.AnimationSets.ContainsKey(setName);
		}

		public IEnumerator ChangeSet(string setName, int? pose = null)
		{
			if (!this.HasSet(setName))
			{
				PLogger.LogError($"Unknown animation set {setName}");
				yield break;
			}

			this.CurrentSetName = setName;
			if (pose.HasValue)
				this.CurrentPose = pose.Value;

			yield return this.Perform(this.CurrentAction);

			yield return HookManager.Instance.RunEventHook(this.Controller.GetScene(), EventNames.OnAnimSetChanged, this);
		}
	}
}
