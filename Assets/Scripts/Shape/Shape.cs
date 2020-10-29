using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Shape
{
	/// <summary>
	/// 0 -> oriPosition, 1 -> ltPos, 2 -> rtPos, 3 -> rbPos
	/// </summary>
	[HideInInspector]
	public List<Vector3> points = new List<Vector3>();
	[HideInInspector]
	public Vector2 centerPos;

	public float seed = 0;
	public float minLimit = 0.3f;
	public int maxCount = 10;

	public Color shapeColor;
	public List<GameObject> stuffs = new List<GameObject>();

	private GameObject quad;
	private List<GameObject> generator = new List<GameObject>();

	public void UpdateRect(int pointIndex) 
	{
		int mi1 = -1;
		int mi2 = -1;

		switch (pointIndex) 
		{
			case 0:
				mi1 = 1;
				mi2 = 3;
				break;
			case 1:
				mi1 = 0;
				mi2 = 2;
				break;
			case 2:
				mi1 = 3;
				mi2 = 1;
				break;
			case 3:
				mi1 = 2;
				mi2 = 0;
				break;
			default:
				return;
		}

		Vector3 p = points[mi1];
		p.x = points[pointIndex].x;
		points[mi1] = p;

		p = points[mi2];
		p.z = points[pointIndex].z;
		points[mi2] = p;
	}

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
		for (int i = 0; i < generator.Count; i++) 
		{
			GameObject.DestroyImmediate(generator[i]);
		}
		generator.Clear();

		// generate at rect area 
		Vector3 oriPos = points[0];
		Vector3 rtPos = points[2];

		float width = rtPos.x - oriPos.x;
		float height = rtPos.z - oriPos.z;

		for (int x = 0; x < maxCount; x++) 
		{
			for (int y = 0; y < maxCount; y++) 
			{
				float dx = width * x / maxCount;
				float dy = height * y / maxCount;
				float re = Mathf.PerlinNoise(dx + seed, dy + seed);
				
				if (re > minLimit)
				{
					GenerateStuff(dx - width / 2, dy - height / 2);
				} 
			}
		}
	}

	private void GenerateStuff(float x, float y) 
	{
		GameObject tmpObj;
		if (stuffs.Count > 0)
		{
			tmpObj = GameObject.Instantiate(stuffs[Mathf.FloorToInt(Mathf.Clamp(Random.Range(0, 1), 0, stuffs.Count))]);
		}
		else 
		{
			tmpObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
		}

		tmpObj.transform.SetParent(quad.transform);
		tmpObj.transform.localPosition = new Vector3(x, 0, y);
		tmpObj.transform.localScale = Vector3.one;

		generator.Add(tmpObj);
	}

	public void Delete() 
	{
		if (quad != null)
			GameObject.DestroyImmediate(quad);

		generator.Clear();
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

		// rotate
		
		return null;
	}

	private float getLineRotateAngle(Vector3 tb) 
	{
		Vector2 tb2v = tb.ToXZ();
		return Mathf.Acos(Vector2.Dot(tb2v.normalized, Vector2.up));
	}
}