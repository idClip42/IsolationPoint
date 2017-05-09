using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHouseMaterials : MonoBehaviour 
{
	public GameObject[] floors;
	public Material[] floorMats;
	public GameObject[] walls;
	public Material[] wallMats;
	public GameObject[] exterior;
	public Material[] extMats;
	public GameObject[] distLightCube;
	public Material[] lightMats;

	//void Start () 
	void Awake()
	{
		SetMats(floors, floorMats);
		SetMats(walls, wallMats);
		SetMats(exterior, extMats);
		SetMats(distLightCube, lightMats);
	}

	void SetMats(GameObject[] objs, Material[] mats)
	{
		for(int n = 0; n < objs.Length; ++n)
			objs[n].GetComponent<MeshRenderer>().material = mats[Random.Range(0, mats.Length)];
	}
}
