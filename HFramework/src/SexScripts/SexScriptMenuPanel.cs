#nullable enable

using System;
using System.Collections.Generic;
using UnityEngine;
using YotanModCore.PropPanels;

namespace HFramework.SexScripts
{
	public class SexScriptMenuPanel : BasePropPanel
	{
		private readonly Func<(string Id, string Text)[]> getOptions;
		private readonly Action<string> onSelected;

		public SexScriptMenuPanel(Func<(string Id, string Text)[]> getOptions, Action<string> onSelected)
		{
			this.getOptions = getOptions;
			this.onSelected = onSelected;
		}

		public void Redraw()
		{
			this.Options.Clear();
			foreach (var opt in this.getOptions())
			{
				var id = opt.Id;
				var text = opt.Text;
				this.Options.Add(new MenuItem(text, () => this.onSelected(id)));
			}
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
