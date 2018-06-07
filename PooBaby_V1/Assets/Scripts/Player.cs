using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    Controller2D controller;

	void Start ()
    {
        //----------Referances------------
        controller = GetComponent<Controller2D>();

        //--------------------------------
	}//Start
	
	void Update ()
    {
		
	}//Update

}//Player Class
