using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

	public GameObject weapon;		// Weapon that is currently held

	public bool accurateWithGun = false;

	MeleeWeapon meleeScript;		// The melee script for the weapon, if it is a melee weapon
	Gun gunScript;					// The gun script for the weapon, if it is a gun
	Animator anim;					// The player animator
	Camera cam;						// The player camera

	float timer;					// A timer that determines when player is attacking, or when they can attack, or whatever
	float currentMaxTime;			// Used for determining how much time has passed since melee animation has started

	Transform cameraTarget;			// The location the camera moves to
	float camTargetZ;				// The Z offset from the player of the camera target

	bool isAiming;					// Whether the player is aiming
	public bool IsAiming {
		get { return isAiming; }
	}

	UI_Manager UIScript;

	int weaponsLayerIndex;


	void Start () 
	{
		UIScript = GameObject.Find ("UI").GetComponent<UI_Manager> ();
		// Equips any weapon that is already in the public variable in the inspector
		PickUpWeapon(weapon);

		anim = GetComponentInChildren<Animator>();
		cam = Camera.main;
		timer = 0;
		currentMaxTime = 0;

		cameraTarget = transform.FindChild("CameraAxis").FindChild("CameraTarget");
		if(cameraTarget == null) Debug.Log("Must have camera target named 'CameraTarget'");
		camTargetZ = cameraTarget.localPosition.z;

		weaponsLayerIndex = anim.GetLayerIndex("WeaponLayer");
	}
	
	void Update () 
	{
		if(PlayerController.controller.Player.gameObject != this.gameObject && this.gameObject.tag != "Enemy")
		{
			anim.SetLayerWeight (weaponsLayerIndex, Mathf.Lerp (anim.GetLayerWeight (weaponsLayerIndex), 0, 0.1f));
			return;
		}

		if (weapon == null) {
			// If no new weapon, ensures that no weapon anims continue
			isAiming = false;
			PlayerController.controller.SetAimMode(false);
			anim.SetLayerWeight (weaponsLayerIndex, Mathf.Lerp (anim.GetLayerWeight (weaponsLayerIndex), 0, 0.1f));
			Vector3 c = cameraTarget.localPosition;
			c.z = camTargetZ;
			cameraTarget.localPosition = c;
		}

		AnimateMelee();
		AnimateAiming();


		if(gameObject.tag == "Enemy") return;
		if (gunScript != null && isAiming == true) {
			gunScript.UpdateCrosshair ();
		} 
		else {
			UIScript.ResetCrosshair ();
		}
	}


	/// <summary>
	/// Equips this player with a weapon
	/// </summary>
	/// <param name="w">The weapon to be equipped. Set to null if just dropping the weapon.</param>
	public void PickUpWeapon(GameObject w)
	{
		// Drops current weapon
		if(meleeScript != null)
			meleeScript.IsHeld(false);
		if(gunScript != null)
			gunScript.IsHeld(false);
		if(weapon != null) 
			weapon.transform.parent = null;
		if(gunScript != null) 
			gunScript.CombatScript = null;
		weapon = null;
		meleeScript = null;
		gunScript = null;

		// Equips new weapon
		weapon = w;
		if(weapon == null)
			return;

		meleeScript = weapon.GetComponent<MeleeWeapon>();
		gunScript = weapon.GetComponent<Gun>();
		if(meleeScript != null)
			meleeScript.IsHeld(true);
		if(gunScript != null)
		{
			gunScript.IsHeld(true);
			gunScript.CombatScript = this;
		}
	}


	/// <summary>
	/// Animates the melee attacks
	/// </summary>
	public void AnimateMelee()
	{
		// Does nothing if current weapon is not melee
		if(meleeScript == null) return;

		// Sets the weight of the upper body layer based on whether attacks are happening
		// Deals with timer
		// Sets whether character is aiming based on whether they're attacking
		if(timer > 0)
		{
			anim.SetLayerWeight(weaponsLayerIndex, Mathf.Lerp(anim.GetLayerWeight(weaponsLayerIndex), 1, 0.1f));
			timer -= Time.deltaTime;
			if(PlayerController.controller.Player.gameObject == this.gameObject)
				PlayerController.controller.SetAimMode(true);
		} else {
			anim.SetLayerWeight(weaponsLayerIndex, Mathf.Lerp(anim.GetLayerWeight(weaponsLayerIndex), 0, 0.1f));
			if(PlayerController.controller.Player.gameObject == this.gameObject)
				PlayerController.controller.SetAimMode(false);
		}
	}



	/// <summary>
	/// Handles gun aiming
	/// </summary>
	public void AnimateAiming()
	{

		// Makes sure this is the current player and they have a gun
		// Then checks if they're aiming
		if(gunScript == null) 
			return;
		if(Input.GetButton("AimWeapon"))
			isAiming = true;
		else
			isAiming = false;

		// If aiming,
		// Top half of body aims gun
		// Character always faces forward
		// Camera moves in closer
		Vector3 c = cameraTarget.localPosition;
		if(isAiming) 
		{
			anim.SetLayerWeight(weaponsLayerIndex, Mathf.Lerp(anim.GetLayerWeight(weaponsLayerIndex), 1, 0.5f));
			if(PlayerController.controller.Player.gameObject == this.gameObject)
			{
				PlayerController.controller.SetAimMode(true);
				c.z = camTargetZ/3;
			}
		} else 
		{
			anim.SetLayerWeight(weaponsLayerIndex, Mathf.Lerp(anim.GetLayerWeight(weaponsLayerIndex), 0, 0.1f));
			if(PlayerController.controller.Player.gameObject == this.gameObject)
			{
				PlayerController.controller.SetAimMode(false);
				c.z = camTargetZ;
			}
		}

		if(PlayerController.controller.Player.gameObject != this.gameObject) return;

		cameraTarget.localPosition = c;

		// Points gun aim animation in correct vertical direction
		float animFrame = Vector3.Angle(Vector3.up, cam.transform.forward)/180.0f;
		anim.Play(gunScript.gunAnim.ToString(), weaponsLayerIndex, animFrame);
	}



	/// <summary>
	/// Player attacks with current weapon
	/// Called from the player controller
	/// </summary>
	public void Attack()
	{
		// If the held weapon is melee
		if(meleeScript != null)
		{
			// Perhaps move this stuff into the melee weapon script
			if(timer > 0 && currentMaxTime - timer < meleeScript.wait)
				return;

			string animName = meleeScript.GetAnim();

			anim.Play(animName, weaponsLayerIndex);
			timer = anim.GetCurrentAnimatorClipInfo(weaponsLayerIndex).Length * 0.8f;
			currentMaxTime = timer;
			meleeScript.Attack(timer);
		} else if(gunScript != null && isAiming) {
			gunScript.Shoot(accurateWithGun);
		}
	}
}
