using System.Collections;
using ExtendedHSystem.Scenes;

namespace ExtendedHSystem.Performer
{
	public class SexPerformer
	{
		public readonly SexPerformerInfo Info;

		private readonly ISceneController Controller;

		public SexPerformer(SexPerformerInfo info, ISceneController controller)
		{
			this.Info = info;
			this.Controller = controller;
		}

		public IEnumerator Perform(ActionType action)
		{
			if (this.Info.Actions.TryGetValue(action, out var executor))
				yield return executor(this.Controller);
			else
				PLogger.LogError("No executor found for action " + action);

			yield break;
		}
	}
}