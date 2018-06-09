using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public StateMachine stateMachine;
    public Controller2D controller;

    public float jumpHeight = 4;            
    public float timeToJumpApex = .4f;      
    public float gravity;
    public float jumpVelocity;

	void Start ()
    {
        //Get references
        controller = GetComponent<Controller2D>();

        //Set gravity
        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); //Calculate gravity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; //Calculate jump velocity.

        //Set up State Machine
        stateMachine = new StateMachine();
        stateMachine.ChangeState(new LiquidState(), this);
	}

    private void Update()
    {
        stateMachine.Execute();
    }
}
