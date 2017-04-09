using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event : MonoBehaviour {
    protected GameManager gm;
    public float timeToComplete = 10.0f;

    bool isPlaying = false;
    public bool IsPlaying
    {
        get { return isPlaying; }
    }

    bool isFinished = false;
    public bool IsFinished
    {
        get { return isFinished; }
        set { isFinished = value; }
    }

	// Use this for initialization
	protected virtual void Start () {
        gm = GameObject.Find("GM").GetComponent<GameManager>();
    }

    // Update is called once per frame
    protected virtual void Update () {
        if (!IsPlaying) return;

        timeToComplete -= Time.deltaTime;
        if (timeToComplete <= 0)
        {
            isFinished = true;
            isPlaying = false;
        }
	}

    /// <summary>
    /// Start the event.
    /// </summary>
    public virtual void PlayEvent()
    {
        isPlaying = true;
    }
}
