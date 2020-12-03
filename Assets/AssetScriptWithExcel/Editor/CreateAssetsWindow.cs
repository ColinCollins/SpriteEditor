using UnityEngine;
using UnityEditor;

public class CreateAssetsWindow : EditorWindow
{
	bool needsRepaint;
	static string path;
	static CreateAssetsWindow handle;
	AssetScriptSetting setting;

	[MenuItem("Assets/Custom/Create AssetScripts")]
	public static void OpenAssetScriptWindowByAssets()
	{
		path = string.Concat(Application.dataPath.Substring(0, Application.dataPath.LastIndexOf('/') + 1), AssetDatabase.GetAssetPath(Selection.activeObject));
		if (path == null || path.Equals(""))
			return;

		handle = EditorWindow.GetWindow<CreateAssetsWindow>();
	}

	private void OnGUI()
	{
		draw();
		if (needsRepaint) 
		{
			needsRepaint = false;
			Repaint();
		}
	}

	void draw() 
	{
		EditorGUI.BeginChangeCheck();

		setting.Template = (TextAsset)EditorGUILayout.ObjectField("Script Template: ", setting.Template, typeof(TextAsset), false);
		EditorGUILayout.Space();
		setting.TypeName = EditorGUILayout.TextField("Script Type: ", setting.TypeName);
		EditorGUILayout.Space();
		setting.ExcelPath = EditorGUILayout.TextField("Excel Save Path: ", setting.ExcelPath);
		
		if (EditorGUI.EndChangeCheck())
		{
			needsRepaint = true;
			setting.SaveLocal();
		}

		#region  button group

		EditorGUILayout.Space();
		GUI.color = new Color(0.5f, 0.8f, 0.8f);
		if (GUILayout.Button("create scriptObject file")) 
		{
			setting.CreateObjectFile(path);
		}

		GUI.color = new Color(0.3f, 0.8f, 0.7f);
		if (GUILayout.Button("create asset file")) 
		{
			setting.CreateAssetFile();
		}

		#endregion

		//GUI.color = new Color(0.1f, 0.75f, 0.6f);
		//if (GUILayout.Button("create excel obj file"))
		//{
		//	setting.CreateObjScript();
		//}

		//GUI.color = new Color(0.95f, 0.90f, 0.6f);
		//if (GUILayout.Button("create excel file from assets"))
		//{
		//	setting.AssetTransform2Excel();
		//}
	}

	private void OnEnable()
	{
		titleContent.text = "Create AssetScript";
		position.Set(position.x, position.y, 400f, 400f);
		minSize = new Vector2(400f, 350f);

		needsRepaint = true;
		setting = new AssetScriptSetting();
		setting.Init();
	}
}