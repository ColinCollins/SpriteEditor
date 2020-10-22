using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeCreator : MonoBehaviour
{
	public List<Shape> shapes = new List<Shape>();
	[HideInInspector]
	public bool ShowShapeList;

	public float handleRadius = .5f;
}
