using System.Collections;
using HFramework.Scenes;

namespace HFramework
{
	public interface ISceneController
	{
		void SetScene(IScene scene);

		IScene GetScene();

		bool IsAnimRunning();

		IEnumerator LoopAnimation(string name);
		
		void LoopAnimationBg(string name);
		
		IEnumerator PlayTimedStep(string name, float time);
		
		IEnumerator PlayOnceStep(string name, bool skippable = false);
		
		void PlayOnceStepBg(string name);

		IEnumerator WaitForInput();
		
		void SetLockedAnimation(string name);
	}
}
