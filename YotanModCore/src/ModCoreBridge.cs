using System;
using YotanModCore.Events;
using YotanModCore.NpcTalk;

namespace YotanModCore
{
	/// <summary>
	/// Bridge between YotanModCore and YotanModCoreLoader.
	/// Result of <see cref="Initializer.Init(ILogger)"/> method.
	///
	/// This class should be kept at the bare minimum and just call appropriate methods in other classes.
	///
	/// NOTE: This is meant to be used by YotanModCoreLoader only, to work around some limitations of the split.
	/// DO NOT USE THIS ELSEWHERE!
	/// </summary>
	public class ModCoreBridge
	{
		/// <summary>
		/// Trigger to Game Start. This is meant to be called by YotanModCoreLoader.
		/// </summary>
		/// <param name="__instance"></param>
		public void Pre_GameManager_Start(GameManager __instance)
		{
			__instance.StartCoroutine(GameLifecycleEvents.GameStartHandler(__instance));
		}

		/// <summary>
		/// Trigger to Scene Change. This is meant to be called by YotanModCoreLoader.
		/// </summary>
		public void Pre_SceneScript_SceneChange()
		{
			GameLifecycleEvents.GameEndHandler();
		}

		/// <summary>
		/// This is meant to be called by YotanModCoreLoader.
		/// </summary>
		public void Post_UIManager_Awake(UIManager __instance)
		{
			NpcTalkManager.OnUIManagerAwake();
		}

		/// <summary>
		/// This is meant to be called by YotanModCoreLoader.
		/// </summary>
		public void Post_UIManager_NPCPanelStateChange(CommonStates common)
		{
			NpcTalkManager.OnOpen(common);
		}
	}
}
