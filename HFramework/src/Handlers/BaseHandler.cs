using System.Collections;
using HFramework.Scenes;

namespace HFramework.Handlers
{
	/// <summary>
	/// Base class for creating scene handlers/hooks. For example:
	/// - Playing animations
	/// - Counting changes
	/// - Modifying scenes based on events
	/// - etc
	/// 
	/// It should be extended and "Run" overridden.
	/// 
	/// "ShouldStop" may be set by child classes in order to stop a scene from playing.
	/// </summary>
	public abstract class BaseHandler
	{
		protected readonly IScene2 Scene;
		
		protected readonly IScene OldScene;

		protected bool ShouldStop = false;

		public BaseHandler(IScene2 scene)
		{
			this.Scene = scene;
		}

		public BaseHandler(IScene scene)
		{
			this.OldScene = scene;
		}

		/// <summary>
		/// The method that actually does the work
		/// </summary>
		/// <returns></returns>
		protected abstract IEnumerator Run();

		public IEnumerator Handle()
		{
			yield return this.Run();

			if (this.ShouldStop)
			{
				this.Scene?.Destroy();
				this.OldScene?.Destroy();
			}
		}
	}
}
