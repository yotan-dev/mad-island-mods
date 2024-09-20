using TMPro;
using UnityEngine;

namespace YotanModCore.Wrappers
{
	/// <summary>
	/// Wrapper over UIManager with some accessors
	/// </summary>
	public class WrappedUIManager
	{
		public UIManager UIManager { get; set; }

		public TextMeshProUGUI getText;

		public GameObject needPanel;

		public WrappedUIManager(UIManager uiManager) {
			this.UIManager = uiManager;
			this.getText = uiManager.itemDescPanel.transform.Find("Image/Text").GetComponent<TextMeshProUGUI>();
			this.needPanel = uiManager.itemDescPanel.transform.Find("NeedPanel").gameObject;
		}
	}
}