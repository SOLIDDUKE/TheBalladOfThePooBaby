using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[System.Serializable]
public class MovementStateMachine : MonoBehaviour
{

    public State CurrentForm { get { return currentState; } private set { currentState = value; } }
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
        Destroy(Instantiate(owner.stateChangeEffect, owner.gameObject.transform.position, owner.gameObject.transform.rotation) as GameObject, 2);
    }
}//MovementStateMachine
