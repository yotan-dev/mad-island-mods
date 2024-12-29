#nullable enable

namespace HFramework.Scenes.Conditionals
{
	public interface IConditional
	{
		bool Pass(IScene2 scene);

		bool Pass(CommonStates from, CommonStates? to);
	}
}
