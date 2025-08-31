using System;

namespace YotanModCore
{
	/// <summary>
	/// Result of the <see cref="Initializer.Init(ILogger)"/> method.
	/// This is meant to be used by YotanModCoreLoader only, to work around some limitations of the split.
	/// </summary>
	public class InitializerResult
	{
		public Action<GameManager> Pre_GameManager_Start;
		public Action Pre_SceneScript_SceneChange;
	}
}
