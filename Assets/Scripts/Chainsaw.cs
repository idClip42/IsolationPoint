using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : MonoBehaviour 
{
	GameObject particles;
	AudioSource[] audio;
	MeleeWeapon weapon;
	public GameObject chainsawBlade;
	Material bladeMat;

	void Start () 
	{
		particles = GetComponentInChildren<ParticleSystem>().gameObject;
		audio = GetComponents<AudioSource>();
		weapon = GetComponent<MeleeWeapon>();
		bladeMat = chainsawBlade.GetComponent<MeshRenderer>().material;
	}
	
	void Update () 
	{
		Collider c = weapon.Col;
		if(c.isTrigger == true)
		{
			audio[0].enabled = true;
			particles.SetActive(true);
			bladeMat.mainTextureOffset = new Vector2(Mathf.Sin(Time.time * 100) * 0.2f, bladeMat.mainTextureOffset.y);
		} else {
			audio[0].enabled = false;
			particles.SetActive(false);
		}
	}

	void OnTriggerEnter(Collider c)
	{
		//Plays a sound when hitting anything
		// Except the camera
		if(c.gameObject.tag == "MainCamera") 
			return;
		if(c.isTrigger == false) 
			return;
		audio[1].Play();
	}
}
