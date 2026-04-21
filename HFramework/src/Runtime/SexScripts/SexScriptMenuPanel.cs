#nullable enable

using System;
using HFramework.ScriptNodes;
using YotanModCore.PropPanels;

namespace HFramework.SexScripts
{
	[Experimental]
	public class SexScriptMenuPanel : BasePropPanel
	{
		private readonly Func<(string Id, string Text, MenuOption.EffectType Effect)[]> GetOptions;
		private readonly Action<string, MenuOption.EffectType> OnSelected;

		public SexScriptMenuPanel(Func<(string Id, string Text, MenuOption.EffectType Effect)[]> getOptions, Action<string, MenuOption.EffectType> onSelected) {
			this.GetOptions = getOptions;
			this.OnSelected = onSelected;
		}

		public void Redraw() {
			this.Options.Clear();
			foreach (var (id, text, effect) in this.GetOptions()) {
				this.Options.Add(new MenuItem(text, () => this.OnSelected(id, effect)));
			}
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
