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
                velocity.x = (hit.distance - skinWidth) * directionX;
                rayLength = hit.distance;

                collisions.left = directionX == -1; //If hit something and going left collions.left will be true
                collisions.right = directionX == 1;
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

                collisions.below = directionY == -1;
                collisions.above = directionY == 1;
            }//if
        }//for
    }//VerticalCollisions

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

        public void Reset()
        {
            above = below = false;
            left = right = false;
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
