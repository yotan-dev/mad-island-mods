using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace YotanModCore.NpcTalk
{
	public class NpcTalkManager : MonoBehaviour
	{
		public static NpcTalkManager Instance { get; private set; }

		private static GameObject TalkBtn;

		private static List<TalkButton> RegisteredButtons = [];

		internal static void Init()
		{
			TalkBtn = Initializer.Assets.LoadAsset<GameObject>("TalkBtn");
		}

		public static void RegisterButton(TalkButton btn)
		{
			RegisteredButtons.Add(btn);
		}

		internal static void OnUIManagerAwake()
		{
			NpcTalkManager.Instance = new GameObject("YotanMod_NpcTalk").AddComponent<NpcTalkManager>();

			foreach (var btn in RegisteredButtons)
			{
				var sexBtn = Managers.uiManager.UIManager.npcPanelState[11];
				var newBtn = GameObject.Instantiate(TalkBtn, sexBtn.transform.parent);
				var newBtnComponent = newBtn.GetComponent<Button>();

				newBtnComponent.onClick.AddListener(btn.OnClick);

				var txtObj = newBtn.transform.GetChild(0);
				var lblComponent = txtObj.GetComponent<TextMeshProUGUI>();
				lblComponent.SetText(btn.Text, 0);

				newBtn.SetActive(false);

				btn.Button = newBtnComponent;
				NpcTalkManager.Instance.RealButtons.Add(btn);
			}
		}

		internal static void OnOpen(CommonStates common)
		{
			foreach (var btn in RegisteredButtons)
				btn.Button.gameObject.SetActive(btn.ShouldShow(common));
		}

		private List<TalkButton> RealButtons = [];

		private void OnDestroy()
		{
			foreach (var btn in RealButtons)
				btn.Button = null;
		}
	}
}
