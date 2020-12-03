using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AssetScriptSetting
{
	public string TypeName;
	public string ExcelPath;
	public TextAsset Template;

	const string CLASS_NAME = "CLASS_NAME";
	const string EXCEL_SAVE_PATH = "EXCEL_SAVE_PATH";
	const string TEMPLATE_DEFINE = "TEMPLATE_DEFINE";

	public void Init()
	{
		TypeName = PlayerPrefs.GetString(CLASS_NAME);
		ExcelPath = PlayerPrefs.GetString(EXCEL_SAVE_PATH);
		Template = (TextAsset)AssetDatabase.LoadAssetAtPath(AssetDatabase.GetAssetPath(PlayerPrefs.GetInt(TEMPLATE_DEFINE)), typeof(TextAsset));
	}

	public void SaveLocal()
	{
		PlayerPrefs.SetString(CLASS_NAME, TypeName);
		PlayerPrefs.SetString(EXCEL_SAVE_PATH, ExcelPath);
		PlayerPrefs.SetInt(TEMPLATE_DEFINE, Template.GetInstanceID());
	}
}
