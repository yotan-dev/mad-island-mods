using System;

namespace YotanModCore.PropPanels
{
	/// <summary>
	/// Defines a MenuItem for PropPanels.
	/// </summary>
	[Obsolete("Use MenuItem instead. It supports the same features and more. 'Meta' may be used to replace 'TextId'")]
	public class ConstMenuItem : MenuItem
	{
		public int TextId;

		/// <summary>
		/// Creates a menu item using a text from actButtonTexts from Text Manager (original messages)
		/// </summary>
		/// <param name="textId">See PropPanelConst.Text</param>
		/// <param name="action">Action ran when this menu item is clicked</param>
		public ConstMenuItem(int textId, Action action) : base(Managers.mn.textMN.actButtonTexts[textId], action)
		{
			this.TextId = textId;
		}

	}
}
