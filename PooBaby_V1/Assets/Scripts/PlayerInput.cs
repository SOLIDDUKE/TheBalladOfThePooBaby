using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerInput : MonoBehaviour
{
    Player player;
    MovementStateMachine playerMovementMachine;


    void Start()
    {
        player = GetComponent<Player>();
        playerMovementMachine = player.movementMachine;
    }//Start

    void Update()
    {
        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        playerMovementMachine.CurrentForm.SetDirectionalInput(directionalInput);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            playerMovementMachine.CurrentForm.OnJumpInputDown();
        }//if
        if (Input.GetKeyUp(KeyCode.Space))
        {
            playerMovementMachine.CurrentForm.OnJumpInputUp();
        }//if

    }//Update
}