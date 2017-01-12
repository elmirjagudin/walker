using UnityEngine;
using System;
using System.Collections;
using System.Diagnostics;

public class MakeGroundMesh : MonoBehaviour
{
	const int MESH_SIZE = 32;
	Mesh[,] meshes;

	GPS gps;
	HeightsGrid heightsGrid;

	int getVertIndex(int x, int z)
	{
		return z*(MESH_SIZE+1) + x;
	}

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
				int i = getVertIndex(x, z);

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
			UnityEngine.Debug.LogFormat("i {0} [{1} {2} {3}] {4} {5} {6}",
				i,
				t[i], t[i+1], t[i+2],
				v[t[i]], v[t[i+1]], v[t[i+2]]);
		}
	}

	GameObject MakeMesh(HeightsGrid heightsGrid, Vector3 topLeft, int x, int z, int[] triangles)
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

		return gobj;
	}

	void MakeMeshes(HeightsGrid heightsGrid)
	{
		Vector3 topLeftOffset = new Vector3(0f, 0f, 0f);
		float zSpacing = heightsGrid.northSpacing;
		float xSpacing = heightsGrid.eastSpacing;

        int gridHeight = heightsGrid.levels.GetLength(1);
		int gridWidth = heightsGrid.levels.GetLength(0);

		int meshesW = gridWidth/MESH_SIZE;
		int meshesH = gridHeight/MESH_SIZE;

		meshes = new Mesh[meshesW, meshesH];

		int[] triangles = getTriangles();

		for (int z = 0; z < meshesH; z += 1)
		{
			for (int x = 0; x < meshesW; x += 1)
			{
				int startX = x * MESH_SIZE;
				int startZ = z * MESH_SIZE;

				topLeftOffset.x = startX * xSpacing;
				topLeftOffset.z = startZ * zSpacing;

				Vector3 topLeft = heightsGrid.topLeft + topLeftOffset;

				var meshGameObj = MakeMesh(heightsGrid, topLeft, startX, startZ, triangles);
				meshGameObj.transform.parent = gameObject.transform;

				meshes[x, z] = meshGameObj.GetComponent<MeshFilter>().mesh;
			}
		}
	}

	void Start()
	{
		gps = Camera.main.GetComponent<GPS>();
		heightsGrid = gps.getHeightsGrid();

		MakeMeshes(heightsGrid);
	}

	void UpdateMeshVertY(int meshX, int meshZ, int vertX, int vertZ, float newY)
	{
		Mesh mesh = meshes[meshX, meshZ];
		Vector3[] vertices = mesh.vertices;

		vertices[getVertIndex(vertX, vertZ)].y = newY;
		mesh.vertices = vertices;
	}

	void UpdateHeight(HeightUpdate heightUpdate, float heightsOffset)
	{
		float newY = heightUpdate.level + heightsOffset;

		int x = heightUpdate.x / MESH_SIZE;
		int z = heightUpdate.z / MESH_SIZE;


		int vertX = heightUpdate.x % MESH_SIZE;
		int vertZ = heightUpdate.z % MESH_SIZE;

		if (x < meshes.GetLength(0) && z < meshes.GetLength(1))
		{
		    UpdateMeshVertY(x, z, vertX, vertZ, newY);
		}

		/* take care of shared vertices */
		if (vertX == 0 && x > 0 && z < meshes.GetLength(1))
		{
			UpdateMeshVertY(x - 1, z, MESH_SIZE, vertZ, newY);
		}

		if (vertZ == 0 && z > 0 && x < meshes.GetLength(0))
		{
			UpdateMeshVertY(x, z - 1, vertX, MESH_SIZE, newY);
		}

		if (vertX == 0 && vertZ == 0 && x > 0 && z > 0)
		{
			UpdateMeshVertY(x - 1, z - 1, MESH_SIZE, MESH_SIZE, newY);
		}
	}

	void Update()
	{
		var updates = gps.getHeightsUpdates();
		float heightsOffset = heightsGrid.topLeft.y;

		if (updates == null)
		{
			return;
		}

		foreach (var update in updates)
		{
			UpdateHeight(update, heightsOffset);
		}
	}
}
