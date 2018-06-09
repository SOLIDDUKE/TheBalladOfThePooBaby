using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//This Script is not complete, will need to be modified so it can:
//Seamlessly Switch to following objects other than the player.
//Block player input.
//Freeze player posision(mid air).
//Seamlessly Switch back to player and unblock movement.
public class CameraFollow : MonoBehaviour {

    public Controller2D target;                                //What the focus area will focus on.
    public Vector2 focusAreaSize = new Vector2(x:3,y:5);       //Set the size of the Focus Area.
    public float lookAheadDstX;                                //The distance the camera will look ahead in the horizontal directions the player is looking.
    public float lookSmoothTimeX;                              //The smooth time for the lookahead feature.
    public float verticalSmoothTime;                           //The smoothe time for verticicly following the player. (Set to 0 if falling from great distances.)
    public float verticalOffset;                               //The verticle offset of the camera.

    float currentLookAheadX;
    float targetLookAheadX;
    float lookAheadDirX;
    float smoothLookVelocityX;
    float smoothVelocityY;

    bool lookAheadStopped;

    FocusArea focusArea;


    void Start()
    {
        focusArea = new FocusArea(target.collider.bounds, focusAreaSize);
    }//Start


    void LateUpdate()
    {
        focusArea.Update(target.collider.bounds);
        Vector2 focusPosition = focusArea.centre + Vector2.up * verticalOffset;


        if (focusArea.velocity.x != 0)
        {
            lookAheadDirX = Mathf.Sign(focusArea.velocity.x);

            if (Mathf.Sign(target.playerInput.x) == Mathf.Sign(focusArea.velocity.x) && target.playerInput.x != 0)
            {
                lookAheadStopped = false;
                targetLookAheadX = lookAheadDirX * lookAheadDstX;
            }//if
            else
            {
                if (!lookAheadStopped)
                {
                    lookAheadStopped = true;
                    targetLookAheadX = currentLookAheadX + (lookAheadDirX * lookAheadDstX - currentLookAheadX) / 4f;
                }//if
            }//else

        }//if

        currentLookAheadX = Mathf.SmoothDamp(currentLookAheadX, targetLookAheadX, ref smoothLookVelocityX, lookSmoothTimeX);

        focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref smoothVelocityY, verticalOffset);
        focusPosition += Vector2.right * currentLookAheadX;
        transform.position = (Vector3)focusPosition + Vector3.forward * -10;
    }//LateUpdate


    void OnDrawGizmos()
    {//Draw the foucs area.
        Gizmos.color = new Color(1, 0, 0, .5f);//Transparent Red
        Gizmos.DrawCube(focusArea.centre, focusAreaSize);
    }//OnDrawGizmos


    /// <summary>
    /// The area in which the player can move without moving the camera.
    /// </summary>
    struct FocusArea
    {
        public Vector2 centre;
        public Vector2 velocity;
        float left, right;
        float top, bottom;

        public FocusArea(Bounds targetBounds, Vector2 size)
        {
            left = targetBounds.center.x - size.x / 2;
            right = targetBounds.center.x + size.x / 2;
            bottom = targetBounds.min.y;
            top = targetBounds.min.y + size.y;

            velocity = Vector2.zero;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
        }//FocusArea

        public void Update(Bounds targetBounds)
        {
            float shiftX = 0;
            if (targetBounds.min.x < left)
            {
                shiftX = targetBounds.min.x - left;
            }//if
            else if (targetBounds.max.x > right)
            {
                shiftX = targetBounds.max.x - right;
            }//if
            left += shiftX;
            right += shiftX;

            float shiftY = 0;
            if (targetBounds.min.y < bottom)
            {
                shiftY = targetBounds.min.y - bottom;
            }//if
            else if (targetBounds.max.y > top)
            {
                shiftY = targetBounds.max.y - top;
            }//if
            top += shiftY;
            bottom += shiftY;
            centre = new Vector2((left + right) / 2, (top + bottom) / 2);
            velocity = new Vector2(shiftX, shiftY);
        }//Update
    }//FocusArea struct



}//CameraFollow Class
