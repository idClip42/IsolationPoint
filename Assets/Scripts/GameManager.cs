using System.Collections;
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
    public Event[] scriptedEvents;      //match the event to occur with the completion of the corresponding objective, leave null if no event wanted
    public int currentObjective;

    SunSetting sunset;
    Text objText;

    //bool night;
    //public bool Night { get { return night; } }
    public bool gameover;

    bool isPlayingEvent = false;
    public bool IsPlayingEvent
    {
        get { return isPlayingEvent; }
        set { isPlayingEvent = true; }
    }

    bool isFadingText = true;
    public float textFadeTime = 1.0f;

	// Use this for initialization
	void Start () {
        Random.InitState((int)Time.time);
        InitializeVariables();
	}

    void InitializeVariables()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers = players.Length;
        //night = false;
        gameover = false;

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
        if (objectives.Length > 0) DisplayObjective();
    }
	
	// Update is called once per frame
	void Update () {

        if (gameover)
        {
            //end game screen
            return;
        }

        //fade in the objective text
        if (!isPlayingEvent && isFadingText)
        {
            if (objText.canvasRenderer.GetAlpha() <= 0.011f)
            {
                DisplayObjective();
            }
        }

        /*
        //night starts --> spawn or warp enemies --> warp for now
        if(!night && sunset.Night)
        {
            foreach(GameObject enemy in enemies)
            {
                //warp enemy to start location --> held off level in the meantime
                enemy.GetComponent<NavMeshAgent>().Warp(enemyStart[Random.Range(0, enemyStart.Length - 1)].position);
            }
            night = true;
        }
        */
	}

    /// <summary>
    /// Show completed objective and get new objective. No new objectives = win.
    /// </summary>
    public void NextObjective()
    {
        if(currentObjective+1 < objectives.Length)
        {
            objectives[currentObjective].enabled = false;
            currentObjective++;
            objectives[currentObjective].enabled = true;
            foreach(Objective sub in objectives[currentObjective].subObjectives)
            {
                sub.enabled = true;
            }
            return;
        }
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
        objText.CrossFadeAlpha(1.0f, textFadeTime, false);
		if(objectives.Length == 0) return;
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

        //play event
        Event e = scriptedEvents[currentObjective];
        if (e != null)
        {
            e.PlayEvent();
        }
        NextObjective();
    }
}
