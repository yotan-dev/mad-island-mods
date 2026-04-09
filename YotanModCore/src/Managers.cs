using YotanModCore.Wrappers;

namespace YotanModCore
{
	/// <summary>
	/// Provides simplified access to game Managers.
	/// </summary>
	public class Managers
	{
		public static ManagersScript mn;

		public static WrappedUIManager uiManager;

		public static SexManager sexMN => mn.sexMN;

		public static FXManager fxMN => mn.fxMN;

		public static StoryManager storyMN => mn.story;
	}
}
