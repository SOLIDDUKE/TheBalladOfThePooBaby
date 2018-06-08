using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    [Tooltip("The layers the player will collide with.")]
    public LayerMask collisionMask;         //The layers the player will collide with.

    const float skinWidth = .015f;          //The width of the inset from where the rays are cast.
    public int horizontalRayCount = 4;      //The ammount of rays to be cast on the horizontal directions.
    public int verticalRayCount = 4;        //The ammount of rays to be cast in the verticel directions.

    public CollisionInfo collisions;

    float maxClimbAngle = 80;
    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    

    void Start ()
    {
        //----------Referances------------
        collider = GetComponent<BoxCollider2D>();
        //--------------------------------
        CalculateRaySpacing();
    }//Start


    /// <summary>
    /// Methods called when player is moved.
    /// </summary>
    public void Move(Vector3 velocity)
    {
        UpdateRaycastOrigins();

        collisions.Reset();

        if (velocity.x !=0) HorizontalCollisions(ref velocity);
        if (velocity.y !=0) VerticalCollisions(ref velocity);
        transform.Translate(velocity);
    }//Move

    

    /// <summary>
    /// Detecting collisions in horizontal directions with raycasts.
    /// </summary>
    void HorizontalCollisions(ref Vector3 velocity)
    {
        float directionX = Mathf.Sign(velocity.x);
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        for (int i = 0; i < horizontalRayCount; i++)
        {
            Vector2 rayOrigin = (directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                if (i == 0 && slopeAngle <= maxClimbAngle)
                {//if the bottom ray hits a slope.
                    print(slopeAngle);
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

    /// <summary>
    /// Raycast origin will be inset by small ammount to allow casting when player object is flat agains a surface.
    /// </summary>
    void UpdateRaycastOrigins()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }//UpdateRaycastOrigins

    /// <summary>
    /// Minimum of 2 rays are casted and the space between the rays is calculated.
    /// </summary>
    void CalculateRaySpacing()
    {
        Bounds bounds = collider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }//CalculateRaySpacing

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public float slopeAngle, slopeAngleOld;
        public void Reset()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }//Reset

    }//CollisionInfo Struct

    

    /// <summary>
    /// Will Store corners of player collider in vector form.
    /// These vectors will be where the raycasts emit from.
    /// </summary>
    struct RaycastOrigins
    {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;

    }//RaycastOrigins Scruct

}//Controller2D Class
