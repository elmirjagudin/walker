using UnityEngine;
using System;
using System.Collections;

public class MakeGroundMesh : MonoBehaviour
{
	const int MESH_SIZE = 3;

	GPS gps;
	int gridHeight;
	int gridWidth;

	Vector3[] getVertices(HeightsGrid heightsGrid, Vector3 topLeft, int startX, int startZ)
	{
		float[,] heights = heightsGrid.levels;
		float zSpacing = heightsGrid.northSpacing;
		float xSpacing = heightsGrid.eastSpacing;


		Vector3[] v = new Vector3[(MESH_SIZE + 1) * (MESH_SIZE + 1)];

		for (int z = 0; z <= MESH_SIZE; z += 1)
		{
			for (int x = 0; x <= MESH_SIZE; x += 1)
			{
				int i = z*(MESH_SIZE+1) + x;

				v[i] = new Vector3(
					((float)x * xSpacing),
					heights[startX + x, startZ + z],
				    ((float)z * zSpacing));
				v[i] += topLeft;
			}
		}

		return v;
	}

	int[] getTriangles()
	{
		int[] t = new int[MESH_SIZE*MESH_SIZE*2*3];
		int w = MESH_SIZE + 1;
		int h = MESH_SIZE + 1;

		int cntr = 0;
		for (int i = 0; i < (w-1) * h ; i += 1)
		{
			if (i % w  == 0)
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

	void _dumpMesh(Mesh mesh)
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

	void MakeMesh(HeightsGrid heightsGrid, Vector3 topLeft, int x, int z, int[] triangles)
	{
		var gobj = new GameObject(String.Format("Ground Mesh {0}x{1}", x, z));
		var meshFilter = gobj.AddComponent<MeshFilter>();
		var renderer = gobj.AddComponent<MeshRenderer>();
		renderer.material.shader = Shader.Find("invisible");

		Mesh mesh = new Mesh();
		mesh.vertices = getVertices(heightsGrid, topLeft, x, z);
		mesh.triangles = triangles;

		meshFilter.mesh = mesh;

		//_dumpMesh(mesh);
	}

	void MakeMeshes(HeightsGrid heightsGrid)
	{
		Vector3 topLeftOffset = new Vector3(0f, 0f, 0f);
		float zSpacing = heightsGrid.northSpacing;
		float xSpacing = heightsGrid.eastSpacing;

		int[] triangles = getTriangles();

		for (int z = 0; z < (gridHeight-MESH_SIZE); z += MESH_SIZE)
		{
			for (int x = 0; x < (gridWidth-MESH_SIZE); x += MESH_SIZE)
			{
				topLeftOffset.x = x * xSpacing;
				topLeftOffset.z = z * zSpacing;

				Vector3 topLeft = heightsGrid.topLeft + topLeftOffset;

				MakeMesh(heightsGrid, topLeft, x, z, triangles);
			}
		}

	}

	void Start ()
	{
		gps = Camera.main.GetComponent<GPS>();
		HeightsGrid heightsGrid = gps.getHeightsGrid();
        gridHeight = heightsGrid.levels.GetLength(0);
		gridWidth = heightsGrid.levels.GetLength(1);

		MakeMeshes(heightsGrid);
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
