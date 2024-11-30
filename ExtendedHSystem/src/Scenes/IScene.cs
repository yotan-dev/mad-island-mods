namespace ExtendedHSystem.Scenes
{
	public interface IScene
	{
		void Init(ISceneController controller);
		bool CanContinue();
	}
}
