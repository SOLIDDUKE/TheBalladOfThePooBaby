using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : MonoBehaviour {

    const float skinWidth = .015f;
    public int horizontalRayCount = 4;
    public int verticalRayCount = 4;

    float horizontalRaySpacing;
    float verticalRaySpacing;

    BoxCollider2D collider;
    RaycastOrigins raycastOrigins;

    

    void Start ()
    {
        //----------Referances------------
        collider = GetComponent<BoxCollider2D>();
        //--------------------------------

    }//Start


    void Update()
    {
        UpdateRaycastOrigins();
        CalculateRaySpacing();

        for (int i = 0; i < verticalRayCount; i++)
        {
            Debug.DrawRay(raycastOrigins.bottomLeft + Vector2.right * verticalRaySpacing * i, Vector2.up * -2, Color.red);
        }
    }//Update


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
