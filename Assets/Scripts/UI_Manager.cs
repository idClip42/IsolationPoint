using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : MonoBehaviour {

	GameObject UI;
	Generator gScript;

	public Sprite redDot;
	public Sprite redCross;

	List<Health> healthList;
	List<Image> healthBarList;

	public Image crosshair;
	public Image Gbar;
	Image GbarPrefab;

	Text pauseMenu;

	Vector2 crosshairDefault;

	bool pauseGame;
	public bool fixingGenerator;

	float timer;

	void Start () {
		gScript = GameObject.Find ("Generator").GetComponent<Generator> ();
		crosshairDefault = new Vector2 (0.5f, 0.5f);
		healthList = new List<Health> ();
		healthBarList = new List<Image> ();
		pauseGame = false;
		fixingGenerator = false;
		timer = 0;
		UI = GameObject.Find ("UI");
		//if(UI == null) UI = GameObject.Find("UI 1");
		//if(UI == null) Debug.Log("Didn't find UI object");
		GetCrosshair ();
		GetGBar ();
		GetHealthScripts ();
		GetHealthBars ();
		GetPauseMenu ();

		GbarPrefab.enabled = false;
		Gbar.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			pauseGame = !pauseGame;
		}

		if (Input.GetButtonDown("AimWeapon")) {
			crosshair.sprite = redCross;
		}

		if (Input.GetButtonUp("AimWeapon")) {
			crosshair.sprite = redDot;
		}

		if (pauseGame) {
			pauseMenu.enabled = true;
			Time.timeScale = 0.0f;
		} else {
			pauseMenu.enabled = false;
			Time.timeScale = 1.0f;
		}

		if (fixingGenerator) {
			UpdateGbar ();
		}
		
	}

	/// <summary>
	/// Gets the health script for each player in the scene
	/// </summary>
	void GetHealthScripts() {
		foreach (Health script in GameObject.Find("Players").GetComponentsInChildren<Health>()) {
			healthList.Add (script);
		}
	}

	/// <summary>
	/// Gets the health bar for each player.
	/// </summary>
	void GetHealthBars() {
		Image[] healthBars = UI.GetComponentsInChildren<Image> ();
		foreach (Image item in healthBars) {
			if (item.tag == "HealthBar")
				healthBarList.Add (item);
		}
	}

	public void UpdateHealthBars() {
		for (int i = 0; i < healthBarList.Count; i++) {
			healthBarList [i].fillAmount = (healthList [i].health * .01f);
		}
	}

	void GetCrosshair() {
		Image[] temp = UI.GetComponentsInChildren<Image> ();
		foreach (Image item in temp) {
			if (item.name == "Crosshair")
				crosshair = item;
		}
	}

	void GetPauseMenu(){
		Text[] texts = UI.GetComponentsInChildren<Text> ();
		foreach (Text text in texts) {
			if (text.name == "PauseMenu")
				pauseMenu = text;
		}
	}

	public void ResetCrosshair(){
		crosshair.rectTransform.anchorMax = crosshairDefault;
		crosshair.rectTransform.anchorMin = crosshairDefault;
	}

	public void ShowGBar(){
		GbarPrefab.enabled = true;
		Gbar.enabled = true;
		Gbar.fillAmount = 0;
		fixingGenerator = true;
	}

	void GetGBar(){
		Image[] temp = UI.GetComponentsInChildren<Image> ();
		foreach (Image item in temp) {
			if (item.name == "ProgressBar")
				Gbar = item;
			if (item.name == "ProgressBarFill")
				GbarPrefab = item;
		}
	}

	void UpdateGbar(){
		
		timer += Time.deltaTime;

		Gbar.fillAmount = (timer / 5.0f);

		if (Gbar.fillAmount == 1.0f) {
			fixingGenerator = false;
			GbarPrefab.enabled = false;
			Gbar.enabled = false;
			gScript.isFixed = true;
			gScript.currentlyFixing = false;
		}
	}
}
