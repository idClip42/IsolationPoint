using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Escape : MonoBehaviour, IInteractable 
{

	void Start () 
	{
		
	}
	
	void Update () 
	{
		
	}

	public string ActionDescription(){
		return "Escape!";
	}

	public void Action(){
		CharacterController player = PlayerController.controller.Player;
		PlayerController.controller.RemovePlayerFromList(player, false);
		PlayerController.controller.SwapToNextChar();
		player.gameObject.SetActive(false);
	}
}
