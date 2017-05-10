using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ding : MonoBehaviour, IInteractable {

	AudioSource ding;
	float[] forWhomTheBellTolls;
	int index;

	void Start()
	{
		ding = GetComponent<AudioSource>();
		forWhomTheBellTolls = new float[] {1, 1, 0.9f, 1.2f, 0.9f};
		index = 0;
	}

	public string ActionDescription()
	{
		return "Ding";
	}

	public void Action()
	{
		ding.pitch = forWhomTheBellTolls[index];
		++index;
		if(index >= forWhomTheBellTolls.Length) index = 0;
		ding.time = 1;
		ding.Play();
	}
}
