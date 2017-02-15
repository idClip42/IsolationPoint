using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour {

    Animator anim;  //Required for placing footesteps beneath feet

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Footstep(string s)
    {
        Transform foot = transform;
        //find correct foot transform
        switch (s)
        {
            case "right":
                foreach (Transform child in anim.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "Toe_R")
                    {
                        foot = child;
                    }
                }
                break;
            case "left":
                foreach (Transform child in anim.GetComponentsInChildren<Transform>())
                {
                    if (child.name == "Toe_L")
                    {
                        foot = child;
                    }
                }
                break;
            default: Debug.Log("Invalid footprint string");
                break;
        }

        //raycast down to find material of floor
        RaycastHit hit;
        Physics.Raycast(foot.position, -transform.up, out hit, 1);
        if (hit.collider != null)
        {
            switch (hit.transform.tag)
            {
                case "Snow":
                    //play snow sound
                    /*
                    //location in world space of print
                    Vector3 printLoc = hit.point + new Vector3(0, 0.01f, 0);
                    Transform print = foot;
                    print.position = printLoc;
                    print.localScale = new Vector3(0.09f, 0.09f, 0.09f);//for plane -- might change

                    //create object for footprint -- make prefab with timer to delete?
                    GameObject footprint = new GameObject("footprint");
                    footprint.transform.position = print.position;
                    footprint.transform.rotation = print.rotation;
                    footprint.transform.localScale = print.localScale;
                    */
                    break;
                case "Wood":
                    //play wood thump sound
                    break;
                case "Crunch":
                    //play crunch sound - ie. on glass or gravel
                    break;
                default:
                    //nothing...
                    Vector3 printLoc = hit.point + new Vector3(0, 0.01f, 0);
                    Transform print = foot;
                    print.position = printLoc;
                    print.localScale = new Vector3(0.09f, 0.09f, 0.09f);

                    //create object for footprint -- make prefab with timer to delete?
                    GameObject footprint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                    footprint.transform.position = print.position;
                    footprint.transform.rotation = print.rotation;
                    footprint.transform.localScale = print.localScale;
                    break;
            }
        }
    }
}
