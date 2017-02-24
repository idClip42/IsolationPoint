using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float health = 100;				// The health of the character
	public GameObject bloodParticles;		// The blood that will spill from their body as it is cleaved in two like the Red Sea. Or shot or stabbed or whatever.
	public GameObject bloodStainProjector;	// Projectors that project bloodstains on characters




	/// <summary>
	/// This is called by weapons when they attack and hit this player
	/// </summary>
	/// <param name="damage">The amount of damage done to the player</param>
	/// <param name="blood">Whether the weapon draws blood.</param>
	/// <param name="hitPoint">The point in world space at which the weapon hit the player</param>
	/// <param name="hitNormal">The normal of the spot that was hit</param>
	public void Hit(float damage, bool blood, Vector3 hitPoint, Vector3 hitNormal)
	{
		health -= damage;

		if(health <= 0) Die();
		if(blood) 
		{
			BloodParticles(hitPoint, hitNormal);
			MakeBloodStain(hitPoint, hitNormal, transform);
		}
	}



	/// <summary>
	/// Emits blood spatter from a wound
	/// </summary>
	/// <param name="point">The point in world space where the player was hit</param>
	/// <param name="normal">The normal of the spot that was hit</param>
	void BloodParticles(Vector3 point, Vector3 normal)
	{
		if(bloodParticles == null) return;

		GameObject newBlood = (GameObject) Instantiate(
			bloodParticles,
			point,
			Quaternion.identity
		);
		newBlood.name = "Blood";
		newBlood.transform.forward = normal;
		Destroy(newBlood, 2);
	}

	/// <summary>
	/// Makes a blood stain projector
	/// </summary>
	/// <param name="point">The point in world space where the player was hit</param>
	/// <param name="normal">The normal of the spot that was hit</param>
	/// <param name="collider">The collider to parent the projector to</param>
	void MakeBloodStain(Vector3 point, Vector3 normal, Transform collider)
	{
		if(bloodStainProjector == null) return;

		GameObject b = (GameObject) Instantiate(
			bloodStainProjector,
			point + normal * 0.25f,
			Quaternion.identity
		);
		b.transform.forward = -normal;
		b.transform.SetParent(collider);
		Destroy(b, 30);
	}



	/// <summary>
	/// The character dies.
	/// </summary>
	void Die()
	{
		// Plays death animation
		Animator anim = GetComponentInChildren<Animator>();
		anim.Play("Death", 0);

		// Stops animation on higher layers
		anim.SetLayerWeight(1, 0);

		// TODO: pool of blood

		// TODO: drop whatever is being held
		Destroy(GetComponent<Combat>());

		// TODO: angle body with floor normal

		// Removes character controller from player list and destroys it
		CharacterController cc = GetComponent<CharacterController>();
		PlayerController.controller.RemovePlayerFromList(cc);
		Destroy(cc);

		Destroy(this);
	}
}
