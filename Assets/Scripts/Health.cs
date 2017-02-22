using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {

	public float health = 100;
	public GameObject bloodParticles;
	bool dead;

	void Start()
	{
		dead = false;
	}



	public void Hit(float damage, bool blood, Vector3 hitPoint, Vector3 hitNormal)
	{
		health -= damage;

		if(health <= 0 && !dead) Die();
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
		newBlood.name = "Blood";
		newBlood.transform.forward = normal;
		Destroy(newBlood, 2);
	}




	void Die()
	{
		dead = true;
		Animator anim = GetComponentInChildren<Animator>();
		anim.Play("Death", 0);
		anim.SetLayerWeight(1, 0);
		// TODO: pool of blood

		// TODO: drop whatever is being held
		Destroy(GetComponent<Combat>());

		CharacterController cc = GetComponent<CharacterController>();

		cc.height /= 3;
		cc.center -= new Vector3(0, cc.height, 0);

		GameObject.Find("PlayerController").GetComponent<PlayerController>().RemovePlayerFromList(cc);

		//Destroy(this);
	}
}
