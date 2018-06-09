using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Controller2D : RaycastController {


    public CollisionInfo collisions;

    internal bool allowPassThrough;     //Determin weather the precious baby is alloud to pass through platfroms that can be passed through.
    float maxClimbAngle = 80;           //The maximum angle player can climb.
    float maxDescendAngle = 75;         //The maximum angle player can descend.

    internal Vector2 playerInput;

    public override void Start()
    {
        base.Start();
        collisions.faceDir = 1;
    }//Start

    public void Move(Vector3 velocity, bool standingOnPlatform)
    {//This method just calls the main move method so the moving platfrom class doens t have to worry about a vector 2 input.
        Move(velocity, Vector2.zero, standingOnPlatform);
    }//Move

    /// <summary>
    /// Methods called when player is moved.
    /// </summary>
    public void Move(Vector3 velocity, Vector2 input, bool standingOnPlatform=false)
    {
        UpdateRaycastOrigins();
        collisions.Reset();
        collisions.velocityOld = velocity;
        playerInput = input;

        if (velocity.x != 0) collisions.faceDir = (int)Mathf.Sign(velocity.x); //set facing direction of player.

        if (velocity.y < 0) DescendSlope(ref velocity);

        HorizontalCollisions(ref velocity);

        if (velocity.y !=0) VerticalCollisions(ref velocity);

        transform.Translate(velocity);

        if (standingOnPlatform) collisions.below = true;
    }//Move

    

    /// <summary>
    /// Detecting collisions in horizontal directions with raycasts.
    /// </summary>
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = collisions.faceDir;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {//Giving extra distance to detect wall.
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                if (hit.distance == 0)
                {//This fixes slow movement when inside moving platfrom.
                    continue;
                }//if

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {//if the bottom ray hits a slope.
                    if (collisions.descendingSlope)
                    {//Fixes issue with moving between to slopes beside and intersecting (like a 'V' shape).
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }//if

                    //print(slopeAngle);
                    float distnaceToSlopeStart = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {//If player is starting to climb a new slope.
                        distnaceToSlopeStart = hit.distance - skinWidth;
                        velocity.x -= distnaceToSlopeStart * directionX; //Climb slope method will use the velocity of when it reaches the slope rather than when the raycast does
                    }//if
                    ClimbSlope(ref velocity, slopeAngle);
                    velocity.x += distnaceToSlopeStart * directionX;
                }//if

                if (!collisions.climbingSlope || slopeAngle > maxClimbAngle)
                {//Only check other rays if climbing a slope.
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    if (collisions.climbingSlope)
                    {//Stops player spazzing when colliding with object on slope.
                        velocity.y = Mathf.Tan(collisions.slopeAngle* Mathf.Deg2Rad)*Mathf.Abs(velocity.x);
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
    void VerticalCollisions(ref Vector3 velocity)
    {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++)
        {
            Vector2 rayOrigin = (directionY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit)
            {
                if (hit.collider.tag == "Through" && allowPassThrough)
                {//if the player has hit an object they can passthrough and passthrough is alloud.
                    if (directionY == 1 || hit.distance ==0)
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
                        Invoke("ResetFallingThroughPlatfrom",.5f);
                        continue;
                    }//if
                }//if

                velocity.y = (hit.distance -skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisions.climbingSlope)
                {//Stops player spazzing if collision above him on slope.
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }//if
        }//for

        if (collisions.climbingSlope)
        {//Stops player getting stuck for a frame in begeniing of slope degree change
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {//means player has collided with new slope on a slope
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisions.slopeAngle = slopeAngle;
                }//if
            }//if
        }//if
    }//VerticalCollisions

    void ClimbSlope(ref Vector3 velocity, float slopeAngle)
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;//If player is climbing a slope assume ground collision
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
        }
 
    }//ClimbSlope

    void DescendSlope(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        //ray origin will come from direction player is descending slope.
        Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, collisionMask);

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle != 0 && slopeAngle <= maxDescendAngle)
            {
                if (Mathf.Sign(hit.normal.x) == directionX)
                {//Means player is moving down slope.
                    if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                    {//Check if close enough to slope and not just falling down slope.
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisions.slopeAngle = slopeAngle;
                        collisions.descendingSlope = true;
                        collisions.below = true;
                    }//if
                }//if
            }//if
        }//if
    }//DescendSlope

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
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;
        public int faceDir;
        public bool fallingThroughPlatfrom;

        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;

            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }//Reset

    }//CollisionInfo Struct

}//Controller2D Class
