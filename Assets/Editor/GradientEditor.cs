using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

/// <summary>
/// 颜色渐变的窗口
/// </summary>
public class GradientEditor : EditorWindow
{
	CustomGradient gradient;

	const int borderSize = 10;
	const float keyWidth = 10;
	const float keyHeight = 20;

	Rect gradientPreviewRect;
	Rect[] keyRects;
	bool mouseDownOverKey;
	bool needsRepaint;
	int selectedKeyIndex;

	void draw() 
	{
		gradientPreviewRect = new Rect(borderSize, borderSize, position.width - borderSize * 2, 25);

		GUI.DrawTexture(gradientPreviewRect, gradient.GetTexture((int)gradientPreviewRect.width));
		keyRects = new Rect[gradient.NumKeys];

		for (int i = 0; i < gradient.NumKeys; i++)
		{
			CustomGradient.ColorKey key = gradient.GetKey(i);
			Rect keyRect = new Rect(gradientPreviewRect.x + gradientPreviewRect.width * key.Time - keyWidth / 2f,
				gradientPreviewRect.yMax + borderSize, keyWidth, keyHeight
				);

			if (i == selectedKeyIndex)
			{
				EditorGUI.DrawRect(new Rect(keyRect.x - 2, keyRect.y - 2, keyRect.width + 4, keyRect.height + 4), Color.black);
			}

			EditorGUI.DrawRect(keyRect, key.Colour);
			keyRects[i] = keyRect;
		}

		Rect settingRect = new Rect(borderSize, keyRects[0].yMax + borderSize, position.width - borderSize * 2, position.height);

		GUILayout.BeginArea(settingRect);
		// EditorGUI.DrawRect(settingRect, Color.white);
		EditorGUI.BeginChangeCheck();
		Color newColor = EditorGUILayout.ColorField(gradient.GetKey(selectedKeyIndex).Colour);
		if (EditorGUI.EndChangeCheck()) 
		{
			gradient.UpdateKeyColour(selectedKeyIndex, newColor);
		}

		gradient.blendMode = (CustomGradient.BlendMode)EditorGUILayout.EnumPopup("Blend mode", gradient.blendMode);
		gradient.randomizeColor = EditorGUILayout.Toggle("Randomize color", gradient.randomizeColor);
		GUILayout.EndArea();
	}

	private void handleMouseListen() 
	{
		Event guiEvent = Event.current;

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0)
		{
			for (int i = 0; i < keyRects.Length; i++)
			{
				if (keyRects[i].Contains(guiEvent.mousePosition))
				{
					mouseDownOverKey = true;
					selectedKeyIndex = i;
					needsRepaint = true;
					break; 
				}
			}

			if (!mouseDownOverKey)
			{
				float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
				Color interpolatedColor = gradient.Evaluate(keyTime);
				Color randomColor = new Color(Random.value, Random.value, Random.value);
				selectedKeyIndex = gradient.AddKey(gradient.randomizeColor ? randomColor : interpolatedColor, keyTime);
				mouseDownOverKey = true;
				needsRepaint = true;
			}
		}

		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
		{
			OnLeftMouseUp();
		}

		if (guiEvent.type == EventType.MouseDrag && mouseDownOverKey && guiEvent.button == 0)
		{
			float keyTime = Mathf.InverseLerp(gradientPreviewRect.x, gradientPreviewRect.xMax, guiEvent.mousePosition.x);
			selectedKeyIndex = gradient. UpdateKeyTime(selectedKeyIndex, keyTime);
			needsRepaint = true;
		}

		if ((guiEvent.keyCode == KeyCode.D || guiEvent.keyCode == KeyCode.Delete) && guiEvent.type == EventType.KeyDown)
		{
			OnSpaceKeyDown();
		}
	}

	private void OnLeftMouseUp() 
	{
		mouseDownOverKey = false;
	}

	private void OnSpaceKeyDown() 
	{
		gradient.RemoveKey(selectedKeyIndex);
		if (selectedKeyIndex >= gradient.NumKeys)
		{
			selectedKeyIndex--;
		}

		needsRepaint = true;
	}

	public void SetGradient(CustomGradient gradient) 
	{
		this.gradient = gradient;
	}

	private void OnGUI()
	{
		draw();
		handleMouseListen();

		if (needsRepaint)
		{
			needsRepaint = false;
			Repaint();
		}
	}

	private void OnEnable()
	{
		titleContent.text = "Gradient Editor";
		position.Set(position.x, position.y, 400, 150f);
		minSize = new Vector2(200, 150);
		maxSize = new Vector2(1920, 150);
	}

	private void OnDisable()
	{
		UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
	}

}
