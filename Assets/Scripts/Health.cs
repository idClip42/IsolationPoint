using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float health = 100;
	public GameObject bloodParticles;



	public void Hit(float damage, bool blood, Vector3 hitPoint, Vector3 hitNormal)
	{
		health -= damage;

		if(health <= 0) Die();
		if(blood) BloodParticles(hitPoint, hitNormal);
	}




	void BloodParticles(Vector3 point, Vector3 normal)
	{
		if(bloodParticles == null) return;

		GameObject newBlood = (GameObject) Instantiate(
			bloodParticles,
			point,
			Quaternion.identity
		);
		newBlood.transform.forward = normal;
		Destroy(newBlood, 3);
	}




	void Die()
	{
		Animator anim = GetComponentInChildren<Animator>();
		anim.Play("Death", 0);
		// Should probably drop weapon
		Destroy(GetComponent<Combat>());

		CharacterController cc = GetComponent<CharacterController>();

		cc.height /= 3;
		cc.center -= new Vector3(0, cc.height, 0);

		GameObject.Find("PlayerController").GetComponent<PlayerController>().RemovePlayerFromList(cc);

		Destroy(this);
	}
}
