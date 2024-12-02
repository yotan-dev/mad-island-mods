using System.Collections.Generic;
using UnityEngine;

namespace YotanModCore.PropPanels
{
	/// <summary>
	/// Base class to create Prop Panels
	/// </summary>
	public abstract class BasePropPanel
	{
		public List<ConstMenuItem> Options { get; private set; } = new List<ConstMenuItem>();

		public BasePropPanel()
		{
		}

		/// <summary>
		/// Opens the panel at pos. Should be used when opening it for the first time.
		/// </summary>
		/// <param name="pos"></param>
		public virtual void Open(Vector3 pos)
		{
			PropPanelManager.Instance.Open(this, pos);
		}

		/// <summary>
		/// Makes an already open panel visible. Usually used after using Hide()
		/// </summary>
		public virtual void Show()
		{
			PropPanelManager.Instance.Show(this);
		}

		/// <summary>
		/// Hides a panel, making it invisible but still "existent". To show it again use "Show".
		/// To fully close it, use "Close"
		/// </summary>
		public virtual void Hide()
		{
			PropPanelManager.Instance.Hide(this);
		}

		/// <summary>
		/// Closes a panel, making it invisible and "non-existent".
		/// </summary>
		public virtual void Close()
		{
			PropPanelManager.Instance.Close(this);
		}
	}
}
