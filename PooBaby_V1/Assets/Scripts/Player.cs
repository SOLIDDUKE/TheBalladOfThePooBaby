using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    [Tooltip("Players Jump Hight.")]
    public float jumpHeight = 4;
    [Tooltip("Time to reach top of jump.")]
    public float timeToJumpApex = .4f; //How long it will take the charecter to reach highest point of jump in seconds.
    float accelerationTimeAirbourn = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed= 6;

    float gravity;
    float jumpVelocity;
    float velocityXSmoothing;
    Vector3 velocity;


    Controller2D controller;

	void Start ()
    {
        //----------Referances------------
        controller = GetComponent<Controller2D>();
        //--------------------------------

        gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); //Calculate gravity
        jumpVelocity = Mathf.Abs(gravity) * timeToJumpApex; //Calculate jump velocity.
        //print("Gravity: " + gravity + "   Jump Velocity: " + jumpVelocity);
	}//Start
	
	void Update ()
    {
        if (controller.collisions.above || controller.collisions.below)
        {//This is to combat the probem of acumulating gravity.
            velocity.y = 0;
        }//if

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (Input.GetKeyDown(KeyCode.Space)&& controller.collisions.below == true)
        {//If the player hits space and is on something jump.
            velocity.y = jumpVelocity;
        }//if

         float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below)?accelerationTimeGrounded:accelerationTimeAirbourn);
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
	}//Update

}//Player Class
