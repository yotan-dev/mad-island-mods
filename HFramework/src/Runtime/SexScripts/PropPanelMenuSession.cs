#nullable enable

using System;
using HFramework.ScriptNodes;
using UnityEngine;

namespace HFramework.SexScripts
{
	[Experimental]
	public class PropPanelMenuSession : ISexScriptMenuSession
	{
		private readonly CommonContext Context;
		private readonly SexScriptMenuPanel Panel;
		private (string Id, string Text, MenuOption.EffectType Effect)[] Options = Array.Empty<(string Id, string Text, MenuOption.EffectType Effect)>();

		public PropPanelMenuSession(CommonContext context, Vector3 openPos) {
			this.Context = context;
			this.Panel = new SexScriptMenuPanel(() => this.Options, this.SubmitChoice);
			this.Panel.Open(openPos);
		}

		public void SetOptions((string Id, string Text, MenuOption.EffectType Effect)[] options) {
			this.Options = options ?? Array.Empty<(string Id, string Text, MenuOption.EffectType Effect)>();
			this.Panel.Redraw();
		}

		public void Show() {
			this.Panel.Show();
		}

		public void Hide() {
			this.Panel.Hide();
		}

		public void Close() {
			this.Panel.Close();
		}

		private void SubmitChoice(string choiceId, MenuOption.EffectType effect) {
			if (string.IsNullOrEmpty(choiceId))
				return;

			if (effect == MenuOption.EffectType.ChangeState) {
				this.Context.PendingChoiceId = choiceId;
			} else {
				this.Context.PendingChoiceAction = choiceId;
			}
		}
	}
}
