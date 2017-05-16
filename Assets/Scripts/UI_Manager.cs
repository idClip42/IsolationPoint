using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Manager : MonoBehaviour {

    public static UI_Manager UIManager;

	GameObject UI;
	Generator gScript;
	Radio rScript;

    ScreenFadeEvent fadeEvent;
    public ScreenFadeEvent FadeEvent
    {
        get { return fadeEvent; }
    }

	public Sprite redDot;
	public Sprite redCross;
	public Sprite greenCircle;
	public Sprite redCircle;

	List<Health> healthList;
	List<Image> healthBarList;

	public Image crosshair;
	public Image bar;
	Image barPrefab;

	public List<Image> portraits;

	public Image pauseMenu;
	public Button controls;
	public Button quit;
	public Button back;
	public Sprite skull;

	public Text controlsMenu;

	Vector2 crosshairDefault;

	public bool pauseGame = false;
	public bool fixingGenerator;
	public bool fixingRadio;

	float gTimer;
	float pTimer;
    float fTimer;

	float generatorFixTime;
	float radioFixTime;

	int playerIndex;

	void Start () {
        UIManager = this;
		playerIndex = GameObject.Find ("PlayerController").GetComponent<PlayerController>().playerNum;
		gScript = GameObject.Find ("Generator").GetComponent<Generator> ();
		rScript = GameObject.Find ("Radio").GetComponent<Radio> ();
        fadeEvent = GetComponentInChildren<ScreenFadeEvent>();
		crosshairDefault = new Vector2 (0.5f, 0.5f);
		healthList = new List<Health> ();
		healthBarList = new List<Image> ();
		pauseGame = false;
		fixingGenerator = false;
		fixingRadio = false;
		gTimer = 0;
		UI = GameObject.Find ("UI");
		//if(UI == null) UI = GameObject.Find("UI 1");
		//if(UI == null) Debug.Log("Didn't find UI object");
		GetCrosshair ();
		GetGBar ();
		GetHealthScripts ();
		GetHealthBars ();
		GetPauseMenu ();

		barPrefab.enabled = false;
		bar.enabled = false;

		generatorFixTime = 5.0f;
     	radioFixTime = 5.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.P) || Input.GetButtonDown("Pause")) {
			pauseGame = !pauseGame;

			if (pauseGame) {
				EnablePauseMenu ();
			} else {
				DisablePauseMenu ();
			}
		}

		if (Input.GetButtonDown("AimWeapon")) {
			crosshair.sprite = redCross;
		}

		if (Input.GetButtonUp("AimWeapon")) {
			crosshair.sprite = redDot;
		}
			
		if (fixingGenerator) {
			UpdateGbar ();
		}
		if (fixingRadio) {
			UpdateRBar ();
		}

		UpdateCurrentPlayer ();
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
		int deathCount = 0;

		for (int i = 0; i < healthBarList.Count; i++) {
			healthBarList [i].fillAmount = (healthList [i].health * .01f);
			if ((healthBarList [i].fillAmount) == 0) {
				portraits [i].color = Color.grey;
				deathCount++;
			}
		}

		if (deathCount == 4) {
			SceneManager.LoadScene ("EndGameMenu");
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
		Image[] images = UI.GetComponentsInChildren<Image> ();
		foreach (Image image in images) {
			if (image.name == "PauseMenu") {
				pauseMenu = image;
			}
		}
	}

	public void ResetCrosshair(){
		crosshair.rectTransform.anchorMax = crosshairDefault;
		crosshair.rectTransform.anchorMin = crosshairDefault;
	}

	public void ResetBar (){
		barPrefab.enabled = true;
		bar.enabled = true;
		bar.fillAmount = 0;
	}

	public void StartGeneratorFix(){
		ResetBar ();
		fixingGenerator = true;

		if(PlayerController.controller.Player.GetComponent<Engineer>() != null)
			generatorFixTime /= 2;
	}

	public void StartRadioFix(){
		ResetBar ();
		fixingRadio = true;
		rScript.isFixing = true;

		if(PlayerController.controller.Player.GetComponent<Engineer>() != null)
			radioFixTime /= 2;
	}

	void GetGBar(){
		Image[] temp = UI.GetComponentsInChildren<Image> ();
		foreach (Image item in temp) {
			if (item.name == "ProgressBar")
				bar = item;
			if (item.name == "ProgressBarFill")
				barPrefab = item;
		}
	}

	void UpdateGbar(){
		
		gTimer += Time.deltaTime;

		//bar.fillAmount = (gTimer / 5.0f);
		bar.fillAmount = (gTimer / generatorFixTime);

		if (bar.fillAmount == 1.0f) {
			fixingGenerator = false;
			barPrefab.enabled = false;
			bar.enabled = false;
			gScript.IsFixed = true;
			gScript.currentlyFixing = false;
		}

   /*     if (!PlayerController.controller.FollowScript.IsWorking)
        {
            barPrefab.enabled = false;
            bar.enabled = false;
        }
        else
        {
            barPrefab.enabled = true;
            bar.enabled = true;
        } */
    }

	void UpdateRBar(){

		pTimer += Time.deltaTime;

		//bar.fillAmount = (pTimer / 5.0f);
		bar.fillAmount = (pTimer / radioFixTime);

		if (bar.fillAmount == 1.0f) {
			fixingRadio = false;
			barPrefab.enabled = false;
			bar.enabled = false;
			rScript.isFixing = false;
			rScript.isFixed = true;
            rScript.onFixObj.IsCompleted = true;
            return;
		}

     /*   if (!PlayerController.controller.FollowScript.IsWorking)
        {
            barPrefab.enabled = false;
            bar.enabled = false;
        }
        else
        {
            barPrefab.enabled = true;
            bar.enabled = true;
        } */
    }

    /// <summary>
    /// Call from other scripts that have their own timers. Should allow for this one function to be used for all bar related timers.
    /// </summary>
    /// <param name="fill">How full the bar is</param>
    public void UpdateBar(float fill, Follower worker)
    {
		bar.fillAmount = fill;

        if (bar.fillAmount == 1.0f)
        {
            barPrefab.enabled = false;
            bar.enabled = false;
            return;
        }

        //Only show the bar if the currect character is working on something (doing the thing that needs the bar)
        if (PlayerController.controller.FollowScript != worker)
        {
            barPrefab.enabled = false;
            bar.enabled = false;
        }
        else
        {
            barPrefab.enabled = true;
            bar.enabled = true;
        }
    }

	void UpdateCurrentPlayer(){
		// Gets the current player index
		playerIndex = GameObject.Find ("PlayerController").GetComponent<PlayerController>().playerNum;

		// Makes all UI circles red
		for (int  i = 0; i < 4; i++){
			healthBarList [i].sprite = redCircle;
			healthBarList [i].color = Color.red;
		}

		// Updates the current UI to a green circle health bar
		healthBarList [playerIndex].sprite = greenCircle;
		healthBarList [playerIndex].color = Color.green;
	}

	void EnablePauseMenu(){
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
		pauseMenu.enabled = true;
		controls.enabled = true;
		controls.GetComponentInChildren<Text> ().enabled = true;
		quit.enabled = true;
		quit.GetComponentInChildren<Text> ().enabled = true;
		Time.timeScale = 0.0f;
	}

	void DisablePauseMenu(){
		Cursor.visible = false;
		Cursor.lockState = CursorLockMode.Locked;
		pauseMenu.enabled = false;
		controls.enabled = false;
		quit.enabled = false;
		quit.GetComponentInChildren<Text> ().enabled = false;
		controls.GetComponentInChildren<Text> ().enabled = false;
		Time.timeScale = 1.0f;
	}

	public void ShowControls(){
		controls.GetComponentInChildren<Text> ().enabled = false;
		controlsMenu.enabled = true;
		controls.enabled = false;
		quit.enabled = false;
		quit.GetComponentInChildren<Text>().enabled = false;
		back.enabled = true;
		back.GetComponentInChildren<Text>().enabled = true;
	}

	public void HideControls(){
		controls.GetComponentInChildren<Text> ().enabled = true;
		controls.enabled = true;
		controlsMenu.enabled = false;
		quit.enabled = true;
		quit.GetComponentInChildren<Text>().enabled = true;
		back.enabled = false;
		back.GetComponentInChildren<Text>().enabled = false;
	}
}
