using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustLODs : MonoBehaviour 
{
	/// <summary>
	///  Changes all LODs that are children of this
	/// </summary>


	[Range(0, 1)] public float LOD1Percentage;
	[Range(0, 1)] public float LOD2Percentage;

	LODGroup[] lods;

	void Start () 
	{
		lods = GetComponentsInChildren<LODGroup>();

		for(int n = 0; n < lods.Length; ++n)
		{
			LODGroup l = lods[n];
			LOD[] stuff = l.GetLODs();
			stuff[0].screenRelativeTransitionHeight = LOD1Percentage;
			if(stuff.Length > 2)
				stuff[1].screenRelativeTransitionHeight = LOD2Percentage;
			l.SetLODs(stuff);
		}
	}
}
