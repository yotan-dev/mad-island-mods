using System;

namespace YotanModCore.PropPanels
{
	/// <summary>
	/// Defines a Prop Panel menu item that uses the text from the constants list.
	/// See PropPanelConst.Text
	/// </summary>
	public class ConstMenuItem
	{
		/// <summary>
		/// Text ID to be shown. See PropPanelConst.Text
		/// </summary>
		public int TextId;

		/// <summary>
		/// Action ran when this menu item is clicked
		/// </summary>
		public Action Action;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="textId">See PropPanelConst.Text</param>
		/// <param name="action">Action ran when this menu item is clicked</param>
		public ConstMenuItem(int textId, Action action)
		{
			TextId = textId;
			Action = action;
		}
	}
}
