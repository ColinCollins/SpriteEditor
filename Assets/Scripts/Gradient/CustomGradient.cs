using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CustomGradient
{
	public enum BlendMode
	{
		Linear,
		Discrete
	}
	public BlendMode blendMode;
	public bool randomizeColor;

	[SerializeField]
	List<ColorKey> keys = new List<ColorKey>();

	public CustomGradient() 
	{
		AddKey(Color.white, 0);
		AddKey(Color.black, 1);
	}

	public Color Evaluate(float time)
	{
		ColorKey keyLeft = keys[0];
		ColorKey keyRight = keys[keys.Count - 1];

		for (int i = 0; i < keys.Count - 1; i++)
		{
			if (keys[i].Time <= time)
			{
				keyLeft = keys[i];
			}

			if (keys[i + 1].Time >= time) 
			{
				keyRight = keys[i + 1];
				break;
			}
		}

		if (blendMode == BlendMode.Linear)
		{
			float blendTime = Mathf.InverseLerp(keyLeft.Time, keyRight.Time, time);
			return Color.Lerp(keyLeft.Colour, keyRight.Colour, blendTime);
		}

		return keyRight.Colour;
	}

	/// <summary>
	/// Time is kind of color clip
	/// </summary>
	/// <param name="color"></param>
	/// <param name="time"></param>
	public int AddKey(Color color, float time)
	{
		ColorKey newKey = new ColorKey(color, time);
		for (int i = 0; i < keys.Count; i++)
		{
			if (newKey.Time < keys[i].Time)
			{
				keys.Insert(i, newKey);
				return i;
			}
		}

		keys.Add(newKey);
		return keys.Count - 1;
	}

	public int NumKeys
	{
		get
		{
			return keys.Count;
		}
	}

	public ColorKey GetKey(int i)
	{
		return keys[i];
	}

	public Texture2D GetTexture(int width)
	{
		Texture2D texture = new Texture2D(width, 1);
		Color[] colors = new Color[width];

		for (int i = 0; i < width; i++)
		{
			colors[i] = Evaluate((float)i / (width - 1));
		}

		texture.SetPixels(colors);
		texture.Apply();
		return texture;
	}

	public int UpdateKeyTime(int index, float time)
	{
		Color color = keys[index].Colour;
		RemoveKey(index);
		return AddKey(color, time);
	}

	public void UpdateKeyColour(int index, Color color)
	{
		keys[index] = new ColorKey(color, keys[index].Time);
	}

	public void RemoveKey(int index) 
	{
		if (keys.Count >= 2) 
		{
			keys.RemoveAt(index);
		}
	}

	[System.Serializable]
	public struct ColorKey 
	{
		/// <summary>
		/// SerializeField 强制序列化一个私有变量
		/// </summary>
		[SerializeField]
		Color color;
		[SerializeField]
		float time;

		public ColorKey(Color _color, float _time) 
		{
			color = _color;
			time = _time;
		}

		public Color Colour 
		{
			get 
			{
				return color;
			}
		}

		public float Time 
		{
			get 
			{
				return time;
			}
		}
	}
}
