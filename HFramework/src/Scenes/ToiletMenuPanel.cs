using System;
using YotanModCore.Consts;
using YotanModCore.PropPanels;

namespace HFramework.Scenes
{
	public class ToiletMenuPanel : BasePropPanel
	{
		private const int UrinateMeta = 1;

		private const int StopUrinateMeta = 2;

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
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); }, UrinateMeta)); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.FaceReveal, () => { this.OnFaceRevealSelected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowInsertMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Move, () => { this.OnMoveSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); }, UrinateMeta)); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.FaceReveal, () => { this.OnFaceRevealSelected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}

		public void ChangeToStopUrinate()
		{
			var idx = this.Options.FindIndex((MenuItem item) => item.Meta == UrinateMeta);

			if (idx != -1)
				this.Options[idx] = new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopUrinateSelected?.Invoke(this, 0); }, StopUrinateMeta); // 2

			PropPanelManager.Instance.DrawOptions();
		}

		public void ChangeStopToUrinate()
		{
			var idx = this.Options.FindIndex((MenuItem item) => item.Meta == StopUrinateMeta);

			if (idx != -1)
				this.Options[idx] = new MenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); }, UrinateMeta); // 2

			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowMoveMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Stop, () => { this.OnStopSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Speed, () => { this.OnSpeedSelected?.Invoke(this, 0); })); // 5
			this.Options.Add(new MenuItem(PropPanelConst.Text.Finish, () => { this.OnFinishSelected?.Invoke(this, 0); })); // 6
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.FaceReveal, () => { this.OnFaceRevealSelected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowStopMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Move, () => { this.OnMoveSelected?.Invoke(this, 0); })); // 4
			this.Options.Add(new MenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); }, UrinateMeta)); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.FaceReveal, () => { this.OnFaceRevealSelected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}

		public void ShowFinishMenu()
		{
			this.Options.Clear();
			this.Options.Add(new MenuItem(PropPanelConst.Text.Insert, () => { this.OnInsertSelected?.Invoke(this, 0); })); // 1
			this.Options.Add(new MenuItem(PropPanelConst.Text.Urinate, () => { this.OnUrinateSelected?.Invoke(this, 0); }, UrinateMeta)); // 2
			this.Options.Add(new MenuItem(PropPanelConst.Text.Leave, () => { this.OnLeaveSelected?.Invoke(this, 0); })); // 3
			this.Options.Add(new MenuItem(PropPanelConst.Text.FaceReveal, () => { this.OnFaceRevealSelected?.Invoke(this, 0); })); // 7
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
