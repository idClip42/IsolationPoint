using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

	public GameObject weapon;
	MeleeWeapon meleeScript;
	Gun gunScript;
	Animator anim;
	Camera cam;

	float timer;
	float currentMaxTime;

	Transform cameraTarget;
	float camTargetZ;

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

		CloserCamera();
	}

	public void PickUpWeapon(GameObject w)
	{
		if(weapon == null) return;
		weapon = w;
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
		} else {
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, 0.1f));
		}
	}

	public void AnimateAiming()
	{
		if(gunScript == null) return;
		//if player controller.Player or whatever isnt this guy

		float animFrame = Vector3.Angle(Vector3.up, cam.transform.forward)/180.0f;

		anim.SetLayerWeight(1, 1);
		anim.Play(gunScript.GetAnim(), 1, animFrame);
	}

	void CloserCamera()
	{
		Vector3 c = cameraTarget.localPosition;
		if(gunScript != null)
			c.z = camTargetZ/2;
		else
			c.z = camTargetZ;
		cameraTarget.localPosition = c;
	}

	// Call this from PlayerController
	public void Attack()
	{
		// TODO: Wait before allowing user to click again

		// If the held weapon is melee
		if(meleeScript != null)
		{
			if(timer > 0 && currentMaxTime - timer < meleeScript.wait)
				return;

			string animName = meleeScript.GetAnim();

			anim.Play(animName, 1);
			timer = anim.GetCurrentAnimatorClipInfo(1).Length * 0.8f;
			currentMaxTime = timer;
			meleeScript.Attack(timer);
		} else if(gunScript != null) {
			gunScript.Shoot(true);
		}
	}
}
