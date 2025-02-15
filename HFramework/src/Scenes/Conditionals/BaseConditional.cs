namespace HFramework.Scenes.Conditionals
{
	public abstract class BaseConditional : IConditional
	{
		public abstract bool Pass(IScene scene);

		public abstract bool Pass(CommonStates from, CommonStates to);
	}
}
