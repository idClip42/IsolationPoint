using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasSource : MonoBehaviour, IInteractable
{

	public string ActionDescription(){
		GasCan gasScript = GetCurrentGasScript();
		if(gasScript == null)
			return "You need a gas can to fill here";
		else if(gasScript.hasGas == true)
			return "The gas can is filled";
		else
			return "Fill gas can";
	}

	public void Action(){
		GasCan gasScript = GetCurrentGasScript();
		if(gasScript.hasGas == false)
			gasScript.hasGas = true;
	}

	GasCan GetCurrentGasScript()
	{
		CharacterController currentPlayer = PlayerController.controller.Player;
		Pickup_Drop_Items pickupScript = currentPlayer.GetComponent<Pickup_Drop_Items>();
		GameObject item = pickupScript.LeftHandItem;
		if(item == null) return null;
		GasCan gasScript = item.GetComponent<GasCan>();
		return gasScript;
	}
}
