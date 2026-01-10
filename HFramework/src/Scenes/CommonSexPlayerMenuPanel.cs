#nullable enable

using System;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class CommonSexPlayerMenuPanel : BasePropPanel
	{
		public event EventHandler<int>? OnCaressSelected;
		public event EventHandler<int>? OnInsertSelected;
		public event EventHandler<int>? OnSpeedSelected;
		public event EventHandler<int>? OnPose2Selected;
		public event EventHandler<int>? OnFinishSelected;
		public event EventHandler<int>? OnLeaveSelected;
		public event EventHandler<int>? OnStopSelected;
		public event EventHandler<int>? OnToggleManDisplaySelected;

		public bool CanToggleManDisplay = false;

		public void ShowInitialMenu(string? pose2Text = null)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			if (pose2Text != null)
				this.Options.Add(new MenuItem(pose2Text, () => { this.OnPose2Selected?.Invoke(this, 0); })); // 7
			if (this.CanToggleManDisplay)
				this.Options.Add(new MenuItem(PropPanelConst.Text.ToggleManDisplay, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 8
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowCaressMenu(string? pose2Text = null)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			if (pose2Text != null)
				this.Options.Add(new MenuItem(pose2Text, () => { this.OnPose2Selected?.Invoke(this, 0); })); // 7
			if (this.CanToggleManDisplay)
				this.Options.Add(new MenuItem(PropPanelConst.Text.ToggleManDisplay, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 8
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowInsertMenu(string? pose2Text = null)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Speed, () => { this.OnSpeedSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.Finish, () => { this.OnFinishSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			if (pose2Text != null)
				this.Options.Add(new MenuItem(pose2Text, () => { this.OnPose2Selected?.Invoke(this, 0); })); // 7
			if (this.CanToggleManDisplay)
				this.Options.Add(new MenuItem(PropPanelConst.Text.ToggleManDisplay, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 8
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowFinishMenu(string? pose2Text = null)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			if (pose2Text != null)
				this.Options.Add(new MenuItem(pose2Text, () => { this.OnPose2Selected?.Invoke(this, 0); })); // 7
			if (this.CanToggleManDisplay)
				this.Options.Add(new MenuItem(PropPanelConst.Text.ToggleManDisplay, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 8
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowStopMenu(string? pose2Text = null)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Caress, () => { this.OnCaressSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 5
			if (pose2Text != null)
				this.Options.Add(new MenuItem(pose2Text, () => { this.OnPose2Selected?.Invoke(this, 0); })); // 7
			if (this.CanToggleManDisplay)
				this.Options.Add(new MenuItem(PropPanelConst.Text.ToggleManDisplay, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 8
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
