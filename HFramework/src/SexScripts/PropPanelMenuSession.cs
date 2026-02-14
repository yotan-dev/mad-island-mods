#nullable enable

using System;
using HFramework.Tree;
using UnityEngine;

namespace HFramework.SexScripts
{
	public class PropPanelMenuSession : ISexScriptMenuSession
	{
		private readonly CommonContext context;
		private readonly SexScriptMenuPanel panel;
		private (string Id, string Text)[] options = Array.Empty<(string Id, string Text)>();

		public PropPanelMenuSession(CommonContext context, Vector3 openPos)
		{
			this.context = context;
			this.panel = new SexScriptMenuPanel(() => this.options, this.SubmitChoice);
			this.panel.Open(openPos);
		}

		public void SetOptions((string Id, string Text)[] options)
		{
			this.options = options ?? Array.Empty<(string Id, string Text)>();
			this.panel.Redraw();
		}

		public void Show()
		{
			this.panel.Show();
		}

		public void Hide()
		{
			this.panel.Hide();
		}

		public void Close()
		{
			this.panel.Close();
		}

		private void SubmitChoice(string choiceId)
		{
			if (string.IsNullOrEmpty(choiceId))
				return;

			this.context.PendingChoiceId = choiceId;
		}
	}
}
