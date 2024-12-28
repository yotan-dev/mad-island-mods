#nullable enable

namespace ExtendedHSystem.Scenes.Conditionals
{
	public interface IConditional
	{
		bool Pass(IScene2 scene);

		bool Pass(CommonStates from, CommonStates? to);
	}
}
