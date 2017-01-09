using UnityEngine;
using System.Collections;

public class MakeGroundMesh : MonoBehaviour
{
	GPS gps;
	int gridHeight;
	int gridWidth;

	Vector3[] getVertices(HeightsGrid heightsGrid)
	{
		Vector3 topLeft = heightsGrid.topLeft;
		float[,] heights = heightsGrid.levels;
		float zSpacing = heightsGrid.northSpacing;
		float xSpacing = heightsGrid.eastSpacing;

		Vector3[] v = new Vector3[gridWidth*gridHeight];

		for (int z = 0; z < gridHeight; z += 1)
		{
			for (int x = 0; x < gridWidth; x += 1)
			{
				int i = z*gridWidth + x;

				v[i] = new Vector3(
					((float)x * xSpacing),
					heights[z, x],
				    ((float)z * zSpacing));
				v[i] += topLeft;
			}
		}

		return v;
	}

	int[] getTriangles(HeightsGrid heightsGrid)
	{
		float[,] heights = heightsGrid.levels;

		int h = heights.GetLength(0);
		int w = heights.GetLength(1);

		int[] t = new int[(h-1)*(w-1)*2*3];

		int cntr = 0;
		for (int i = 0; i < w * (h-1); i += 1)
		{
			if (i % w == 0)
			{
				continue;
			}
			t[cntr] = i;
			t[cntr+1] = i + w - 1;
			t[cntr+2] = i + w;

			t[cntr+3] = i;
			t[cntr+4] = i - 1;
			t[cntr+5] = i + w - 1;

			cntr += 6;
		}

		return t;
	}

	void dumpMesh(Mesh mesh)
	{
		Vector3[] v = mesh.vertices;
		int[] t = mesh.triangles;

		for (int i = 0; i < t.Length; i += 3)
		{
			Debug.LogFormat("i {0} [{1} {2} {3}] {4} {5} {6}",
				i,
				t[i], t[i+1], t[i+2],
				v[t[i]], v[t[i+1]], v[t[i+2]]);
		}
	}

	void Start ()
	{
		gps = Camera.main.GetComponent<GPS>();
		HeightsGrid heightsGrid = gps.getHeightsGrid();
        gridHeight = heightsGrid.levels.GetLength(0);
		gridWidth = heightsGrid.levels.GetLength(1);

		Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;


		mesh.vertices = getVertices(heightsGrid);
		mesh.triangles = getTriangles(heightsGrid);

		GetComponent<Renderer>().material.shader = Shader.Find("invisible");

		//dumpMesh(mesh);
	}

	void Update()
	{
		var updates = gps.getHeightsUpdates();
		if (updates == null)
		{
			return;
		}

		Mesh mesh = GetComponent<MeshFilter>().mesh;
		Vector3[] vertices = mesh.vertices;

		foreach (var heightUpdate in updates)
		{
			vertices[heightUpdate.z*gridWidth + heightUpdate.x].y = heightUpdate.level;
		}
		mesh.vertices = vertices;
	}
}
