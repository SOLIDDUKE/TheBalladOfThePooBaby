using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    //---------------Wall Climb Settings----------------------------------------------------------------------------------------------------
    [Header("WallJump Actions Settings")]
    [Tooltip("The ammount the player will jump up and across when preforming any of these actions.")]
    public Vector2 wallJumpClimb = new Vector2(x:7.5f,y:16);        
    public Vector2 wallJumpOff = new Vector2(x: 8.5f, y: 7);
    public Vector2 wallLeap = new Vector2(x: 18f, y: 17);

    [Header("WallClimb Settings")]
    public float wallSlideSpeedMax = 3;     //The max speed the player can build up sliding down a wall.
    [Tooltip("After the player tries to move away from the wall they will be stuck for this ammount of time. Helps with wall jumping.")]
    public float wallStickTime = .25f;      //The time the player will stick to the wall for this ammount of time after trying to move in opposite direction, this make wall jumping easier.
    public bool enableWallClimbing;         //Toggle the ability to wall climb on and off
    //----------------------------------------------------------------------------------------------------------------------------------------

    
    //--------------Player Settings------------------------------------------------------------------------------------------------------------
    [Header("Player Settings")]
    [Tooltip("Players Jump Hight.")]
    public float maxJumpHeight = 4;             //Max height the player will jump.
    public float minJumpHeight = 1;             //Min hight the player will jump.
    [Tooltip("Time to reach top of jump.")]
    public float timeToJumpApex = .4f;          //How long it will take the charecter to reach highest point of jump in seconds.
    [Tooltip("This will allow the player to pass through specific platfroms tagged with passthrough.")]
    public bool allowPassThrough;               //This will allow the player to pass through specific platfroms tagged with passthrough. This ability can be used for gas form. Press down to drop through platfroms or jump under them.
    //-----------------------------------------------------------------------------------------------------------------------------------------

    float timeToWallUnctick;
    float accelerationTimeAirbourn = .2f;
    float accelerationTimeGrounded = .1f;
    float moveSpeed= 6;

    float gravity;
    float maxJumpVelocity;
    float minJumpVelocity;
    float velocityXSmoothing;

    Vector3 velocity;
    Controller2D controller;

	void Start ()
    {
        //----------Referances------------
        controller = GetComponent<Controller2D>();
        //--------------------------------

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);          //Calculate gravity.
        controller.allowPassThrough = allowPassThrough;                         //Passthough in controler is set to whatever is set in this script.
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;                  //Calculate max jump velocity.
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);   //Calculate min jump velocity.
        //print("Gravity: " + gravity + "   Jump Velocity: " + jumpVelocity);
	}//Start
	
	void Update ()
    {
        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        int wallDirX = (controller.collisions.left) ? -1 : 1;//If player facing left var = -1 else +1.

        float targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirbourn);

        bool wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right)&& !controller.collisions.below && velocity.y < 0 && enableWallClimbing)
        {//If these variables are met the conditions are right for wall sliding.
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                //constrain downwards speed to wall slide speed limit.
                velocity.y = -wallSlideSpeedMax;
            }//if

            if (timeToWallUnctick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (input.x != wallDirX && input.x != 0)
                {//only do this if moving in opposite direction of wall.
                    timeToWallUnctick -= Time.deltaTime;
                }//if
                else timeToWallUnctick = wallStickTime; 
            }//if
            else timeToWallUnctick = wallStickTime;


        }//if

        if (controller.collisions.above || controller.collisions.below)
        {//This is to combat the probem of acumulating gravity.
            velocity.y = 0;
        }//if

        if (Input.GetKeyDown(KeyCode.Space))
        {//If the player hits space.

            if (wallSliding)
            {//If player is wallsiding.
                if (wallDirX == input.x)
                {//if trying to move in same directino as facing wall jump up wall.
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;

                }//if
                else if (input.x ==0)
                {//if just jumping off the wall
                    velocity.x = -wallDirX * wallJumpOff.x;
                    velocity.y = wallJumpOff.y;
                }//else if
                else
                {//if input opposite to wall direcrtion prefrom a wall leap.
                    velocity.x = -wallDirX * wallLeap.x;
                    velocity.y = wallLeap.y;
                }//else
            }//if

            if (controller.collisions.below)
            {//If player standing on something.
                velocity.y = maxJumpVelocity;
            }//if
            
        }//if

        if (Input.GetKeyUp(KeyCode.Space))
        {//if the player releases the space bar.
            if (velocity.y > minJumpVelocity)
            {
                velocity.y = minJumpVelocity;
            }//if
        }//if

        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime, input);

        if (controller.collisions.above || controller.collisions.below)
        {//This is to combat the probem of acumulating gravity.
            velocity.y = 0;
        }//if
    }//Update

}//Player Class
