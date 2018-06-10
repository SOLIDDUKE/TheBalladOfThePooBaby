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
    public float timeToWallUnstick;
    //----------------------------------------------------------------------------------------------------------------------------------------

    // Update is called once per frame
    public override void Execute ()
    {
        base.Execute();

        HandleWallSliding();
    }

    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;//If player facing left var = -1 else +1.
        wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {//If these variables are met the conditions are right for wall sliding.
            wallSliding = true;

            if (velocity.y < -wallSlideSpeedMax)
            {
                //constrain downwards speed to wall slide speed limit.
                velocity.y = -wallSlideSpeedMax;
            }//if

            if (timeToWallUnstick > 0)
            {
                velocityXSmoothing = 0;
                velocity.x = 0;

                if (directionalInput.x != wallDirX && directionalInput.x != 0)
                {//only do this if moving in opposite direction of wall.
                    timeToWallUnstick -= Time.deltaTime;
                }//if
                else timeToWallUnstick = wallStickTime;
            }//if
            else timeToWallUnstick = wallStickTime;
        }
    }

    override public void OnJumpInputDown()
    {
        base.OnJumpInputDown();

        if (wallSliding)
        {//If player is wallsiding.
            if (wallDirX == directionalInput.x)
            {//if trying to move in same directino as facing wall jump up wall.
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;

            }//if
            else if (directionalInput.x == 0)
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
    }
}
