using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public GameObject stateChangeEffect;
    public MovementStateMachine movementMachine;
    [HideInInspector] public Controller2D controller;

	void Awake ()
    {
        controller = GetComponent<Controller2D>();

        movementMachine = new MovementStateMachine();
        movementMachine.ChangeState(new SolidState(), this);
	}
  
    private void Update()
    {
        movementMachine.Execute();//This is run every frame as MovementStateMachine is not inheriting from monobehaviour therefore doesn not have an update method.

        Vector2 directionalInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        movementMachine.CurrentForm.SetDirectionalInput(directionalInput);//Tell the state machine the players current input.


        if (Input.GetKeyDown(KeyCode.Space))
        {
            movementMachine.CurrentForm.OnJumpInputDown();
        }//if
        if (Input.GetKeyUp(KeyCode.Space))
        {
            movementMachine.CurrentForm.OnJumpInputUp();
        }//if



        //------STATE SWITCHING--------------------------------------------
        if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Entering gas state");
            movementMachine.ChangeState(new GasState(), this);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Entering liquid state");
            movementMachine.ChangeState(new LiquidState(), this);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Entering solid state");
            movementMachine.ChangeState(new SolidState(), this);
        }
        //----------------------------------------------------------------
    }
}
