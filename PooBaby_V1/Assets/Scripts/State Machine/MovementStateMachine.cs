using UnityEngine;

/// <summary>
/// Runs three methods, which each modify and act upon a State's VELOCITY attribute:
/// 1. Calculate velocity - gets current player input, and sets the velocity
/// 2. Execute - handles all movement functionalities
/// 3. Move - moves the object according to the above 2 steps
/// </summary>
public class MovementStateMachine : MonoBehaviour
{
    public State CurrentForm { get; private set; }

    //public void Execute()
    public void Update()
    {
        if (CurrentForm != null)
        {
            //Get and set input
            CurrentForm.CalculateVelocity();

            //Handle Movement Functionalities
            CurrentForm.Execute();

            //Move the object
            CurrentForm.Move();
        }
    }

    //Initialize a state and set the FSM's current to that state
    public void ChangeState(State newState, Player owner)
    {
        CurrentForm = newState.Enter(owner);
        Destroy(Instantiate(owner.stateChangeEffect, owner.gameObject.transform.position, owner.gameObject.transform.rotation) as GameObject, 2);
    }
}//MovementStateMachine

public enum PooTypes { Solid, Gas, Liquid }
