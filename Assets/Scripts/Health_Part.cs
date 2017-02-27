using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Part : MonoBehaviour 
{
	Health mainHealthScript;
	public Health MainHealth{ get { return mainHealthScript; } set { mainHealthScript = value; } }

	float damageMultiplier;
	public float DamageMultiplier{ set { damageMultiplier = value; } }

	public void Hit(float damage, bool blood, Vector3 hitPoint, Vector3 hitNormal)
	{
		if(mainHealthScript == null) return;
		mainHealthScript.Hit(damage * damageMultiplier, blood, hitPoint, hitNormal, transform);
	}
}
