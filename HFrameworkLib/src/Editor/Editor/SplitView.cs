using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace HFramework.Tree.EditorUI
{
	public class SplitView : TwoPaneSplitView
	{
		public new class UxmlFactory : UxmlFactory<SplitView, TwoPaneSplitView.UxmlTraits> { }
	}
}
