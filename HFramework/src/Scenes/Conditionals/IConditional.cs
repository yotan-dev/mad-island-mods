#nullable enable

namespace HFramework.Scenes.Conditionals
{
	public interface IConditional
	{
		bool Pass(IScene scene);

		bool Pass(CommonStates from, CommonStates? to);
	}
}
