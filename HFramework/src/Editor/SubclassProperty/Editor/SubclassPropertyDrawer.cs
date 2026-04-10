using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/**
 * https://discussions.unity.com/t/sub-class-property-drawer/883731
 * https://github.com/lordconstant/SubclassPropertyDrawer
 */

[CustomPropertyDrawer(typeof(SubclassAttribute))]
public class SubclassPropertyDrawer : PropertyDrawer
{
	Dictionary<System.Type, SubclassSelector> m_selectorMap = new Dictionary<System.Type, SubclassSelector>();

	const float m_spacing = 2.0f;

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		float lineSize = EditorGUIUtility.singleLineHeight;
		float fieldSpacing = EditorGUIUtility.standardVerticalSpacing;

		EditorGUI.BeginProperty(position, label, property);
		{
			position.x -= m_spacing;
			position.width += m_spacing * 2.0f;
			position.y -= m_spacing;
			position.height += m_spacing * 2.0f;

			EditorGUI.DrawRect(position, new Color(0.28f, 0.28f, 0.28f));
			position.x += m_spacing;
			position.y += m_spacing;

			position.width -= m_spacing * 2.0f;
			position.height -= m_spacing * 2.0f;

			SubclassAttribute propertyAtt = attribute as SubclassAttribute;

			SubclassSelector selector = null;

			System.Type selectType = null;

			if (selectType == null)
			{
				selectType = fieldInfo.FieldType;

				if (selectType.IsArray)
					selectType = selectType.GetElementType();

				if (propertyAtt.IsList)
				{
					System.Type[] genericTypes = selectType.GetGenericArguments();

					if (genericTypes == null || genericTypes.Length <= 0)
						return;

					selectType = genericTypes[0];
				}
			}

			if (!m_selectorMap.TryGetValue(selectType, out selector))
			{
				selector = new SubclassSelector(selectType, property.managedReferenceValue == null ? null : property.managedReferenceValue.GetType(), propertyAtt.IncludeSelf);
				m_selectorMap.Add(selectType, selector);
			}
			else
			{
				selector.RefreshSelection(property.managedReferenceValue == null ? null : property.managedReferenceValue.GetType());
			}

			position.height = lineSize;

			if (selector != null && selector.Draw(position))
			{
				property.managedReferenceValue = selector.CreateSelected();
				property.serializedObject.ApplyModifiedProperties();
			}
			else
			{
				position.y += lineSize;

				EditorGUI.PropertyField(position, property, label);

				bool indented = false;

				if (EditorGUI.indentLevel <= 0)
				{
					EditorGUI.indentLevel++;
					indented = true;
				}

				position.y += lineSize;

				SerializedProperty lastProp = property.GetEndProperty();

				if (property.hasVisibleChildren && property.isExpanded)
				{
					SerializedProperty curProp = property;

					if (curProp.NextVisible(true))
					{
						while (!SerializedProperty.EqualContents(curProp, lastProp))
						{
							position.height = EditorGUI.GetPropertyHeight(curProp);
							EditorGUI.PropertyField(position, curProp);

							position.y += fieldSpacing;

							if (curProp.hasChildren && !curProp.isArray && curProp.isExpanded)
							{
								position.y += lineSize;
							}
							else
							{
								float propHeight = EditorGUI.GetPropertyHeight(curProp);
								position.y += propHeight;
							}

							if (!curProp.NextVisible(!curProp.isArray && curProp.isExpanded))
								break;
						}
					}
				}

				if (indented)
					EditorGUI.indentLevel--;
			}
		}
		EditorGUI.EndProperty();
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		float lineSize = EditorGUIUtility.singleLineHeight;
		float fieldSpacing = EditorGUIUtility.standardVerticalSpacing;

		float totalHeight = lineSize + (m_spacing * 2.0f);

		SerializedProperty lastProp = property.GetEndProperty();

		if (property.hasVisibleChildren && property.isExpanded)
		{
			SerializedProperty curProp = property;

			if (curProp.NextVisible(true))
			{
				while (!SerializedProperty.EqualContents(curProp, lastProp))
				{
					totalHeight += fieldSpacing;

					if (curProp.hasChildren && !curProp.isArray && curProp.isExpanded)
					{
						totalHeight += lineSize;
					}
					else
					{
						float propHeight = EditorGUI.GetPropertyHeight(curProp);
						totalHeight += propHeight;
					}

					if (!curProp.NextVisible(!curProp.isArray && curProp.isExpanded))
						break;
				}
			}
		}

		return base.GetPropertyHeight(property, label) + totalHeight;
	}
}
