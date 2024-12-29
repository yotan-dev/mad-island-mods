using System.Collections;

namespace HFramework.Scenes
{
	public interface IScene
	{
		IEnumerator Run();
		void Init(ISceneController controller);
		bool CanContinue();
		void Destroy();
	}
}
