using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnlockDoorObject : MonoBehaviour, IInteractable {

    public string actionDesc;   //Modifiable because this can be applied to any object that could unlock a door(or several)
    public string actionDesc2;  //Used if this object can relock doors
    public GameObject[] doorsToUnlock;  //List of doors this action will unlock
    public bool setRigidBodyKinematic = false;   //If a rigid body is attached set it to be this value
    bool isUsed = false;
    public bool relockable = false; //Is this a one time thing? Yes = false. Can this relock the doors? Yes = true.
    public Objective toAffect;  //Objective affected by this action
	Generator generatorScript;

    public void Action()
    {
        if (isUsed) return;
        foreach(GameObject d in doorsToUnlock)
        {
            d.GetComponent<DoorMovement>().IsLocked = !d.GetComponent<DoorMovement>().IsLocked;
        }
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = setRigidBodyKinematic; }
        isUsed = true;
        if (relockable)
        {
            isUsed = false;
            string temp = actionDesc;
            actionDesc = actionDesc2;
            actionDesc2 = temp;
        }
        if (toAffect != null)
        {
            toAffect.IsCompleted = true;
        }
		generatorScript.SwitchLights (false);
    }

    public string ActionDescription()
    {
        if (isUsed) return " ";
        return actionDesc;
    }

    // Use this for initialization
    void Start () {
		generatorScript = GameObject.Find ("Generator").GetComponent<Generator> ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
