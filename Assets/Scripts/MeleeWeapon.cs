using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour {

	public enum AttackAnimation {Swing1, Swing2, SwingDown};
	public AttackAnimation[] attackAnimations;
	public float damage = 25.0f;
	public bool drawBlood = true;
	public float wait = 0.5f;

	Collider col;

	float timer;
	int currentAnimIndex;


	void Start () 
	{
		col = GetComponent<Collider>();
		col.isTrigger = true;

		timer = 0;
		currentAnimIndex = 0;

		AddKinematicRigidbody();
	}
	
	void Update () 
	{
		if(timer > 0)
		{
			col.enabled = true;
			timer -= Time.deltaTime;
		} else {
			col.enabled = false;
		}
	}

	public string GetAnim()
	{
		AttackAnimation choice = attackAnimations[currentAnimIndex];
		++currentAnimIndex;
		if(currentAnimIndex >= attackAnimations.Length) currentAnimIndex = 0;

		return choice.ToString();
	}


	public void Attack(float t)
	{
		timer = t;
	}


	void AddKinematicRigidbody()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if(rb == null)
			rb = gameObject.AddComponent<Rigidbody>();
		rb.isKinematic = true;
		rb.useGravity = false;
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}

	void OnTriggerEnter(Collider c)
	{
		if(timer <= 0) return;

		timer = 0;
		Health healthScript = c.gameObject.GetComponent<Health>();

		// If there's no health script, returns
		if(healthScript == null) return;
		// If collided with the player holding it, returns
		if(transform.IsChildOf(c.gameObject.transform)) return;

		// If colliding with another character, does damage
		Vector3 point = c.ClosestPointOnBounds(transform.position);
		Vector3 normal = Vector3.up;	// Perhaps this should be the dif between the closest point and the transform from above
		healthScript.Hit(damage, drawBlood, point, normal);
	}
}
