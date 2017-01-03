using UnityEngine;
using System.Collections;

public class MakeGroundMesh : MonoBehaviour
{
	float[,] heights = new float[,] {
		{ 0, 0, 0, 0, 0, },
		{ 0, 0, 1, 1.2f, 0, },
		{ 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, 0, },
		{ 0, 0, 0, 0, -.4f, },
	};

	Vector3[] getVertices()
	{
		int h = heights.GetLength(0);
		int w = heights.GetLength(1);

		Vector3[] v = new Vector3[w*h];

		for (int z = 0; z < h; z += 1)
		{
			for (int x = 0; x < w; x += 1)
			{
				v[z*w + x] = new Vector3(
					(float)x,
					heights[z, x],
					(float)z);
			}
		}

		return v;
	}

	int[] getTriangles()
	{

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
		Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
		mesh.vertices = getVertices();
		mesh.triangles = getTriangles();

		//dumpMesh(mesh);
	}

}
