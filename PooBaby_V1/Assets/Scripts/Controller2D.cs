using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController
{
    public CollisionInfo collisions;

    internal bool allowPassThrough;     //Determin weather the precious baby is alloud to pass through platfroms that can be passed through.
    public float maxSlopeAngle = 80;    //The maximum angle player can climb and decend.

    internal Vector2 playerInput;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }//Start

    public void Move(Vector2 moveAmmount, bool standingOnPlatform)
    {//This method just calls the main move method so the moving platfrom class doens t have to worry about a vector 2 input.
        Move(moveAmmount, Vector2.zero, standingOnPlatform);
    }//Move

    /// <summary>
    /// Methods called when player is moved.
    /// </summary>
    public void Move(Vector2 moveAmmount, Vector2 input, bool standingOnPlatform = false)
    {
        CalculateRaySpacing();      //Improvement would be to calculate only on state cahnge rather than every frame.
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = moveAmmount;
        playerInput = input;


        if (moveAmmount.y < 0) DescendSlope(ref moveAmmount);

        if (moveAmmount.x != 0) collisions.faceDir = (int)Mathf.Sign(moveAmmount.x); //set facing direction of player.


        HorizontalCollisions(ref moveAmmount);

        if (moveAmmount.y != 0) VerticalCollisions(ref moveAmmount);
        
        transform.Translate(moveAmmount);

        if (standingOnPlatform) collisions.below = true;
    }//Move



    /// <summary>
    /// Detecting collisions in horizontal directions with raycasts.
    /// </summary>
    void HorizontalCollisions(ref Vector2 moveAmmount)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(moveAmmount.x) + skinWidth;

        if (Mathf.Abs(moveAmmount.x) < skinWidth)
        {//Giving extra distance to detect wall.
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {//This fixes slow movement when inside moving platfrom.
                    continue;
                }//if

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxSlopeAngle)
                {//if the bottom ray hits a slope.
                    if (collisions.descendingSlope)
                    {//Fixes issue with moving between to slopes beside and intersecting (like a 'V' shape).
                        collisions.descendingSlope = false;
                        moveAmmount = collisions.velocityOld;
                    }//if

                    //print(slopeAngle);
                    float distnaceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {//If player is starting to climb a new slope.
                        distnaceToSlopeStart = hit.distance - skinWidth;
                        moveAmmount.x -= distnaceToSlopeStart * directionX; //Climb slope method will use the moveAmmount of when it reaches the slope rather than when the raycast does
                    }//if
                    ClimbSlope(ref moveAmmount, slopeAngle, hit.normal);
                    moveAmmount.x += distnaceToSlopeStart * directionX;
                }//if

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {//Only check other rays if climbing a slope.
                    moveAmmount.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {//Stops player spazzing when colliding with object on slope.
                        moveAmmount.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmmount.x);
                    }//if

                    collisions.left = directionX == -1; //If hit something and going left collions.left will be true
                    collisions.right = directionX == 1;
                }//if 
            }//if
        }//for
    }//HorizontalCollisions

    /// <summary>
    /// Detecting collisions in verticle directions with raycasts.
    /// </summary>
    void VerticalCollisions(ref Vector2 moveAmmount)
    {
        float directionY = Mathf.Sign(moveAmmount.y);
        float rayLength = Mathf.Abs(moveAmmount.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + moveAmmount.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through" && allowPassThrough)
                {//if the player has hit an object they can passthrough and passthrough is alloud.
                    if (directionY == 1 || hit.distance == 0)
                    {//if moving up
                        continue;
                    }//if
                    if (collisions.fallingThroughPlatfrom)
                    {
                        continue;
                    }//if
                    if (playerInput.y == -1)
                    {//if player presses down fall through.
                        collisions.fallingThroughPlatfrom = true;
                        Invoke("ResetFallingThroughPlatfrom", .5f);
                        continue;
                    }//if
                }//if

                moveAmmount.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {//Stops player spazzing if collision above him on slope.
                    moveAmmount.x = moveAmmount.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(moveAmmount.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }//if
        }//for

        if (collisions.climbingSlope)
        {//Stops player getting stuck for a frame in begeniing of slope degree change
            float directionX = Mathf.Sign(moveAmmount.x);
            rayLength = Mathf.Abs(moveAmmount.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * moveAmmount.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {//means player has collided with new slope on a slope
                    moveAmmount.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = hit.normal;
                }//if
            }//if
        }//if
    }//VerticalCollisions

    void ClimbSlope(ref Vector2 moveAmmount, float slopeAngle, Vector2 slopeNormal)
    {
        float moveDistance = Mathf.Abs(moveAmmount.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (moveAmmount.y <= climbVelocityY)
        {
            moveAmmount.y = climbVelocityY;
            moveAmmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmmount.x);
            collisions.below = true;//If player is climbing a slope assume ground collision
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }//if
    }//ClimbSlope

    void DescendSlope(ref Vector2 moveAmmount)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(moveAmmount.y) + skinWidth, collisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(moveAmmount.y) + skinWidth, collisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {//If only one of these is true call method.
            SlideDownMaxSlope(maxSlopeHitLeft, ref moveAmmount);
            SlideDownMaxSlope(maxSlopeHitRight, ref moveAmmount);
        }//if
        
        if (!collisions.slidingDownMaxSlope)
        {//Only needs to calculate if not sliding down a max slope
            float directionX = Mathf.Sign(moveAmmount.x);
            //ray origin will come from direction player is descending slope.
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == directionX)
                    {//Means player is moving down slope.
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(moveAmmount.x))
                        {//Check if close enough to slope and not just falling down slope.
                            float moveDistance = Mathf.Abs(moveAmmount.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            moveAmmount.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(moveAmmount.x);
                            moveAmmount.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                            collisions.slopeNormal = hit.normal;
                        }//if
                    }//if
                }//if
            }//if
        }//if
    }//DescendSlope

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmmount)
    {
        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);
                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }//if
        }//if
    }//SlideDownMaxSlope


    void ResetFallingThroughPlatfrom()
    {
        collisions.fallingThroughPlatfrom = false;
    }//ResetFallingThroughPlatfrom

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector2 velocityOld;
        public int faceDir;
        public bool fallingThroughPlatfrom;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }//Reset

    }//CollisionInfo Struct

}//Controller2D Class