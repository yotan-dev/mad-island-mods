#nullable enable

using System;
using System.Collections.Generic;
using HFramework.Tree;
using UnityEngine;
using YotanModCore.PropPanels;

namespace HFramework.SexScripts
{
	public class SexScriptMenuPanel : BasePropPanel
	{
		private readonly Func<(string Id, string Text, MenuOption.EffectType Effect)[]> getOptions;
		private readonly Action<string, MenuOption.EffectType> onSelected;

		public SexScriptMenuPanel(Func<(string Id, string Text, MenuOption.EffectType Effect)[]> getOptions, Action<string, MenuOption.EffectType> onSelected)
		{
			this.getOptions = getOptions;
			this.onSelected = onSelected;
		}

		public void Redraw()
		{
			this.Options.Clear();
			foreach (var (id, text, effect) in this.getOptions())
			{
				this.Options.Add(new MenuItem(text, () => this.onSelected(id, effect)));
			}
			PropPanelManager.Instance.DrawOptions();
		}
	}
}
