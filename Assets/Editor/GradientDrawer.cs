﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// 渐变颜色绘制 Property
/// </summary>
[CustomPropertyDrawer(typeof(CustomGradient))]
public class GradientDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return base.GetPropertyHeight(property, label);
	}

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		Event guiEvent = Event.current;

		CustomGradient gradient = (CustomGradient)fieldInfo.GetValue(property.serializedObject.targetObject);
		// base.OnGUI(position, property, label);
		// GUI.DrawTexture(position, gradient.GetTexture((int)position.width));
		float labelWidth = GUI.skin.label.CalcSize(label).x + 5f;
		Rect textureRect = new Rect(position.x + labelWidth, position.y, position.width - labelWidth, position.height);

		if (guiEvent.type == EventType.Repaint)
		{
			GUI.Label(position, label.text);
			GUIStyle gradientStyle = new GUIStyle();
			gradientStyle.normal.background = gradient.GetTexture((int)position.width);
			GUI.Label(textureRect, GUIContent.none, gradientStyle);
		}
		else
		{
			if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0) 
			{
				if (textureRect.Contains(guiEvent.mousePosition)) 
				{
					GradientEditor window = EditorWindow.GetWindow<GradientEditor>();
					window.SetGradient(gradient);
				}
			}
		}
		
	}
}
