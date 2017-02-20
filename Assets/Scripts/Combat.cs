using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour {

	public GameObject weapon;
	MeleeWeapon meleeScript;
	Animator anim;

	float timer;

	void Start () 
	{
		if(weapon != null) meleeScript = weapon.GetComponent<MeleeWeapon>();
		anim = GetComponentInChildren<Animator>();
		timer = 0;
	}
	
	void Update () 
	{
		if(timer > 0)
		{
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 1, 0.1f));
			timer -= Time.deltaTime;
		} else {
			anim.SetLayerWeight(1, Mathf.Lerp(anim.GetLayerWeight(1), 0, 0.1f));
		}
	}

	// Call this from PlayerController
	public void Attack()
	{
		// Prevent multiple clicks based on... layer weight maybe?
		// Or duration I guess.
		// Yeah. Probably duration.

		// If the held weapon is melee
		if(meleeScript != null)
		{
			string animName = meleeScript.GetAnim();

			anim.Play(animName, 1);
			timer = anim.GetCurrentAnimatorClipInfo(1).Length * 0.8f;
			meleeScript.Attack(timer);
		}
	}
}
