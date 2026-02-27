using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

/**
 * https://discussions.unity.com/t/sub-class-property-drawer/883731
 * https://github.com/lordconstant/SubclassPropertyDrawer
 */

public class SubclassSelector
{
	List<System.Type> m_subClasses;
	string[] m_subClassNames;

	int m_selectedType;

	public SubclassSelector(System.Type InType, System.Type InDefault = null, bool InIncludeSelf = false)
	{
		m_selectedType = -1;

		m_subClasses = new List<System.Type>();

		System.Type[] foundClasses = System.AppDomain.CurrentDomain.GetAssemblies()
		 .SelectMany(assembly => assembly.GetTypes())
		 .Where(type => type.IsSubclassOf(InType)).ToArray();

		if (InIncludeSelf)
		{
			m_subClasses.Add(InType);

			if (InDefault.GetType() == InType)
				m_selectedType = 0;
		}

		for (int i = 0; i < foundClasses.Length; i++)
		{
			if (foundClasses[i].ContainsGenericParameters)
				continue;

			if (InDefault != null)
			{
				if (InDefault == foundClasses[i])
					m_selectedType = m_subClasses.Count;
			}

			m_subClasses.Add(foundClasses[i]);
		}

		m_subClassNames = new string[m_subClasses.Count];

		for (int i = 0; i < m_subClassNames.Length; i++)
		{
			m_subClassNames[i] = m_subClasses[i].Name;
		}
	}

	public void RefreshSelection(System.Type InDefault)
	{
		m_selectedType = -1;

		for (int i = 0; i < m_subClasses.Count; i++)
		{
			if (InDefault == null)
				break;

			if (InDefault != m_subClasses[i])
				continue;

			m_selectedType = i;
			break;
		}
	}

	public bool Draw()
	{
		int selectedIdx = m_selectedType < 0 ? 0 : m_selectedType;

		if (selectedIdx >= 0)
		{
			selectedIdx = EditorGUILayout.Popup(selectedIdx, m_subClassNames);
		}

		if (selectedIdx != m_selectedType)
		{
			m_selectedType = selectedIdx;

			return true;
		}

		return false;
	}

	public bool Draw(Rect InPosition)
	{
		int selectedIdx = m_selectedType < 0 ? 0 : m_selectedType;

		if (selectedIdx >= 0)
		{
			selectedIdx = EditorGUI.Popup(InPosition, selectedIdx, m_subClassNames);
		}

		if (selectedIdx != m_selectedType)
		{
			m_selectedType = selectedIdx;

			return true;
		}

		return false;
	}

	public object CreateSelected()
	{
		return System.Activator.CreateInstance(m_subClasses[m_selectedType]);
	}

	public System.Type GetClassType()
	{
		return m_subClasses[m_selectedType];
	}
}

public class SubclassSelector<T> : SubclassSelector where T : class
{
	public SubclassSelector(System.Type InDefault) : base(typeof(T), InDefault)
	{

	}

	public T CreateSelectedFromType()
	{
		return CreateSelected() as T;
	}
}
