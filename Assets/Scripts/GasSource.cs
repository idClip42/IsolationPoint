using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasSource : MonoBehaviour, IInteractable
{
    public float fillTime;
    float timer = 0.0f;
    GasCan gasCan;
    Follower worker;
    bool isFilling = false;
    bool isFull = false;
    public bool IsFilling
    {
        get { return isFilling; }
        set
        {
            isFilling = value;
            if (gasCan == null) return;

            if (isFilling)
            {
                worker = PlayerController.controller.FollowScript;
                worker.IsWorking = true;
                UI_Manager.UIManager.ResetBar();
            }
            else
            {
                if (isFull)
                {
                    gasCan.hasGas = true;
                    isFull = false;
                }
                worker.IsWorking = false;
                worker = null;
                gasCan = null;
            }
        }
    }


    void Update()
    {
        if (!IsFilling) return;

        timer += Time.deltaTime;
        UI_Manager.UIManager.UpdateBar(timer / fillTime);
        if(timer > fillTime)
        {
            isFull = true;
            IsFilling = false;
            timer = 0;
        }
    }


	public string ActionDescription(){
		GasCan gasScript = GetCurrentGasScript();
        if (gasScript == null)
            return "You need a gas can to fill here";
        else if (IsFilling)
            return "Filling gas can";
        else if (gasScript.hasGas == true)
            return "The gas can is filled";
        else
            return "Fill gas can";
	}

	public void Action(){
        //return if pump is in use
        if (IsFilling) return;
        //get the current gas can from the player
        gasCan = GetCurrentGasScript();
        //do nothing if there is no gas can or the can is full
        if (gasCan == null || gasCan.hasGas) return;
        IsFilling = true;
        /*
		if(gasCan.hasGas == false)
            gasCan.hasGas = true;
            */
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
