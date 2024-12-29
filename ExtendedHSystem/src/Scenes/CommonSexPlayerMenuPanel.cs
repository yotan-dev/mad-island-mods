using System;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace ExtendedHSystem.Scenes
{
	public class CommonSexPlayerMenuPanel : BasePropPanel
	{
		public event EventHandler<int> OnCaressSelected;
		public event EventHandler<int> OnInsertSelected;
		public event EventHandler<int> OnSpeedSelected;
		public event EventHandler<int> OnPose2Selected;
		public event EventHandler<int> OnFinishSelected;
		public event EventHandler<int> OnLeaveSelected;
		public event EventHandler<int> OnStopSelected;

		public void ShowInitialMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowCaressMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowInsertMenu(bool hasPose2)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Speed, () => { this.OnSpeedSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.Finish, () => { this.OnFinishSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			if (hasPose2)
				this.Options.Add(new MenuItem(PropPanelConst.Text.Pose2, () => { this.OnPose2Selected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowFinishMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowStopMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
