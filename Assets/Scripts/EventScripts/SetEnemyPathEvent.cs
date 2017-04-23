using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetEnemyPathEvent : Event {

    public bool autoSwitchPaths;
    public string pathName;
    public int pathIndex = -1;
    public Enemy[] enemies;


	// Use this for initialization
	protected override void Start () {
        base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
        base.Update();
	}

    public override void PlayEvent()
    {
        base.PlayEvent();
        Debug.Log("Name: " + pathName);
        if(pathName != null && pathName != " " && pathName != "")
        {
            foreach(Enemy e in enemies)
            {
                e.SetPath(pathName);
                e.autoChangePath = autoSwitchPaths;
            }
        }else if(pathIndex >= 0)
        {
            foreach (Enemy e in enemies)
            {
                e.SetPath(pathIndex);
                e.autoChangePath = autoSwitchPaths;
            }
        }
    }
}
