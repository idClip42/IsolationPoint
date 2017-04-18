using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chainsaw : MonoBehaviour 
{
	GameObject particles;
	AudioSource[] audio;
	MeleeWeapon weapon;

	void Start () 
	{
		particles = GetComponentInChildren<ParticleSystem>().gameObject;
		audio = GetComponents<AudioSource>();
		weapon = GetComponent<MeleeWeapon>();
	}

	// Can we tell its hit something when the collider is disabled?
	// No, collider is enabled when swinging
	// Have two separate audio sources?
	
	void Update () 
	{
		Collider c = weapon.Col;
		if(c.isTrigger == true)
		{
			audio[0].enabled = true;
			particles.SetActive(true);

			//if(c.enabled == true)
			//{
			//	if(!audio[1].isPlaying)
			//	{
			//		audio[1].Play();
			//	}
			//} else 
			//	audio[1].Stop();
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
		audio[1].Play();
	}
}
