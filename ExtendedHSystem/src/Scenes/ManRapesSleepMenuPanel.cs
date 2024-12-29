using System;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace ExtendedHSystem.Scenes
{
	public class ManRapesSleepMenuPanel : BasePropPanel
	{
		public event EventHandler<int> OnForcefullyRapeSelected;
		public event EventHandler<int> OnUseSleepPowderSelected;
		public event EventHandler<int> OnGentlyRapeSelected;
		public event EventHandler<int> OnInsertSelected;
		public event EventHandler<int> OnSpeedSelected;
		public event EventHandler<int> OnSpeed2Selected;
		public event EventHandler<int> OnFinishSelected;
		public event EventHandler<int> OnLeaveSelected;

		public void ShowInitialMenu(bool hasSleepPowder)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.ForcefullyRape, () => { this.OnForcefullyRapeSelected?.Invoke(this, 0); })); // 1
			if (hasSleepPowder)
				this.Options.Add(new MenuItem(PropPanelConst.Text.UseSleepPowder, () => { this.OnUseSleepPowderSelected?.Invoke(this, 0); })); // 2 - 2 , 0
			this.Options.Add(new MenuItem(PropPanelConst.Text.GentlyRape, () => { this.OnGentlyRapeSelected?.Invoke(this, 0); })); // 9 - 2 , 1
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowRapeMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 10
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowInsertMenu(bool canSpeed2)
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Speed, () => { this.OnSpeedSelected?.Invoke(this, 0); })); // 4
			
			if (canSpeed2)
				this.Options.Add(new MenuItem(PropPanelConst.Text.Speed2, () => { this.OnSpeed2Selected?.Invoke(this, 0); })); // 6
			
			this.Options.Add(new MenuItem(PropPanelConst.Text.Finish, () => { this.OnFinishSelected?.Invoke(this, 0); })); // 5
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 10
			
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowFinishMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 10
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
