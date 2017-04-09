﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject[] players;        //list of players -> access positions and alive status
    public int activePlayers;           //score at the end
    public GameObject[] locations;      //lists of locations the enemy can wander to -> nodes for AI
                                        //2nd layer that holds nodes within a location to wander between
    public Transform[] enemyStart;
    public GameObject[] enemies;

    public Objective[] objectives;     //list of objectives, in order, for the player to complete
    int currentObjective;

    //Restricts input when true. Used for events where the player should not be able to interact with anything.
    bool pauseInput;
    public bool PauseInput
    {
        get { return pauseInput; }
        set { pauseInput = value; }
    }

    //SunSetting sunset;
    Text objText;

    public bool gameover;

    bool isPlayingEvent = false;
    public bool IsPlayingEvent
    {
        get { return isPlayingEvent; }
        set { isPlayingEvent = value; }
    }

    bool isFadingText = true;
    public float textFadeTime = 1.0f;

	// Use this for initialization
	void Start () {
        Random.InitState((int)Time.time);
        InitializeVariables();
        if (objectives.Length > 0) DisplayObjective();
    }

    void InitializeVariables()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers = players.Length;
        //night = false;
        gameover = false;
        pauseInput = false;

        //sunset = GameObject.Find("Sun").GetComponent<SunSetting>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        objText = GameObject.Find("Objective Text").GetComponent<Text>();
        //set to transparent
        objText.canvasRenderer.SetAlpha(0.01f);


        //disable objectives and enable the first
        for(int i = 0; i < objectives.Length; i++)
        {
            objectives[i].enabled = false;

            foreach (Objective sub in objectives[i].subObjectives)
            {
                sub.parent = objectives[i];
                sub.enabled = false;
            }

            if (i == 0) {
                objectives[i].enabled = true;
                foreach (Objective sub in objectives[i].subObjectives)
                {
                    sub.enabled = true;
                }
            }
        }
        currentObjective = 0;
    }
	
	// Update is called once per frame
	void Update () {

        if (gameover)
        {
            //end game screen
            return;
        }

        //fade in the objective text
        if (!IsPlayingEvent && isFadingText)
        {
            if (objText.canvasRenderer.GetAlpha() <= 0.02f)
            {
                NextObjective();
                DisplayObjective();
            }
        }
	}

    /// <summary>
    /// Show completed objective and get new objective. No new objectives = win.
    /// </summary>
    public void NextObjective()
    {
        if(currentObjective+1 < objectives.Length)
        {            
            //enable next set of objectives
            currentObjective++;
            objectives[currentObjective].enabled = true;
            foreach(Objective sub in objectives[currentObjective].subObjectives)
            {
                sub.enabled = true;
            }
            return;
        }

        //disable the current obj and end game
        objectives[currentObjective].enabled = false;
        gameover = true;
    }

    /// <summary>
    /// Display the text of the current objective.
    /// </summary>
    void DisplayObjective()
    {
        //fade in new text
        isFadingText = false;
		if(objectives.Length == 0 || gameover) return;
        objText.CrossFadeAlpha(1.0f, textFadeTime, false);
        objText.text = objectives[currentObjective].UIText;
    }

    /// <summary>
    /// Plays the event associated with the completed objective. Objective should increment after the event completes.
    /// </summary>
    public void PlayEvent()
    {
        //fade out prev objective
        isFadingText = true;
        objText.CrossFadeAlpha(0.01f, textFadeTime, false);
    }
}
