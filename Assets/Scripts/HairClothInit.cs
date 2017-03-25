using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HairClothInit : MonoBehaviour 
{
	/*
	 * 
	 * Takes a hair/fur mesh and 
	 * figures out which vertices
	 * to keep in place on the cloth mesh
	 * so that the hair/fur moves around
	 * 
	 */

	public enum Direction {LEFT, DOWN};
	public Direction hairTextureDirection = Direction.LEFT;



	void Start () 
	{
		Mesh mesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
		Cloth cloth = GetComponent<Cloth>();

		ClothSkinningCoefficient[] newConstraints = cloth.coefficients;


		List <Vector3> staticPoints = new List<Vector3>();
		float lowestX = 1.0f;
		float highestX = 0.0f;

		for(int n = 0; n < mesh.vertexCount; ++n)
		{
			if(hairTextureDirection == Direction.LEFT)
			{
				if(mesh.uv[n].x < lowestX)
					lowestX = mesh.uv[n].x;
				if(mesh.uv[n].x > highestX)
					highestX = mesh.uv[n].x;
			} else if(hairTextureDirection == Direction.DOWN)
			{
				if(mesh.uv[n].y < lowestX)
					lowestX = mesh.uv[n].y;
				if(mesh.uv[n].y > highestX)
					highestX = mesh.uv[n].y;
			}
		}

		float threshold = 0;
		if(hairTextureDirection == Direction.LEFT)
			threshold = lowestX + (highestX-lowestX)/4.0f;
		else if(hairTextureDirection == Direction.DOWN)
			threshold = highestX - (highestX-lowestX)/4.0f;

		for(int n = 0; n < mesh.vertexCount; ++n)
		{
			if(hairTextureDirection == Direction.LEFT && mesh.uv[n].x < threshold)
				staticPoints.Add(mesh.vertices[n]);
			if(hairTextureDirection == Direction.DOWN && mesh.uv[n].y > threshold)
				staticPoints.Add(mesh.vertices[n]);
		}

		for(int n = 0; n < newConstraints.Length; ++n)
		{
			Vector3 vert = cloth.vertices[n] / transform.lossyScale.x;

			foreach(Vector3 stat in staticPoints)
			{
				if(stat == vert)
				{
					newConstraints[n].maxDistance = 0;
				}
			}
		}

		cloth.coefficients = newConstraints;

		Debug.Log("Updated Cloth constraints. Now please for the love of god copy the new ones and apply them to the original outside of play mode so we don't do this every time.");
	}
}
