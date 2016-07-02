﻿using UnityEngine;
using System.Collections;

public class Spike : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag == "Player")
        {
            collision.gameObject.GetComponent<Mario>().GotDirectHit();
        }
    }
}