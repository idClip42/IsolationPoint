using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour {

	public enum AttackAnimation {Swing1, Swing2, SwingDown};
												// The options for the attack animation
	public AttackAnimation[] attackAnimations;	// An array of the animations that will play when attacking, in order
	public float damage = 25.0f;				// How much damage the weapon does
	public bool drawBlood = true;				// Whether the weapon draws blood
	public float wait = 0.5f;					// Minimum time between attacks

	Collider col;								// The weapon's collider
	public Collider Col	{ get { return col; } }

	float timer;								// used with wait variable
	int currentAnimIndex;						// The current animation to be played from attackAnimations



	BloodyKiller bloodyKiller;
	public BloodyKiller BloodyKiller { set { bloodyKiller = value; } }



	void Start () 
	{
		// Gets the collider, makes sure it's NOT a trigger
		col = GetComponent<Collider>();
		if(col == null) col = GetComponentInChildren<Collider>();
		//Debug.Log(col);
		//col.isTrigger = true;
		col.isTrigger = false;

		// Will only collide with character hit boxes
		gameObject.layer = 9;

		// Initializes values
		timer = 0;
		currentAnimIndex = 0;

		// Sets up the rigidbody
		AddKinematicRigidbody();
		
		// Will need to ignore character controller colliders
		// https://docs.unity3d.com/ScriptReference/Physics.IgnoreCollision.html
	}
	
	void Update () 
	{
		// Don't do nothin if this here collider ain't a trigger
		// Jus don't do nothin cept 'nable that collider
		if(col.isTrigger == false)
		{
			col.enabled = true;
			return;
		}

		// If the timer is going, that means an attack is happening
		// So the collider is on and the timer is ticking down
		// Otherwise, they're off
		if(timer > 0)
		{
			col.enabled = true;
			timer -= Time.deltaTime;
		} else {
			col.enabled = false;
		}
	}

	/// <summary>
	/// Gets the attack animation to be played next
	/// </summary>
	/// <returns>The animation name string.</returns>
	public string GetAnim()
	{
		AttackAnimation choice = attackAnimations[currentAnimIndex];
		++currentAnimIndex;
		if(currentAnimIndex >= attackAnimations.Length) currentAnimIndex = 0;

		return choice.ToString();
	}




	public void IsHeld(bool value)
	{
		//col.isTrigger = value;
		GetComponentInChildren<Collider>().isTrigger = value;

		GetComponent<Rigidbody>().isKinematic = value;
	}



	/// <summary>
	/// Attacks
	/// </summary>
	/// <param name="t">Attack animation time</param>
	public void Attack(float t)
	{
		// Starts the timer, which is all the weapon needs for attacking
		timer = t;
	}

	/// <summary>
	/// Adds and configures the rigidbody,
	/// which is needed for weapon collisions to register
	/// </summary>
	void AddKinematicRigidbody()
	{
		Rigidbody rb = GetComponent<Rigidbody>();
		if(rb == null)
			rb = gameObject.AddComponent<Rigidbody>();
		rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
	}


	/// <summary>
	/// Event called when the weapon hits someone
	/// </summary>
	void OnTriggerEnter(Collider c)
	{
		if(timer <= 0) return;

		// Ends the animation and attacking
		timer = 0;

		// Gets the health script of the target
		Health healthScript = c.gameObject.GetComponent<Health>();
		Health_Part healthPartScript = c.gameObject.GetComponent<Health_Part>();
		// If there's no health scripts, returns
		if(healthScript == null && healthPartScript == null) return;
		// If this collided with the player holding it, returns
		if(transform.IsChildOf(c.gameObject.transform)) return;
		if(healthPartScript != null && transform.IsChildOf(healthPartScript.MainHealth.transform)) return;

		// If colliding with another character, does damage
		Vector3 point = c.ClosestPointOnBounds(transform.position);
		Vector3 normal = Vector3.up;	// Perhaps this should be the dif between the closest point and the transform from above
		if(healthScript != null)
			healthScript.Hit(damage, drawBlood, point, normal, null);
		else if(healthPartScript != null)
			healthPartScript.Hit(damage, drawBlood, point, normal);

		if(bloodyKiller != null)
			bloodyKiller.doBlood();
	}
}
