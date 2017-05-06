using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodyKiller : MonoBehaviour 
{
	public GameObject body;
	public Texture[] bloodTexs;
	Material mat;
	MeleeWeapon melee;
	int index;

	void Start () 
	{
		mat = body.GetComponent<SkinnedMeshRenderer>().material;
		mat.EnableKeyword("_DETAIL_MULX2");
		melee = null;
		index = -1;

	}
	
	void Update () 
	{
		MeleeWeapon m = GetComponentInChildren<MeleeWeapon>();
		if(m != melee)
		{
			if(melee != null)
				melee.BloodyKiller = null;
			melee = m;
			if(melee != null)
				melee.BloodyKiller = this;
		}
	}

	public void doBlood()
	{
		Debug.Log("doBlood()");
		++index;
		if(index < bloodTexs.Length)
		{
			mat.SetTexture("_DetailAlbedoMap", bloodTexs[index]);
		}
	}
}
