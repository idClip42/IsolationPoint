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

    public GameObject[] warps;          //warp locations
    public Transform[] enemyStart;
    public GameObject[] enemies;

    public Objective[] objectives;     //list of objectives, in order, for the player to complete
    public int currentObjective;

    //Restricts input when true. Used for events where the player should not be able to interact with anything.
    bool pauseInput;
    public bool PauseInput
    {
        get { return pauseInput; }
        set { pauseInput = value; }
    }

    bool freezeCam;
    public bool FreezeCam
    {
        get { return freezeCam; }
        set { freezeCam = value; }
    }

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
        freezeCam = false;

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

    /// <summary>
    /// Find the nearest warp location.
    /// </summary>
    /// <param name="location">Location to find close to.</param>
    /// <returns>The transform of the nearest warp spot.</returns>
    Transform NearestWarp(Vector3 location)
    {
        Transform nearest = null;
        float dist = 1000;
        foreach(GameObject g in warps)
        {
            if (nearest == null)
            {
                nearest = g.transform;
                dist = Vector3.Distance(nearest.position, location);
            }else
            {
                float temp = Vector3.Distance(g.transform.position, location);
                if(temp < dist)
                {
                    nearest = g.transform;
                    dist = temp;
                }
            }
        }
        return nearest;
    }

    /// <summary>
    /// Find the nearest path point.
    /// </summary>
    /// <param name="location">Point to find the closest path point from.</param>
    /// <returns>Transform of the nearest path point.</returns>
    Transform NearestPathPoint(Vector3 location)
    {
        Transform nearest = null;
        float dist = 1000;
        foreach(GameObject list in locations)
        {
            LocationList locList = list.GetComponent<LocationList>();
            foreach (Transform l in locList.locations)
            {
                if(nearest == null)
                {
                    nearest = l;
                    dist = Vector3.Distance(nearest.position, location);
                }else
                {
                    float temp = Vector3.Distance(l.position, location);
                    if(temp < dist)
                    {
                        nearest = l;
                        dist = temp;
                    }
                }
            }
        }

        return nearest;
    }

    /// <summary>
    /// Get the first path index that includes the given transform.
    /// </summary>
    /// <param name="point">Path point to find in a path.</param>
    /// <returns>The index of the path with the point.</returns>
    int NearestPath(Transform point)
    {
        for(int i = 0; i < locations.Length; i++)
        {
            LocationList locList = locations[i].GetComponent<LocationList>();
            foreach(Transform l in locList.locations)
            {
                if (l == point) return i;
            }
        }
        //this should hopefully never happen
        return 0;
    }
}
