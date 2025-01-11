using System.Collections;
using HFramework.Performer;
using Spine.Unity;

namespace HFramework.Scenes
{
	/// <summary>
	/// Holds some common functionality for all scenes
	/// </summary>
	public abstract class BaseScene : IScene
	{
		protected readonly string SceneName;

		protected SkeletonAnimation CommonAnim;

		protected ISceneController Controller;

		protected SexPerformer Performer;

		protected bool Destroyed = false;

		public BaseScene(string name)
		{
			this.SceneName = name;
		}

		public virtual void Init(ISceneController controller)
		{
			this.Controller = controller;
			controller.SetScene(this);
		}

		public virtual bool CanContinue()
		{
			return !this.Destroyed;
		}

		public virtual void Destroy()
		{
			this.Destroyed = true;
		}

		public virtual string ExpandAnimationName(string originalName)
		{
			return originalName;
		}

		public virtual string GetName()
		{
			return this.SceneName;
		}

		public virtual SexPerformer GetPerformer()
		{
			return this.Performer;
		}

		public virtual SkeletonAnimation GetSkelAnimation()
		{
			return this.CommonAnim;
		}

		public abstract CommonStates[] GetActors();

		public abstract IEnumerator Run();
	}
}
