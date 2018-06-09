using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine : MonoBehaviour {

    public State currentState;

	// Update is called once per frame
	void Update () {
        if(currentState != null) currentState.Execute();
	}

    //Initialize a state and set the FSM's current to that state
    public void ChangeState(State newState)
    {
        currentState = newState.Enter();
    }
}

public class State
{
    /// <summary>
    /// Create and return an initialized state
    /// </summary>
    public State Enter()
    {
        return this;
    }

    /// <summary>
    /// Perform behaviour
    /// </summary>
    public virtual void Execute()
    {

    }
}
