﻿using UnityEngine;

public class MovementStateMachine {

    public State CurrentForm { get; private set; }

    public void Execute()
    {
        if (CurrentForm != null)
        {
            CurrentForm.Execute();
            CurrentForm.Move();
        }
    }

    //Initialize a state and set the FSM's current to that state
    public void ChangeState(State newState, Player owner)
    {
        CurrentForm = newState.Enter(owner);
    }
}

public class State
{
    protected Player owner;
    protected Controller2D controller;

    protected Vector2 input;
    protected int wallDirX;
    protected float targetVelocityX;

    //Result of state checks in execute
    protected Vector3 velocity;
    protected Vector2 directionalInput;

    #region  State variables
    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;

    public bool allowPassThrough;
    public bool wallSliding;

    public float timeToWallUnstick;
    public float accelTimeAirborne = .2f;
    public float accelTimeGrounded = .1f;
    public float moveSpeed = 6;

    public float gravity;
    public float maxJumpVelocity;
    public float minJumpVelocity;
    public float velocityXSmoothing;
    #endregion

    /// <summary>
    /// Create and return an initialized state
    /// </summary>
    public State Enter(Player owner)
    {
        this.owner = owner;
        controller = owner.controller;

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        controller.allowPassThrough = allowPassThrough;
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        return this;
    }

    /// <summary>
    /// First get inputs and check jump, then run specific state code
    /// </summary>
    public virtual void Execute()
    {
        CalculateVelocity();
    }

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
        controller.Move(velocity * Time.deltaTime, input);

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

    protected void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGrounded : accelTimeAirborne);
        velocity.y += gravity * Time.deltaTime;
    }

    public void SetDirectionalInput(Vector2 input)
    {
        directionalInput = input;
    }
}
