using System.Collections;

namespace ExtendedHSystem.Scenes
{
	public interface IScene
	{
		IEnumerator Run();
		void Init(ISceneController controller);
		bool CanContinue();
		void Destroy();
	}
}
