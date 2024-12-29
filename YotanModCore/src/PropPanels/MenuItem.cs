using System;

namespace YotanModCore.PropPanels
{
	/// <summary>
	/// Defines a MenuItem for PropPanels.
	/// </summary>
	public class MenuItem
	{
		/// <summary>
		/// Text to be shown.
		/// </summary>
		public string Text;

		/// <summary>
		/// Action ran when this menu item is clicked
		/// </summary>
		public Action Action;

		/// <summary>
		/// Creates a menu item with our own text
		/// </summary>
		/// <param name="text">Text</param>
		/// <param name="action">Action ran when this menu item is clicked</param>
		public MenuItem(string text, Action action)
		{
			Text = text;
			Action = action;
		}

		/// <summary>
		/// Creates a menu item using a text from actButtonTexts from Text Manager (original messages)
		/// </summary>
		/// <param name="textId">See PropPanelConst.Text</param>
		/// <param name="action">Action ran when this menu item is clicked</param>
		public MenuItem(int textId, Action action)
		{
			Text = Managers.mn.textMN.actButtonTexts[textId];
			Action = action;
		}

	}
}
