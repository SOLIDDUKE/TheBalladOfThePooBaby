using UnityEngine;

public class StateMachine {

    public State currentState;
    public Player owner;

    public void Execute()
    {
        if (currentState != null)
        {
            currentState.Execute();
            currentState.Move();
        }
    }

    //Initialize a state and set the FSM's current to that state
    public void ChangeState(State newState, Player player)
    {
        owner = player;
        currentState = newState.Enter(owner);
    }

    //Initialize a state and set the FSM's current to that state
    public void ChangeState(State newState)
    {
        currentState = newState.Enter(owner);
    }
}

public class State
{
    protected Player player;

    protected Vector2 input;
    protected int wallDirX;
    protected float targetVelocityX;

    //Result of state checks in execute
    protected Vector3 velocity;

    #region  State variables
    public float jumpHeight = 4;            
    public float timeToJumpApex = .4f; 

    public float timeToWallUnstick;
    public float accelTimeAirborne = .2f;
    public float accelTimeGrounded = .1f;
    public float moveSpeed = 6;

    public float gravity;
    public float jumpVelocity;
    public float velocityXSmoothing;
    #endregion
    /// <summary>
    /// Create and return an initialized state
    /// </summary>
    public State Enter(Player player)
    {
        this.player = player;
        gravity = player.gravity;
        jumpVelocity = player.jumpVelocity;

        return this;
    }

    /// <summary>
    /// First get inputs and check jump, then run specific state code
    /// </summary>
    public virtual void Execute()
    {
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        wallDirX = (player.controller.collisions.left) ? -1 : 1;//If player facing left var = -1 else +1.

        targetVelocityX = input.x * moveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (player.controller.collisions.below) ? accelTimeGrounded : accelTimeAirborne);

        //Fix accumulating gravity
        if (player.controller.collisions.above || player.controller.collisions.below)
        {
            velocity.y = 0;
        }
    }

    public void Move()
    {
        //If player standing on something.
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (player.controller.collisions.below)
            {
                velocity.y = jumpVelocity;
            }
        }

        velocity.y += gravity * Time.deltaTime;
        player.controller.Move(velocity * Time.deltaTime);
    }
}
