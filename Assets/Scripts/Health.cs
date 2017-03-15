using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float health = 100;				// The health of the character
	public GameObject bloodParticles;		// The blood that will spill from their body as it is cleaved in two like the Red Sea. Or shot or stabbed or whatever.
	public bool bloodStains = true;			// Whether there are blood stains
	public GameObject bloodStainProjector;	// Projectors that project bloodstains on characters

	UI_Manager gameUI;



	void Start()
	{
		AssignBodyParts();

		gameUI = GameObject.Find ("UI").GetComponent<UI_Manager> ();
	}





	/// <summary>
	/// This is called by weapons when they attack and hit this player
	/// </summary>
	/// <param name="damage">The amount of damage done to the player</param>
	/// <param name="blood">Whether the weapon draws blood.</param>
	/// <param name="hitPoint">The point in world space at which the weapon hit the player</param>
	/// <param name="hitNormal">The normal of the spot that was hit</param>
	public void Hit(float damage, bool blood, Vector3 hitPoint, Vector3 hitNormal, Transform bodyPart)
	{
		health -= damage;

		if(health <= 0) Die();
		if(blood) 
		{
			BloodParticles(hitPoint, hitNormal, (bodyPart == null) ? transform : bodyPart);
			if(bloodStains)
				MakeBloodStain(hitPoint, hitNormal, (bodyPart == null) ? transform : bodyPart);
		}

		gameUI.UpdateHealthBars ();
	}



	/// <summary>
	/// Emits blood spatter from a wound
	/// </summary>
	/// <param name="point">The point in world space where the player was hit</param>
	/// <param name="normal">The normal of the spot that was hit</param>
	void BloodParticles(Vector3 point, Vector3 normal, Transform bodyPart)
	{
		if(bloodParticles == null) return;

		GameObject newBlood = (GameObject) Instantiate(
			bloodParticles,
			point,
			Quaternion.identity
		);
		newBlood.name = "Blood";
		newBlood.transform.forward = normal;
		newBlood.transform.SetParent(bodyPart);
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
		if(anim != null)
		{
			anim.Play("Death", 0);
			// Stops animation on higher layers
			anim.SetLayerWeight(1, 0);
		}

		// TODO: pool of blood

		// TODO: drop whatever is being held
		Destroy(GetComponent<Combat>());

		// TODO: angle body with floor normal

		// Disables all colliders
		// DROP ANYTHING HELD BEFORE CALLING THIS
		DisableColliders();

		// Removes character controller from player list and destroys it
		CharacterController cc = GetComponent<CharacterController>();
		if(cc != null)
		{
			PlayerController.controller.RemovePlayerFromList(cc);
			Destroy(cc);
		}

		Destroy(this);
	}



	/// <summary>
	/// Finds all body part colliders and assigns them Health_Part scripts
	/// with damage multipliers
	/// Tells the character collider to ignore raycasts (used for bullets)
	/// only if body part colliders were found
	/// </summary>
	void AssignBodyParts()
	{
		Collider[] bodyParts = GetComponentsInChildren<Collider>();

		bool foundColliders = false;

		for(int n = 0; n < bodyParts.Length; ++n)
		{
			switch (bodyParts[n].name)
			{
			case "Head":
				foundColliders = AddHealthPartScript(bodyParts[n], 4.0f);
				break;
			case "Head_end":
				foundColliders = AddHealthPartScript(bodyParts[n], 4.0f);
				break;
			case "Spine":
				foundColliders = AddHealthPartScript(bodyParts[n], 2.0f);
				break;
			case "Hip_L":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			case "LowerLeg_L":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			case "Hip_R":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			case "LowerLeg_R":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			case "UpperArm_L":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			case "Forearm_L":
				foundColliders = AddHealthPartScript(bodyParts[n], 0.5f);
				break;
			case "UpperArm_R":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			case "Forearm_R":
				foundColliders = AddHealthPartScript(bodyParts[n], 1.0f);
				break;
			}
		}

		if(foundColliders)
			gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
	}

	/// <summary>
	/// Adds Health_Part script to given collider object
	/// </summary>
	/// <returns><c>true</c>, for use in simplifying above method</returns>
	/// <param name="c">The collider object</param>
	/// <param name="damageMult">Damage multiplier for attacks</param>
	bool AddHealthPartScript(Collider c, float damageMult)
	{
		c.isTrigger = true;

		// Adds Health_Part script
		GameObject obj = c.gameObject;
		Health_Part script = obj.AddComponent<Health_Part>();
		script.MainHealth = this;
		script.DamageMultiplier = damageMult;

		// Turns off mesh renderers
		MeshRenderer mr = obj.GetComponent<MeshRenderer>();
		if(mr != null) mr.enabled = false;

		// Prevent character controller from colliding with own parts
		//Physics.IgnoreCollision(c, GetComponent<CharacterController>());
		c.gameObject.layer = 8;

		return true;
	}

	/// <summary>
	/// Disables all child colliders upon death
	/// </summary>
	void DisableColliders()
	{
		Collider[] bodyParts = GetComponentsInChildren<Collider>();
		for(int n = 0; n < bodyParts.Length; ++n)
			bodyParts[n].enabled = false;
	}
		
}
