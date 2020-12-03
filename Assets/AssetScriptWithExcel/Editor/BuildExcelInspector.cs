using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class BuildExcelInspector
{
	static BasicTemplate basicAsset;

	[MenuItem("Assets/Custom/BuildExcelFromAssetScript")]
	public static void BuildExcelFromAssetScript()
	{
		if (Selection.activeObject.GetType().BaseType.IsAssignableFrom(typeof(BasicTemplate)))
		{
			basicAsset = Selection.activeObject as BasicTemplate;
		}

		if (basicAsset != null)
			basicAsset.GenerateAutoScript();
		// basicAsset = ;
	}
}
