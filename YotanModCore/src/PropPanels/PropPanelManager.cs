using UnityEngine;
using YotanModCore.Consts;

namespace YotanModCore.PropPanels
{
	public class PropPanelManager
	{
		public static PropPanelManager Instance { get; } = new PropPanelManager();

		private BasePropPanel CurrentPanel;

		/// <summary>
		/// Opens the panel at position, making it the active panel.
		/// </summary>
		/// <param name="panel"></param>
		/// <param name="pos"></param>
		public void Open(BasePropPanel panel, Vector3 pos)
		{
			if (CurrentPanel == panel) {
				PLogger.LogWarning("Open: panel is already opened");
				return;
			}

			if (CurrentPanel != null) {
				PLogger.LogWarning("Open: another panel is already open");
				this.Close(CurrentPanel);
			}

			CurrentPanel = panel;
			Managers.mn.uiMN.propActProgress = PropPanelConst.Type.Custom;
			Managers.mn.uiMN.PropPanelHideAll();
			Managers.mn.uiMN.PropPanelVisible(true, pos);

			this.DrawOptions();
		}

		/// <summary>
		/// Hides the panel, but keeps it active.
		/// </summary>
		/// <param name="panel"></param>
		public void Hide(BasePropPanel panel)
		{
			if (CurrentPanel != panel) {
				PLogger.LogWarning("Hide: trying to hide a panel that is not the current one.");
				return;
			}

			Managers.mn.uiMN.propPanel.SetActive(false);
		}

		/// <summary>
		/// Makes panel visible
		/// </summary>
		/// <param name="panel"></param>
		public void Show(BasePropPanel panel)
		{
			if (CurrentPanel != panel) {
				PLogger.LogWarning("Show: trying to show a panel that is not the current one.");
				return;
			}

			Managers.mn.uiMN.propPanel.SetActive(true);
		}

		/// <summary>
		/// Closes the panel, discarding it.
		/// </summary>
		/// <param name="panel"></param>
		public void Close(BasePropPanel panel)
		{
			if (CurrentPanel != panel) {
				PLogger.LogWarning("Close: trying to close a panel that is not the current one.");
				return;
			}

			Managers.mn.uiMN.propPanel.SetActive(false);

			CurrentPanel = null;
		}

		/// <summary>
		/// Draws the options for the current panel into the screen.
		/// </summary>
		public void DrawOptions()
		{
			if (this.CurrentPanel == null) {
				PLogger.LogError("DrawOptions: CurrentPanel is null");
				return;
			}

			Managers.mn.uiMN.PropPanelHideAll();

			int btnCount = 0;
			foreach (var option in this.CurrentPanel.Options) {
				Managers.mn.uiMN.PropPanelStateChange(btnCount, option.TextId, btnCount, true);
				btnCount++;
			}
		}

		/// <summary>
		/// Event triggered when the player clicks a button.
		/// Runs the related action.
		/// </summary>
		/// <param name="buttonId"></param>
		public void OnButtonClicked(int buttonId)
		{
			if (this.CurrentPanel == null) {
				PLogger.LogError("OnButtonClicked: CurrentPanel is null");
				return;
			}

			var options = this.CurrentPanel.Options;
			if (buttonId >= options.Count)
				return;

			this.CurrentPanel.Options[buttonId].Action();
		}
	}
}