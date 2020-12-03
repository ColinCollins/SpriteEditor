using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


/// <summary>
/// 界面图形编辑脚本，目前仅支持矩形形状
/// </summary>
[CustomEditor(typeof(ShapeCreator))]
public class ShapeEditor : Editor
{
	ShapeCreator shapeCreator;
	SelectionInfo selectionInfo;
	bool shapeChangedSinceLastRepaint;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		int shapeDeleteIndex = -1;
		shapeCreator.ShowShapeList = EditorGUILayout.Foldout(shapeCreator.ShowShapeList, "Show Shapes List");

		if (shapeCreator.ShowShapeList)
		{
			for (int i = 0; i < shapeCreator.shapes.Count; i++)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label("Shape" + (i + 1));
				GUI.enabled = i != selectionInfo.shapeIndex;
				if (GUILayout.Button("Select"))
				{
					selectionInfo.shapeIndex = i;
				}

				GUI.enabled = true;
				if (GUILayout.Button("Delete"))
				{
					shapeDeleteIndex = i;
				}

				if (GUILayout.Button("Draw"))
				{
					shapeCreator.shapes[i].DrawShape();
				}

				if (GUILayout.Button("Generate"))
				{
					Undo.RecordObject(shapeCreator, "Random generate");
					shapeCreator.shapes[i].RandomGenerate();
				}

				GUILayout.EndHorizontal();
			}
		}

		if (shapeDeleteIndex != -1)
		{
			Undo.RecordObject(shapeCreator, "Delete shape");
			shapeCreator.shapes[shapeDeleteIndex].Delete();
			shapeCreator.shapes.RemoveAt(shapeDeleteIndex);
			selectionInfo.shapeIndex = Mathf.Clamp(selectionInfo.shapeIndex, 0, shapeCreator.shapes.Count - 1);
		}

		if (GUI.changed)
		{
			shapeChangedSinceLastRepaint = true;
			SceneView.RepaintAll();
		}
	}

	void OnSceneGUI()
	{
		Event guiEvent = Event.current;

		if (guiEvent.type == EventType.Repaint)
		{
			Draw();
		}
		else if (guiEvent.type == EventType.Layout)
		{
			HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
		}
		else
		{
			HandleInput(guiEvent);
			if (shapeChangedSinceLastRepaint)
			{
				HandleUtility.Repaint();
			}
		}
	}

	void CreateNewShape()
	{
		Undo.RecordObject(shapeCreator, "Create New Shape");
		shapeCreator.shapes.Add(new Shape());
		selectionInfo.shapeIndex = shapeCreator.shapes.Count - 1;

		Debug.Log(selectionInfo.shapeIndex);
	}

	void CreateNewPoint(Vector3 mousePosition)
	{
		int newPointIndex = (selectionInfo.mouseIsOverLine) ? selectionInfo.lineIndex + 1 : SelectedShape.points.Count;
		// record a method
		Undo.RecordObject(shapeCreator, "Add point");
		SelectedShape.points.Insert(newPointIndex, mousePosition);
		selectionInfo.pointIndex = newPointIndex;
		selectionInfo.mouseOverShapeIndex = selectionInfo.shapeIndex;

		shapeChangedSinceLastRepaint = true;
	}

	void CreateDefaultRect(Vector3 mousePosition) 
	{
		float tr = shapeCreator.handleRadius * 3;
		Vector3 rtPos = new Vector3(mousePosition.x + tr, 0, mousePosition.z + tr);
		Vector3 rbPos = new Vector3(mousePosition.x + tr, 0, mousePosition.z);
		Vector3 ltPos = new Vector3(mousePosition.x, 0, mousePosition.z + tr);

		CreateNewPoint(mousePosition);
		CreateNewPoint(ltPos);
		CreateNewPoint(rtPos);
		CreateNewPoint(rbPos);
	}

	void SelectPointUnderMouse()
	{
		selectionInfo.pointIsSelected = true;
		selectionInfo.mouseIsOverPoint = true;
		selectionInfo.mouseIsOverLine = false;
		selectionInfo.lineIndex = -1;

		selectionInfo.positionAtStartOfDrag = SelectedShape.points[selectionInfo.pointIndex];
		shapeChangedSinceLastRepaint = true;
	}

	void SelectedShapeUnderMouse()
	{
		if (selectionInfo.mouseOverShapeIndex != -1 && selectionInfo.mouseOverShapeIndex != selectionInfo.shapeIndex)
			selectionInfo.shapeIndex = selectionInfo.mouseOverShapeIndex;
	}

	void HandleInput(Event guiEvent)
	{
		Ray mouseRay = HandleUtility.GUIPointToWorldRay(guiEvent.mousePosition);

		float drawPlaneHeight = 0;
		float dstToDrawPlane = (drawPlaneHeight - mouseRay.origin.y) / mouseRay.direction.y;
		Vector3 mousePosition = mouseRay.GetPoint(dstToDrawPlane);

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
		{
			HandleLeftMouseDown(mousePosition);
		}

		if (guiEvent.type == EventType.MouseDown && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.Shift)
		{
			HandleShiftLeftMouseDown(mousePosition);
		}

		if (guiEvent.type == EventType.MouseUp && guiEvent.button == 0)
		{
			HandleLeftMouseUp(mousePosition);
		}

		if (guiEvent.type == EventType.MouseDrag && guiEvent.button == 0 && guiEvent.modifiers == EventModifiers.None)
		{
			HandleLeftMouseDrag(mousePosition);
		}

		//if (guiEvent.type == EventType.MouseUp && guiEvent.button == 2 && guiEvent.modifiers == EventModifiers.None)
		//{
		//	 HandleMiddleMouseUp(mousePosition);
		//}

		if (!selectionInfo.pointIsSelected)
			UpdateMouseOverInfo(mousePosition);
	}

	void HandleShiftLeftMouseDown(Vector3 mousePosition)
	{
		CreateNewShape();
		CreateDefaultRect(mousePosition);
	}

	void HandleLeftMouseDown(Vector3 mousePosition)
	{
		if (shapeCreator.shapes.Count == 0)
		{
			CreateNewShape();
			CreateDefaultRect(mousePosition);
		}

		SelectedShapeUnderMouse();

		if (selectionInfo.mouseIsOverPoint)
		{
			SelectPointUnderMouse();
		}
	}

	void HandleLeftMouseUp(Vector3 mousePosition)
	{
		if (selectionInfo.pointIsSelected)
		{
			SelectedShape.points[selectionInfo.pointIndex] = selectionInfo.positionAtStartOfDrag;
			Undo.RecordObject(shapeCreator, "Move point");
			SelectedShape.points[selectionInfo.pointIndex] = mousePosition;

			selectionInfo.pointIsSelected = false;
			selectionInfo.pointIndex = -1;
			shapeChangedSinceLastRepaint = true;
		}
	}

	void HandleLeftMouseDrag(Vector3 mousePosition)
	{
		if (selectionInfo.pointIsSelected)
		{
			SelectedShape.points[selectionInfo.pointIndex] = mousePosition;
			SelectedShape.UpdateRect(selectionInfo.pointIndex);
			shapeChangedSinceLastRepaint = true;
		}
	}

	void HandleMiddleMouseUp(Vector3 mousePosition)
	{
		if (!selectionInfo.mouseIsOverPoint)
			return;

		SelectedShapeUnderMouse();

		shapeCreator.shapes[selectionInfo.shapeIndex].points.RemoveAt(selectionInfo.pointIndex);
		selectionInfo.pointIndex = -1;
		selectionInfo.mouseIsOverPoint = false;
		selectionInfo.pointIsSelected = false;
		shapeChangedSinceLastRepaint = true;
	}

	void UpdateMouseOverInfo(Vector3 mousePosition)
	{
		int mouseOverPointIndex = -1;
		int mouseOverShapeIndex = -1;

		for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
		{
			Shape currentShape = shapeCreator.shapes[shapeIndex];
			for (int i = 0; i < currentShape.points.Count; i++)
			{
				if (Vector3.Distance(mousePosition, currentShape.points[i]) < shapeCreator.handleRadius)
				{
					mouseOverPointIndex = i;
					mouseOverShapeIndex = shapeIndex;
					break;
				}
			}
		}

		if (mouseOverPointIndex != selectionInfo.pointIndex)
		{
			selectionInfo.pointIndex = mouseOverPointIndex;
			selectionInfo.mouseIsOverPoint = mouseOverPointIndex != -1;
			selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
			shapeChangedSinceLastRepaint = true;
		}

		// check for line 
		if (selectionInfo.mouseIsOverPoint)
		{
			selectionInfo.mouseIsOverLine = false;
			selectionInfo.lineIndex = -1;
		}
		else
		{
			//int mouseOverLineIndex = -1;
			//float closestLineDst = shapeCreator.handleRadius;

			//for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
			//{
			//	Shape currentShape = shapeCreator.shapes[shapeIndex];
			//	for (int i = 0; i < currentShape.points.Count; i++)
			//	{
			//		Vector3 nextPointInShape = currentShape.points[(i + 1) % currentShape.points.Count];
			//		float dstFromMouseToLine = HandleUtility.DistancePointToLineSegment(mousePosition.ToXZ(), currentShape.points[i].ToXZ(), nextPointInShape.ToXZ());

			//		if (dstFromMouseToLine < closestLineDst)
			//		{
			//			closestLineDst = dstFromMouseToLine;
			//			mouseOverLineIndex = i;
			//			mouseOverShapeIndex = shapeIndex;
			//		}
			//	}

			//}
			// Mouse Over Line 
			//if (selectionInfo.lineIndex != mouseOverLineIndex)
			//{
			//	selectionInfo.lineIndex = mouseOverLineIndex;
			//	selectionInfo.mouseIsOverLine = mouseOverLineIndex != -1;
			//	selectionInfo.mouseOverShapeIndex = mouseOverShapeIndex;
			//	shapeChangedSinceLastRepaint = true;
			//}
		}
	}

	void Draw()
	{
		for (int shapeIndex = 0; shapeIndex < shapeCreator.shapes.Count; shapeIndex++)
		{
			Shape shapeToDraw = shapeCreator.shapes[shapeIndex];

			bool shapeIsSelected = shapeIndex == selectionInfo.shapeIndex;
			bool mouseIsOverShape = shapeIndex == selectionInfo.mouseOverShapeIndex;
			Color deselectedShapeColor = Color.grey;

			for (int i = 0; i < shapeToDraw.points.Count; i++)
			{
				Vector3 nextPoint = shapeToDraw.points[(i + 1) % shapeToDraw.points.Count];

				// draw line 
				if (selectionInfo.lineIndex == i && mouseIsOverShape)
				{
					Handles.color = Color.red;
					Handles.DrawLine(shapeToDraw.points[i], nextPoint);
				}
				else
				{
					Handles.color = shapeIsSelected ? Color.green : deselectedShapeColor;
					Handles.DrawDottedLine(shapeToDraw.points[i], nextPoint, 4);
				}

				// draw point
				if (selectionInfo.pointIndex == i && mouseIsOverShape)
				{
					Handles.color = selectionInfo.pointIsSelected ? Color.black : Color.red;
				}
				else
				{
					Handles.color = shapeIsSelected ? Color.white : deselectedShapeColor;
				}

				Handles.DrawSolidDisc(shapeToDraw.points[i], Vector3.up, shapeCreator.handleRadius);
			}
		}

		shapeChangedSinceLastRepaint = false;
	}

	void OnEnable()
	{
		shapeChangedSinceLastRepaint = true;
		shapeCreator = target as ShapeCreator;
		selectionInfo = new SelectionInfo();
		Undo.undoRedoPerformed += OnUndoOrRedo;
		Tools.hidden = true;
	}

	void OnDisable() 
	{
		Undo.undoRedoPerformed -= OnUndoOrRedo;
		Tools.hidden = false;
	}

	void OnUndoOrRedo() 
	{
		if (selectionInfo.shapeIndex >= shapeCreator.shapes.Count || selectionInfo.shapeIndex == -1) 
		{
			selectionInfo.shapeIndex = shapeCreator.shapes.Count - 1;	
		}

		shapeChangedSinceLastRepaint = true;
	}

	Shape SelectedShape 
	{
		get 
		{
			// Debug.Log(selectionInfo.shapeIndex);
			return shapeCreator.shapes[selectionInfo.shapeIndex];
		}
	}

	public class SelectionInfo 
	{
		public int shapeIndex;
		public int mouseOverShapeIndex = -1;

		public int pointIndex = -1;
		public bool mouseIsOverPoint;
		public bool pointIsSelected;

		public Vector3 positionAtStartOfDrag;

		public int lineIndex = -1;
		public bool mouseIsOverLine = false;
	}
}
