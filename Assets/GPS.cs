using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class HeightsGrid
{
	public Vector3 topLeft;
	public float northSpacing;
	public float eastSpacing;
	public float[,] levels;
}

public class HeightUpdate
{
	public int x,z;
	public float level;

	public HeightUpdate(int x, int z, float level)
	{
		this.x = x;
		this.z = z;
		this.level = level;
	}
}

public class GPS : MonoBehaviour
{
	HeightsGrid heightsGrid = null;
	List<HeightUpdate> heightsUpdates = null;

	public HeightsGrid getHeightsGrid()
	{
		if (heightsGrid == null)
		{
			heightsGrid = new HeightsGrid();

			heightsGrid.topLeft = new Vector3(-32*2f, -10f, -32*1.5f);
			heightsGrid.northSpacing = 1f;
			heightsGrid.eastSpacing = 1f;

			heightsGrid.levels = new float[32*4+1, 32*3+1];
		}
		return heightsGrid;
	}

	public List<HeightUpdate> getHeightsUpdates()
	{
		var res = heightsUpdates;
		heightsUpdates = null;

		return res;
	}

	void addHeightUpdate(int x, int z, float level)
	{
		if (heightsUpdates == null)
		{
			heightsUpdates = new List<HeightUpdate>();
		}

		heightsUpdates.Add(new HeightUpdate(x, z, level));
	}

	void Update ()
	{
		if (heightsGrid == null)
		{
			print("grid not ready");
			return;
		}

		/* use cameras current height to fake height updates */
		Vector3 gridCoord = gameObject.transform.position - heightsGrid.topLeft;
		heightsGrid.levels[(int)gridCoord.x, (int)gridCoord.z] = gridCoord.y;
		addHeightUpdate((int)gridCoord.x,
		                (int)gridCoord.z,
						heightsGrid.levels[(int)gridCoord.x,
						                   (int)gridCoord.z]);
	}
}
