using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof (Player))]
public class PlayerInput : MonoBehaviour {
    Player player;


	void Start ()
    {
        player = GetComponent<Player>();
	}//Start
	
	void Update ()
    {
		Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        player.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.OnJumpInputDown();
        }//if
        if (Input.GetKeyUp(KeyCode.Space))
        {
            player.OnJumpInputUp();
        }//if

    }//Update
}
