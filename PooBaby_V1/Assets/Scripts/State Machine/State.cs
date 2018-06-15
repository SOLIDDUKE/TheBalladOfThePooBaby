using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State
{
    protected Player owner;
    protected Controller2D controller;
    protected SpriteRenderer spriteRenderer;

    protected float targetVelocityX;        //This variable is present because there is smoothing on the plater velocity in place. So the player wil build up to target velocity.
    protected float velocityXSmoothing;     //This is used in smoothing the players velocity on the x axis.
    protected float accelTimeGrounded = .1f;//Time it takes the player to accelerate when they are on the ground.
    protected float accelTimeJumpOffWall = .2f;//Time it takes player to accelerate when wall jumping.


    //Result of state checks in execute
    protected Vector3 velocity;             //The current velocity of the player.
    protected Vector2 directionalInput;     //The directional input of player if moving left this will equal(-1.0, 0.0)


    //Set in State Entry
    protected float moveSpeed;              //The move speed is set in each state.
    protected float gravity;                //The calculate gravity method is called in  state.
    protected float maxJumpVelocity;        //This is the max velocity player can reach when jumping (calculated in CalculateGravity method).
    protected float minJumpVelocity;        //This is the min velocity player can reach when jumping (calculated in CalculateGravity method).

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
    public virtual void Execute(){}

    /// <summary>
    /// Called by player class when jump button is hit.
    /// </summary>
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

    /// <summary>
    /// Called by player class when jump button is released.
    /// </summary>
    public virtual void OnJumpInputUp()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    /// <summary>
    /// Moves the owner, call after movement velocities calculated and handled. 
    /// Called every frame in Movement State Machine.
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


    /// <summary>
    /// Used to calculate the velocity.
    /// Called every frame in Movement State Machine.
    /// </summary>
    public void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGrounded : accelTimeJumpOffWall);
        velocity.y += gravity * Time.deltaTime;
    }

    /// <summary>
    /// Called in the Enter method by each state so each state can have unique gravityy and jump values.
    /// </summary>
    public void CalculateGravity(float maxJumpHeight, float minJumpHeight, float timeToJumpApex)
    {//example maxJumpHeight =4, minJumpHeight=1, timeToJumpApex =.4f

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
