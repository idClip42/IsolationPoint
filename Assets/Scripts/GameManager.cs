using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour {

    public GameObject[] players;        //list of players -> access positions and alive status
    public int activePlayers;           //score at the end
    public GameObject[] locations;      //lists of locations the enemy can wander to -> nodes for AI
                                        //2nd layer that holds nodes within a location to wander between
    public Transform[] enemyStart;
    GameObject[] enemies;

    public GameObject[] objectives;     //list of objectives, in order, for the player to complete
    public int currentObjective;

    SunSetting sunset;

    bool night;
    public bool Night { get { return night; } }
    public bool gameover;

	// Use this for initialization
	void Start () {
        Random.InitState((int)Time.time);
        InitializeVariables();
	}

    void InitializeVariables()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        activePlayers = players.Length;
        night = false;
        gameover = false;

        sunset = GameObject.Find("Sun").GetComponent<SunSetting>();
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }
	
	// Update is called once per frame
	void Update () {
        if (gameover)
        {
            //end game screen
            return;
        }

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
	}

    /// <summary>
    /// Show completed objective and get new objective. No new objectives = win.
    /// </summary>
    public void NextObjective()
    {
        if(currentObjective < objectives.Length)
        {
            currentObjective++;
            //scratch out prev objective
            //display new objective
            return;
        }
        gameover = true;
    }
}
