using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class State
{
    protected Player owner;
    protected Controller2D controller;
    protected SpriteRenderer spriteRenderer;

    protected Vector2 input;
    protected int wallDirX;
    protected float targetVelocityX;
    protected float velocityXSmoothing;

    //Result of state checks in execute
    protected Vector3 velocity;
    protected Vector2 directionalInput;

    protected bool wallSliding;

    #region  State variables
    [Header("These are NOT saved in inspector. Experiment here, edit in script")]
    [Range(0f, 10f)] public float accelTimeJumpOffWall = .2f;
    [Range(0f, 10f)] public float accelTimeGrounded = .1f;
    public float moveSpeed = 6;

    //Set in State Entry
    protected float gravity;
    protected float maxJumpVelocity;
    protected float minJumpVelocity;
    #endregion

    /// <summary>
    /// Create and return an initialized state
    /// </summary>
    public virtual State Enter(Player owner)
    {
        //---------Referances-------------------------------------------------
        this.owner = owner;
        controller = owner.controller;
        spriteRenderer = controller.gameObject.GetComponent<SpriteRenderer>();
        //--------------------------------------------------------------------

        return this;
    }

    /// <summary>
    /// Modify the velocity vector in this method to add movement functionalities.
    /// FInputs -> HANDLE MOVEMENT (Execute() -- you are here) -> Move
    /// This method is called once inputs are known, and before the object is moved.
    /// For example, Liquid state adds wall climbing by overriding this method and fiddling with velocity.
    /// </summary>
    public virtual void Execute(){ }

    public virtual void OnJumpInputDown()
    {
        if (controller.collisions.below)
        {//If player standing on something.
            if (controller.collisions.slidingDownMaxSlope)
            {
                if (directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    velocity.y = maxJumpVelocity * controller.collisions.slopeNormal.y;
                    velocity.x = maxJumpVelocity * controller.collisions.slopeNormal.x;
                }//if
            }//if
            else velocity.y = maxJumpVelocity;
        }//if
    }

    public virtual void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    /// <summary>
    /// Moves the owner, call after movement velocities calculated and handled
    /// </summary>
    public void Move()
    {
        controller.Move(velocity * Time.deltaTime, directionalInput);
        //Fix accumulating gravity
        if (controller.collisions.above || controller.collisions.below)
        {
            if (controller.collisions.slidingDownMaxSlope)
            {
                velocity.y += controller.collisions.slopeNormal.y * -gravity * Time.deltaTime;
            }
            else
            {
                velocity.y = 0;
            }
        }
    }

    public void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGrounded : accelTimeJumpOffWall);
        velocity.y += gravity * Time.deltaTime;
    }


    public void CalculateGravity(float maxJumpHeight)
    {
        float minJumpHeight = 1;
        float timeToJumpApex = .4f;

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
    }



    /// <summary>
    /// This gets called every frame by the player input class in update.
    /// </summary>
    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;

        //Need to find a better way of doing this as it is constantly being called. and does not affect poo baby is his velocity is not changed by player input.
        if (directionalInput.x < 0)
        {//Moving Left
            spriteRenderer.flipX = false;
        }
        if (directionalInput.x > 0)
        {//Moving Right
            spriteRenderer.flipX = true;
        }
    }


}//State Class
