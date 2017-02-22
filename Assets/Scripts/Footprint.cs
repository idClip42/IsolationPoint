using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footprint : MonoBehaviour {

    Animator anim;  //Required for placing footesteps beneath feet
    public AudioClip[] snowSteps;
    public Material snowL;
    public Material snowR;
    public GameObject snowFootprintPrefab;
    public AudioClip[] woodSteps;
    public AudioClip[] crunchSteps;
    public float lifetime = 20;

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
            AudioSource audSou = foot.GetComponent<AudioSource>();
            switch (hit.transform.tag)
            {
                case "Snow":
                    //play snow sound
                    audSou.clip = snowSteps[Random.Range(0, snowSteps.Length - 1)];
                    audSou.Play();

                    //create object for footprint
                    GameObject footprint = Instantiate(snowFootprintPrefab);
                    footprint.transform.position = hit.point + new Vector3(0, 0.01f, 0);
                    //footprint.transform.Rotate(0, foot.rotation.eulerAngles.y + 180, 0);
                    footprint.transform.rotation = Quaternion.FromToRotation(footprint.transform.forward, -transform.forward) * footprint.transform.rotation;
                    if (foot.name == "Toe_L")
                    {
                        footprint.GetComponent<Renderer>().material = snowL;
                    }
                    else
                    {
                        //footprint.transform.Rotate(0, 180, 0);
                        footprint.GetComponent<Renderer>().material = snowR;
                    }
                    footprint.transform.rotation = Quaternion.FromToRotation(footprint.transform.up, hit.normal) * footprint.transform.rotation;
                    Destroy(footprint, lifetime);
                    break;
                case "Wood":
                    //play wood floor sound
                    audSou.clip = woodSteps[Random.Range(0, woodSteps.Length - 1)];
                    audSou.Play();
                    break;
                case "Crunch":
                    //play crunch sound - ie. on glass or gravel
                    audSou.clip = crunchSteps[Random.Range(0, crunchSteps.Length - 1)];
                    audSou.Play();
                    break;
                default:
                    //nothing...
                    break;
            }
        }
    }
}
