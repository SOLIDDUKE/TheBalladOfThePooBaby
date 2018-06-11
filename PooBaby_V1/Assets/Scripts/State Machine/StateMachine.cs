using UnityEngine;

[System.Serializable]
public class MovementStateMachine {

    public State CurrentForm { get { return currentState; } private set { currentState = value;  } }
    [SerializeField] public State currentState;

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
        //Destroy(Instantiate(owner.stateChangeEffect, owner.gameObject.transform.position, owner.gameObject.transform.rotation) as GameObject, 2);
    }
}

[System.Serializable]
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

    //protected bool allowPassThrough;
    protected bool wallSliding;
    
    #region  State variables
    [Header("These are NOT saved in inspector. Experiment here, edit in script")]
    [Range(0f, 10f)] public float accelTimeJumpOffWall = .2f;
    [Range(0f, 10f)] public float accelTimeGrounded = .1f;
    public float moveSpeed = 6;

    //Use to calculate gravity
    protected float maxJumpHeight = 4;
    protected float minJumpHeight = 1;
    protected float timeToJumpApex = .4f;
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
        this.owner = owner;
        controller = owner.controller;
        spriteRenderer = controller.gameObject.GetComponent<SpriteRenderer>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);

        return this;
    }

    /// <summary>
    /// FInputs -> HANDLE MOVEMENT (Execute() -- you are here) -> Move
    /// </summary>
    public virtual void Execute()
    {//This is run every frame
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

    protected void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (controller.collisions.below) ? accelTimeGrounded : accelTimeJumpOffWall);
        velocity.y += gravity * Time.deltaTime;
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
            spriteRenderer.flipX=false;
        }
        if (directionalInput.x > 0)
        {//Moving Right
            spriteRenderer.flipX = true;
        }
    }
}
