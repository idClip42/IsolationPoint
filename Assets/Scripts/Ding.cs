using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ding : MonoBehaviour, IInteractable {

	AudioSource ding;

	void Start()
	{
		ding = GetComponent<AudioSource>();
	}

	public string ActionDescription()
	{
		return "Ding";
	}

	public void Action()
	{
		ding.time = 1;
		ding.Play();
	}
}
