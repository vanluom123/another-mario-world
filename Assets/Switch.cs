﻿using UnityEngine;
using System.Collections;
using Assets;

public class Switch : MonoBehaviour {

    public GameObject[] _listObject;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        string tag = collider.gameObject.tag;
        if (tag == "Player")
        {

            foreach (var obj in _listObject)
            {
                if (obj.name.Contains("hide_gate"))
                    if (collider.gameObject.GetComponent<Animator>().GetInteger("status") == 1)
                        continue;
                obj.GetComponent < ISwitchable>()._switch_on();
            }
        }
    }
}
