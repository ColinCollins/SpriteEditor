using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class AssetsScriptGenerator
{
	public static void CreateObjectFile(this AssetScriptSetting setting, string path)
	{
		if (setting.TypeName.Equals("") || setting.TypeName == null)
		{
			Console.LogError("脚本类型名称不能为空");
			return;
		}

		string dp = Path.Combine(path, $"./{ setting.TypeName }.cs");
		FileInfo info = new FileInfo(dp);

		if (!info.Exists)
		{
			FileStream fs = info.Create();
			fs.Dispose();
			fs.Close();
			Console.Log($"Create { setting.TypeName }.asset success ");
		}
		else
		{
			Console.LogWarning("File already exists");
			return;
		}

		if (setting.Template == null) 
		{
			setting.Template = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Scripts/AssetScriptTemplate.cs", typeof(TextAsset));
		}

		string content = setting.Template.text.Replace("AssetScriptTemplate", setting.TypeName);
		StreamWriter ws = info.AppendText();
		ws.Write(content);
		ws.Dispose();
		ws.Close();

		Console.Log("Assets Script create finished");
	}

	public static void CreateAssetFile(this AssetScriptSetting setting)
	{
		ScriptableObject obj = ScriptableObject.CreateInstance(setting.TypeName);

		if (!obj)
		{
			Console.LogError("Special type not exist");
			return;
		}

		string path = string.Concat(Application.dataPath, "/ScriptObjectSetting/");
		DirectoryInfo dirInfo = new DirectoryInfo(path);
		if (!dirInfo.Exists)
		{
			dirInfo.Create();
		}

		string scriptPath = string.Concat("Assets/ScriptObjectSetting/", $"{ setting.TypeName }.asset");
		AssetDatabase.CreateAsset(obj, scriptPath);
	}

	/// <summary>
	/// 自动生成 Excel 数据对象脚本
	/// </summary>
	public static void CreateObjScript(this AssetScriptSetting setting) 
	{
		Type.GetType(setting.TypeName);
	}

	public static void AssetTransform2Excel(this AssetScriptSetting setting) 
	{
		
	}
}
