using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundBubble : MonoBehaviour {

    AudioSource src;
    public float maxSize = 10;
    float maxScale;
    GameManager gm;

	// Use this for initialization
	void Start () {
        src = GetComponent<AudioSource>();
        GetMaxScale();
        gm = GameObject.Find("GM").GetComponent<GameManager>();
    }
	
	// Update is called once per frame
	void Update () {
        if (src.isPlaying)
        {
            float size = maxSize;
            for(int i = 0; i < gm.enemies.Length; i++)
            {
                if((gm.enemies[i].transform.position - transform.position).sqrMagnitude <= Mathf.Pow(maxSize, 2))
                {
                    gm.enemies[i].GetComponent<Enemy>().SetTarget(transform.position);
                }
            }
        }
	}

    /// <summary>
    /// Get the total scale when considering all the parent transforms as well.
    /// </summary>
    void GetMaxScale()
    {
        maxScale = Mathf.Max(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        Transform p = transform.parent;
        while (p)
        {
            float max = Mathf.Max(p.localScale.x, p.localScale.y, p.localScale.z);
            maxScale *= max;
            p = p.parent;
        }
    }
}
