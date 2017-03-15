using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chandelier : MonoBehaviour 
{
	public float damage = 100.0f;
	Health health;
	bool down;
	Rigidbody rb;

	void Start () 
	{
		health = GetComponent<Health>();
		down = false;
	}
	
	void Update () 
	{
		if(down == true) return;

		if(health.health <= 0)
		{
			down = true;
			rb = gameObject.AddComponent<Rigidbody>();
			GetComponent<Collider>().enabled = true;
		}
	}

	void OnCollisionEnter(Collision c)
	{

		//Debug.Log("Hit " + c.gameObject.name + " at velocity " + rb.velocity.magnitude);
		if(rb == null) return;
		if(rb.velocity.sqrMagnitude < 1) return;
		Health targetHealth = c.gameObject.GetComponent<Health>();
		Health_Part targetHealthPart = c.gameObject.GetComponent<Health_Part>();
		if(targetHealth != null)
		{
			targetHealth.Hit(damage, false, c.contacts[0].point, c.contacts[0].normal, null);
		}
		if(targetHealthPart != null)
		{
			targetHealthPart.Hit(damage, false, c.contacts[0].point, c.contacts[0].normal);
		}

	}
}
