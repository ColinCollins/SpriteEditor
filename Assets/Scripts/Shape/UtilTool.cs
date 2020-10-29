using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UtilTool
{
	public static Vector2 ToXZ(this Vector3 v3) 
	{
		return new Vector2(v3.x, v3.z);
	}

	public static Vector2 Rotate(this Vector3 v3, float degree) 
	{
		float nx = v3.x * Mathf.Cos(degree) - v3.y * Mathf.Sin(degree);
		float ny = v3.y * Mathf.Sin(degree) + v3.y * Mathf.Cos(degree);

		return new Vector2(nx, ny);
	}
}
