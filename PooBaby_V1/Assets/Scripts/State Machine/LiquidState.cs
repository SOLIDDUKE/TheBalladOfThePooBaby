using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LiquidState : State {

    //---------------Wall Climb Settings----------------------------------------------------------------------------------------------------
    [Header("WallJump Actions Settings")]
    [Tooltip("The amount the player will jump up and across when preforming any of these actions.")]
    public Vector2 wallJumpClimb = new Vector2(x: 7.5f, y: 16);
    public Vector2 wallJumpOff = new Vector2(x: 8.5f, y: 7);
    public Vector2 wallLeap = new Vector2(x: 18f, y: 17);

    [Header("WallClimb Settings")]
    public float wallSlideSpeedMax = 3;     //The max speed the player can build up sliding down a wall.
    [Tooltip("After the player tries to move away from the wall they will be stuck for this ammount of time. Helps with wall jumping.")]
    public float wallStickTime = .25f;      //The time the player will stick to the wall for this ammount of time after trying to move in opposite direction, this make wall jumping easier.
    public bool enableWallClimbing;         //Toggle the ability to wall climb on and off
    //----------------------------------------------------------------------------------------------------------------------------------------
	
	// Update is called once per frame
	public override void Execute () {
        base.Execute();

        bool wallSliding = false;

        if ((player.controller.collisions.left || player.controller.collisions.right) 
            && !player.controller.collisions.below && velocity.y < 0)
        {
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                //constrain downwards speed to wall slide speed limit.
                velocity.y = -wallSlideSpeedMax;
            }

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                //only do this if moving in opposite direction of wall.
                if (input.x != wallDirX && input.x != 0)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else timeToWallUnstick = wallStickTime;
            }
            else timeToWallUnstick = wallStickTime;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (wallSliding)
            {//If player is wallsiding.
                if (wallDirX == input.x)
                {//if trying to move in same directino as facing wall jump up wall.
                    velocity.x = -wallDirX * wallJumpClimb.x;
                    velocity.y = wallJumpClimb.y;

                }//if
                else if (input.x == 0)
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
        }//if
    }
}
