/*using System.Collections;
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
        playerMovementMachine.ChangeState(new SolidState(), player);
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

        // STATE SWITCHING <<<<<

        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Entering gas state");
            playerMovementMachine.ChangeState(new GasState(), player);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Entering liquid state");
            playerMovementMachine.ChangeState(new LiquidState(), player);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Entering solid state");
            playerMovementMachine.ChangeState(new SolidState(), player);
        }

        // STATE SWITCHING >>>>>

    }//Update

}*/