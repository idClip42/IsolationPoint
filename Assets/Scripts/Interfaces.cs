using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Used by player interactable items to have a common function to call
public interface IInteractable {

    //Text to be displayed detailing the interaction
    string ActionDescription();

    //Action to be done when the player interacts with this object
    void Action();
}
