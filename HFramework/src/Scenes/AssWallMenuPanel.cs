using System;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class AssWallMenuPanel : BasePropPanel
	{
		public event EventHandler<int> OnInsertSelected;
		public event EventHandler<int> OnSpeedSelected;
		public event EventHandler<int> OnFinishSelected;
		public event EventHandler<int> OnLeaveSelected;
		public event EventHandler<int> OnStopSelected;

		public void ShowInitialMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowInsertMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Speed, () => { this.OnSpeedSelected?.Invoke(this, 0); })); // 5
			this.Options.Add(new MenuItem(PropPanelConst.Text.Finish, () => { this.OnFinishSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowStopMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Move, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowFinishMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
