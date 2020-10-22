using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shape
{
	[HideInInspector]
	public List<Vector3> points = new List<Vector3>();

	public Color shapeColor;
	public List<GameObject> generate = new List<GameObject>();

	private GameObject quad;

	public void DrawShape()
	{
		if (quad == null)
		{
			quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
			quad.transform.SetParent(GameObject.Find("ShapeCreator").transform);
			GameObject.DestroyImmediate(quad.GetComponent<MeshCollider>());
		}

		MeshRenderer render = quad.GetComponent<MeshRenderer>();
		Material mat = new Material(Shader.Find("Standard"));
		mat.color = shapeColor;
		render.material = mat;

		MeshFilter filter = quad.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
		filter.mesh = mesh;

		Vector3 centerPos = Vector3.zero;
		points.ForEach(p => {
			centerPos += p;
		});
		centerPos /= points.Count;
		// reset points position

		List<Vector3> oriPoints = new List<Vector3>();
		for (int i = 0; i < points.Count; i++)
		{
			oriPoints.Add(points[i] - centerPos);
		}

		Triangulator2 triangulator = new Triangulator2(oriPoints.ToArray());
		mesh.SetVertices(oriPoints);
		mesh.SetTriangles(triangulator.Triangulate(), 0);
		mesh.RecalculateNormals();

		quad.transform.localPosition = centerPos;
	}

	public void RandomGenerate()
	{
		if (quad == null)
			return;

		List<TriangleData> triangles = new List<TriangleData>();

		Mesh mesh = quad.GetComponent<MeshFilter>().mesh;
		List<Vector3> vertices = new List<Vector3>();
		mesh.GetVertices(vertices);
		int[] indices = mesh.triangles;

		for (int i = 0; i < indices.Length; i += 3) 
		{
			TriangleData triangle = new TriangleData();
			triangle.a = vertices[indices[i]];
			triangle.b = vertices[indices[i + 1]];
			triangle.c = vertices[indices[i + 2]];

			triangles.Add(triangle);
		}

		// rotate triangle 
		for (int i = 0; i < triangles.Count; i++) 
		{
			triangles[i].GenRandomPoint(100);	
		}

		// make calculate point 
		// rotate back ?
		// get point 
	}


}


public class TriangleData
{
	public Vector3 a;
	public Vector3 b;
	public Vector3 c;

	public List<Vector3> randomPoint;

	public List<Vector3> GenRandomPoint (int count) 
	{
		Vector3 ta, tb, tc;
		
		ta = a - a;
		tb = b - a;
		tc = c - a;
		
		float degree = getLineRotateAngle(tb);

		

		return null;
	}

	private float getLineRotateAngle(Vector3 tb) 
	{
		Vector2 tb2v = tb.ToXZ();
		return Mathf.Acos(Vector2.Dot(tb2v.normalized, Vector2.up));
	}

}