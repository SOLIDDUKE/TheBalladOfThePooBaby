using UnityEngine;

public class LiquidState : State {

    //------------Wall Movment Velocitys-----------------------------
    public Vector2 wallJumpClimb = new Vector2(x: 7.5f, y: 16);     //If the player jumps while moving in direction of wall.
    public Vector2 wallJumpOff = new Vector2(x: 8.5f, y: 7);        //If the player jumps while not moving in any direction.
    public Vector2 wallLeap = new Vector2(x: 18f, y: 17);           //If the player jumps whule moving in opposite direction of wall.
    //---------------------------------------------------------

    public float wallSlideSpeedMax = 3;                             //The max speed the player will slide down the wall.    
    public float wallStickTime = .25f;                              //If the player is holding in the opposite direction of the wall there is a small deley before the unstick to give them time is with wish to prefrom a wall leap.
    public float timeToWallUnstick;                                 

    protected int wallDirX;                                         //If wall to the left this equals -1 if on right just 1.


    public override State Enter(Player owner)
    {
        base.Enter(owner);
        //----------Unique state attributes-----------------
        CalculateGravity(4,1,.4f);                                   //Set jump for this state.
        owner.gameObject.transform.localScale = new Vector3(1.5f, 1, 0);//Set size for this state.
        spriteRenderer.sprite = owner.liquidPoo;                     //Set sprite for this state.
        moveSpeed = 6f;                                              //Set movement speed for this state.
        controller.allowPassThrough = false;                         //Set weather passthough ability is alloud on this state.
        //---------------------------------------------------
        return this;
    }

    public override void Execute ()
    {
        base.Execute();
        HandleWallSliding();
    }


    //Set velocity if sticking to a wall
    private void HandleWallSliding()
    {
        wallDirX = (controller.collisions.left) ? -1 : 1;//If player facing left var = -1 else +1.
        controller.wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && velocity.y < 0)
        {//If these variables are met the conditions are right for wall sliding.
            controller.wallSliding = true;

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
        }//if
    }//HandleWallSliding

    //Add wall sliding to jump functionality
    override public void OnJumpInputDown()
    {
        base.OnJumpInputDown();

        if (controller.wallSliding)
        {//If player is currently wallsiding.
            if (wallDirX == directionalInput.x)
            {//if trying to move in same direction as facing wall jump up wall.
                velocity.x = -wallDirX * wallJumpClimb.x;
                velocity.y = wallJumpClimb.y;

            }//if
            else if (directionalInput.x == 0)
            {//if just jumping off the wall not pressing left or right.
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
