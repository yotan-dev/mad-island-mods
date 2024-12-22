using System;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace ExtendedHSystem.Scenes
{
	public class ToiletMenuPanel : BasePropPanel
	{
		public event EventHandler<int> OnInsertSelected;
		public event EventHandler<int> OnMoveSelected;
		public event EventHandler<int> OnSpeedSelected;
		public event EventHandler<int> OnFinishSelected;
		public event EventHandler<int> OnLeaveSelected;
		public event EventHandler<int> OnStopSelected;
		
		public event EventHandler<int> OnFaceRevealSelected;

		public event EventHandler<int> OnUrinateSelected;
		public event EventHandler<int> OnStopUrinateSelected;

		public void ShowInitialMenu()
		{
			this.Options.Clear();
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.FaceReveal, () => { this.OnFaceRevealSelected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowInsertMenu()
		{
			this.Options.Clear();
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Move, () => { this.OnMoveSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); })); // 2
			PropPanelManager.Instance.DrawOptions();
		}

		public void ChangeToStopUrinate()
		{
			var idx = this.Options.FindIndex((ConstMenuItem item) => item.TextId == PropPanelConst.Text.Urinate);

			if (idx != -1)
				this.Options[idx] = new ConstMenuItem(PropPanelConst.Text.Stop, () => { this.OnStopUrinateSelected?.Invoke(this, 0); }); // 2

			PropPanelManager.Instance.DrawOptions();
		}

		public void ChangeStopToUrinate()
		{
			var idx = this.Options.FindIndex((ConstMenuItem item) => item.TextId == PropPanelConst.Text.Stop);

			if (idx != -1)
				this.Options[idx] = new ConstMenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); }); // 2

			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowMoveMenu()
		{
			this.Options.Clear();
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Speed, () => { this.OnSpeedSelected?.Invoke(this, 0); })); // 5
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Finish, () => { this.OnFinishSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowStopMenu()
		{
			this.Options.Clear();
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Move, () => { this.OnMoveSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowFinishMenu()
		{
			this.Options.Clear();
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); })); // 2
			this.Options.Add(new ConstMenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
