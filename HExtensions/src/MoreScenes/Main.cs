using YotanModCore.Events;
using YotanModCore.NpcTalk;

namespace HExtensions.MoreScenes
{
	public class Main
	{
		public static SexStartController Ctrler;

		public void Init()
		{
			NpcTalkManager.RegisterButton(new SexRapeButton());
			GameLifecycleEvents.OnGameStartEvent += () => Ctrler = SexStartController.Create();
			GameLifecycleEvents.OnGameEndEvent += () => Ctrler = null;
		}
	}
}
