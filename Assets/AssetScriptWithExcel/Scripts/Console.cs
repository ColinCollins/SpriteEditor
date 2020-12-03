using System;
using UnityEngine;

/// <summary>
/// Set DEBUG to Project Setting Scripting Define Symbols could see debug
/// </summary>
public static class Console
{
	public static void Log(string msg) 
	{
#if DEBUG
		Debug.Log(msg);
#endif
	}

	public static void LogError(string msg) 
	{
#if DEBUG
		Debug.LogError(msg);
#endif
	}

	public static void LogWarning(string msg) 
	{
#if DEBUG
		Debug.LogWarning(msg);
#endif
	}
}
