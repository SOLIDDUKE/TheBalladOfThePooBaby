using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public MovementStateMachine movementMachine;
    public Controller2D controller;

	void Awake ()
    {
        controller = GetComponent<Controller2D>();

        movementMachine = new MovementStateMachine();
        movementMachine.ChangeState(new LiquidState(), this);
	}

    private void Update()
    {
        movementMachine.Execute();
    }
}
