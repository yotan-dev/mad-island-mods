using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * https://discussions.unity.com/t/sub-class-property-drawer/883731
 * https://github.com/lordconstant/SubclassPropertyDrawer
 */
public class SubclassAttribute : PropertyAttribute
{
	public bool IncludeSelf = false;
	public bool IsList = false;

	public SubclassAttribute(bool InIncludeSelf = false, bool InIsList = false)
	{
		IncludeSelf = InIncludeSelf;
		IsList = InIsList;
	}
}
