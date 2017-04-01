using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasCan : MonoBehaviour 
{
	public bool hasGas;
	public float explosionRange = 10;
	public float explosionForce = 1000;
	public float explosionDamage = 100;
	public GameObject explosionParticles;
	Health healthScript;
	Collider col;
	Rigidbody rb;

	void Start () 
	{
		healthScript = GetComponent<Health>();
		if(healthScript == null)
			Debug.Log(gameObject.name + " needs a health script.");
		col = GetComponentInChildren<Collider>();
		rb = GetComponent<Rigidbody>();
	}
	
	void Update () 
	{
		Debug.DrawLine(transform.position, transform.position + transform.forward * explosionRange, Color.red);

		if(healthScript.health <= 0)
		{
			if(hasGas)
				Explode();
			DestroyCan();
		}
	}

	public void PickUpPutDown(bool pickUp, CharacterController cc)
	{
		col.isTrigger = pickUp;
		rb.isKinematic = pickUp;
	}

	void DestroyCan()
	{
		Destroy(this.gameObject);
	}

	void Explode()
	{
		Debug.Log("Ka-BOOM!");

		Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, explosionRange);
		for(int n = 0; n < nearbyColliders.Length; ++n)
		{
			Rigidbody nRb = nearbyColliders[n].GetComponent<Rigidbody>();
			if(nRb != null && nRb.isKinematic == false)
				nRb.AddExplosionForce(explosionForce, transform.position, explosionRange, 1);
			Health h = nearbyColliders[n].GetComponent<Health>();
			if(h != null)
				h.Hit(explosionDamage, false);
			Health_Part hP = nearbyColliders[n].GetComponent<Health_Part>();
			if(hP != null)
				hP.Hit(explosionDamage, false);
		}

		if(explosionParticles == null) return;
		GameObject explosion = (GameObject) Instantiate(
			explosionParticles,
			transform.position,
			Quaternion.identity
		);
		Destroy(explosion, 5.0f);
	}
}
