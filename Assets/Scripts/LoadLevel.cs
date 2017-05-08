using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

	Button playButton;

	public int sceneIndex;
	// Use this for initialization
	void Start () {
		playButton = GetComponent<Button> ();
		playButton.onClick.AddListener (onclick);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onclick(){
		Debug.Log ("New scene should load");
		Application.LoadLevel (sceneIndex);
	}
}
