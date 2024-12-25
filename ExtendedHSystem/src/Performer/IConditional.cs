using ExtendedHSystem.Scenes;

namespace ExtendedHSystem.Performer
{
	public interface IConditional
	{
		bool Pass(IScene2 scene);
	}
}