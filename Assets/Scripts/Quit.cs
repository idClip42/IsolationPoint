using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Quit : MonoBehaviour {

	Button quitButton;
	// Use this for initialization
	void Start () {
		quitButton = GetComponent<Button> ();
		quitButton.onClick.AddListener (onclick);
	}
	
	// Update is called once per frame
	void Update () {
			
	}

	public void onclick(){
		Debug.Log ("Should exit application");
		Application.Quit ();
	}
}
