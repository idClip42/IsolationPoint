using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour {

	//CharacterController[] players;
	UI_Manager ui;
	Helicopter heli;
	Font font;
	bool gameEnd;

	void Start () 
	{
		//players = PlayerController.controller.playerList;
		ui = Object.FindObjectOfType<UI_Manager>();
		heli = Object.FindObjectOfType<Helicopter>();
		font = GameObject.Find("Controls").GetComponent<Text>().font;
		gameEnd = false;

		//CreateUI(3, 4);
	}
	
	void Update () 
	{
		int escaped = 0;
		int dead = 0;

		if(gameEnd == false)
		{
			gameEnd = true;

			CharacterController[] players = PlayerController.controller.playerList;

			for(int p = 0; p < players.Length; ++p)
			{
				if(players[p].GetComponent<Health>() == null)
					++dead;
				else if(players[p].gameObject.activeSelf == false)
					++escaped;
				else
					gameEnd = false;
			}

			if(gameEnd == true)
			{

				PlayerController.controller.enabled = false;

				// Disable UI
				ui.enabled = false;
				// Then free cursor
				Cursor.visible = true;
				Cursor.lockState = CursorLockMode.None;

				CreateUI(escaped, 4);
				HelicopterLeave();
			}
		} else {

		}

	}

	void CreateUI(int survivors, int total)
	{
		Debug.Log("CreateUI()");

		GameObject gameOver = new GameObject();
		gameOver.name = "GameOverMessage";
		gameOver.transform.SetParent(ui.transform);

		RectTransform t = gameOver.AddComponent<RectTransform>();
		t.anchorMin = new Vector2(0,0);
		t.anchorMax = new Vector2(1,1);
		t.pivot = new Vector2(0.5f, 0.5f);
		t.offsetMin = new Vector2(0,200);
		t.offsetMax = new Vector2(0,0);

		Text text = gameOver.AddComponent<Text>();
		text.text = "Game Over";
		text.font = font;
		text.alignment = TextAnchor.MiddleCenter;
		text.fontSize = 100;


		GameObject message = new GameObject();
		message.name = "GameOverSurvivors";
		message.transform.SetParent(gameOver.transform);

		RectTransform t2 = message.AddComponent<RectTransform>();
		t2.anchorMin = new Vector2(0,0);
		t2.anchorMax = new Vector2(1,1);
		t2.pivot = new Vector2(0.5f, 0.5f);
		t2.offsetMin = new Vector2(0,-200);
		t2.offsetMax = new Vector2(0,0);

		Text text2 = message.AddComponent<Text>();
		text2.text = survivors + " out of " + total + " survived";
		text2.font = font;
		text2.alignment = TextAnchor.MiddleCenter;
		text2.fontSize = 50;


		GameObject button = new GameObject();
		button.name = "GameOverButton";
		button.transform.SetParent(gameOver.transform);

		RectTransform t3 = button.AddComponent<RectTransform>();
		t3.anchorMin = new Vector2(0,0);
		t3.anchorMax = new Vector2(1,1);
		t3.pivot = new Vector2(0.5f, 0.5f);
		t3.offsetMin = new Vector2(150,-100);
		t3.offsetMax = new Vector2(-150,-275);

		Text text3 = button.AddComponent<Text>();
		text3.text = "Return to Main Menu";
		text3.font = font;
		text3.alignment = TextAnchor.MiddleCenter;
		text3.fontSize = 50;

		Button b = button.AddComponent<Button>();
		ColorBlock c = b.colors;
		c.normalColor = Color.white;
		c.highlightedColor = Color.red;
		b.colors = c;


		LoadLevel level = button.AddComponent<LoadLevel>();
		level.sceneName = "MainMenu";
	}

	void HelicopterLeave()
	{
		if(heli.heliModel.activeSelf == false) return;
	}
}
