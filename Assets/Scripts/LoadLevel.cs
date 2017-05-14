using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadLevel : MonoBehaviour {

	Button playButton;

	public string sceneName;

	bool loadingLevel;

	AsyncOperation asyncLoad;



	// Use this for initialization
	void Start () {
		playButton = GetComponent<Button> ();
		playButton.onClick.AddListener (onclick);
		loadingLevel = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(loadingLevel == true && asyncLoad != null)
			GetComponentInChildren<Text>().text = "Loading: " + asyncLoad.progress * 100 + "%";
	}

	public void onclick(){
		//Debug.Log ("New scene should load");
		//Application.LoadLevel (sceneIndex);
		//SceneManager.LoadScene(sceneName);

		if(loadingLevel == true) return;	// Not sure why onclick is being called twice, but this helps with that
		ChangeToLoad();
		loadingLevel = true;
		asyncLoad = SceneManager.LoadSceneAsync(sceneName);
		//Debug.Log("Click! " + gameObject.name);
	}


	void ChangeToLoad(){
		Button[] buttons = Object.FindObjectsOfType<Button>();
		for(int n = 0; n < buttons.Length; ++n)
			if(buttons[n].gameObject != this.gameObject)
				buttons[n].gameObject.SetActive(false);
			else
				buttons[n].enabled = false;


	}

}
