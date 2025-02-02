using UnityEngine.UI;

namespace YotanModCore.NpcTalk
{
	public abstract class TalkButton
	{
		public string Text { get; private set; }

		public Button Button { get; set; }

		public TalkButton(string text)
		{
			this.Text = text;
		}

		public abstract void OnClick();

		public abstract bool ShouldShow(CommonStates common);
	}
}
