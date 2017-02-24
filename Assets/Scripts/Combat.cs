using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

	public GameObject weapon;		// Weapon that is currently held
	MeleeWeapon meleeScript;		// The melee script for the weapon, if it is a melee weapon
	Gun gunScript;					// The gun script for the weapon, if it is a gun
	Animator anim;					// The player animator
	Camera cam;						// The player camera

	float timer;					// A timer that determines when player is attacking, or when they can attack, or whatever
	float currentMaxTime;			// Used for determining how much time has passed since melee animation has started

	Transform cameraTarget;			// The location the camera moves to
	float camTargetZ;				// The Z offset from the player of the camera target

	bool isAiming;					// Whether the player is aiming




	void Start () 
	{
		PickUpWeapon(weapon);
		anim = GetComponentInChildren<Animator>();
		cam = Camera.main;
		timer = 0;
		currentMaxTime = 0;

		cameraTarget = transform.FindChild("CameraAxis").FindChild("CameraTarget");
		if(cameraTarget == null) Debug.Log("Must have camera target named 'CameraTarget'");
		camTargetZ = cameraTarget.localPosition.z;
	}
	
	void Update () 
	{
		AnimateMelee();
		AnimateAiming();
	}

	public void PickUpWeapon(GameObject w)
	{
		// Drops current weapon
		weapon = null;
		meleeScript = null;
		gunScript = null;

		// Equips new weapon
		weapon = w;
		if(weapon == null) return;
		meleeScript = weapon.GetComponent<MeleeWeapon>();
		gunScript = weapon.GetComponent<Gun>();
	}

	public void AnimateMelee()
	{
		if(meleeScript == null) return;

		if(timer > 0)
		{
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1, 0.1f));
			timer -= Time.deltaTime;
			PlayerController.controller.SetAimMode(true);
		} else {
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, 0.1f));
			PlayerController.controller.SetAimMode(false);
		}
	}

	public void AnimateAiming()
	{
		// Makes sure this is the current player and they have a gun
		// Then checks if they're aiming
		if(gunScript == null) 
			return;
		else if(PlayerController.controller.Player.gameObject != this.gameObject)
			return;
		else if(!Input.GetButton("AimWeapon"))
			isAiming = false;
		else
			isAiming = true;

		// If aiming,
		// Top half of body aims gun
		// Character always faces forward
		// Camera moves in closer
		Vector3 c = cameraTarget.localPosition;
		if(isAiming) 
		{
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1, 0.5f));
			PlayerController.controller.SetAimMode(true);
			c.z = camTargetZ/3;
		} else 
		{
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, 0.1f));
			PlayerController.controller.SetAimMode(false);
			c.z = camTargetZ;
		}
		cameraTarget.localPosition = c;

		// Points gun aim animation in correct vertical direction
		float animFrame = Vector3.Angle(Vector3.up, cam.transform.forward)/180.0f;
		anim.Play(gunScript.GetAnim(), 1, animFrame);
	}

	// Call this from PlayerController
	public void Attack()
	{
		// If the held weapon is melee
		if(meleeScript != null)
		{
			// Perhaps move this stuff into the melee weapon script
			if(timer > 0 && currentMaxTime - timer < meleeScript.wait)
				return;

			string animName = meleeScript.GetAnim();

			anim.Play(animName, 1);
			timer = anim.GetCurrentAnimatorClipInfo(1).Length * 0.8f;
			currentMaxTime = timer;
			meleeScript.Attack(timer);
		} else if(gunScript != null && isAiming) {
			gunScript.Shoot(true);
		}
	}
}
