using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#if UNITY_EDITOR
using OfficeOpenXml;
#endif

[Serializable]
public class BasicTemplate : ScriptableObject
{
	public List<Title> titles = new List<Title>();

	public virtual void GenerateAutoScript()
	{
		AssetScriptSetting ass = new AssetScriptSetting();
		ass.Init();
		DirectoryInfo dirInfo = new DirectoryInfo(ass.ExcelPath);

		if (!dirInfo.Exists)
			dirInfo.Create();

		FileInfo fileInfo = new FileInfo(Path.Combine(ass.ExcelPath, $"{name}.xlsx"));

#if UNITY_EDITOR
		using (ExcelPackage package = new ExcelPackage(fileInfo))
		{
			// 清除旧数据，在excel空文件添加新sheet
			if (package.Workbook.Worksheets[name] != null)
				package.Workbook.Worksheets.Delete(name);
			package.Workbook.Worksheets.Add(name);

			ExcelWorksheet worksheet = package.Workbook.Worksheets[name];
			for (int i = 0; i < titles.Count; i++)
			{
				Title title = titles[i];
				// type
				worksheet.Cells[1, i + 1].Value = title.type;
				// property Name
				worksheet.Cells[2, i + 1].Value = title.name;
				// description
				worksheet.Cells[3, i + 1].Value = title.description;

				for (int j = 0; j < title.values.Count; j++)
				{
					string value = title.values[j];
					worksheet.Cells[4 + j, i + 1].Value = value;
				}
			}

			//保存excel
			package.Save();
			Console.Log("Excel file generate finished");
		}
#endif
	}
}

