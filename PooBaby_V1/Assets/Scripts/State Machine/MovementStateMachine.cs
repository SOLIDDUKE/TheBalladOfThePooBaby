using UnityEngine;

public class MovementStateMachine : MonoBehaviour
{
    public State CurrentForm { get; private set; }

    //public void Execute()
    public void Update()
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
        Destroy(Instantiate(owner.stateChangeEffect, owner.gameObject.transform.position, owner.gameObject.transform.rotation) as GameObject, 2);
    }
}//MovementStateMachine

public enum PooTypes { Solid, Gas, Liquid }
