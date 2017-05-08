using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class MainMenu : MonoBehaviour {

	public Text[] menuText;

	public Text controlsMenu;
	public Text controls;
	public Text play;
	public Text quit;
	public Text instr;
	public Text instrMenu;
	public Text back;
	 
	public Button controlButton;
	public Button backButton;
	public Button instrButton;

	// Use this for initialization
	void Start () {
		controlButton.onClick.AddListener (ShowControls);
		backButton.onClick.AddListener (HideControls);
		backButton.onClick.AddListener (HideInstr);
		instrButton.onClick.AddListener (ShowInstr);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowInstr(){
		instrMenu.enabled = true;
		backButton.enabled = true;
		back.enabled = true;
		HideButtonText ();
	}

	public void HideInstr(){
		instrMenu.enabled = false;
		backButton.enabled = false;
		back.enabled = false;
		RevealButtonText ();
	}

	public void ShowControls(){
		controlsMenu.enabled = true;
		backButton.enabled = true;
		back.enabled = true;
		HideButtonText ();
	}

	public void HideControls(){
		controlsMenu.enabled = false;
		backButton.enabled = false;
		back.enabled = false;
		RevealButtonText ();
	}

	void HideButtonText(){
		controls.enabled = false;
		play.enabled = false;
		quit.enabled = false;
		instr.enabled = false;
	}

	void RevealButtonText(){
		controls.enabled = true;
		play.enabled = true;
		quit.enabled = true;
		instr.enabled = true;
	}
}
