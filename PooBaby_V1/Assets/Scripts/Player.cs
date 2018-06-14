using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class Player : MonoBehaviour {

    public GameObject stateChangeEffect;

    public Sprite liquidPoo;            
    public Sprite solidPoo;
    public Sprite gasPoo;

    private Dictionary<PooTypes, State> states;

    [HideInInspector] public MovementStateMachine movementMachine;
    [HideInInspector] public Controller2D controller;

	void Awake ()
    {
        //--------Referances-------------------------------------
        controller = GetComponent<Controller2D>();
        movementMachine = GetComponent<MovementStateMachine>();
        //-------------------------------------------------------

        //States are saved as objects in dictionary.
        states = new Dictionary<PooTypes, State>
        {
            { PooTypes.Gas, new GasState() },
            { PooTypes.Liquid, new LiquidState() },
            { PooTypes.Solid, new SolidState() }
        };

        movementMachine.ChangeState(states[PooTypes.Solid], this); //On Start he will be this state.
    }
  
    private void Update()
    {
        //-----MOVEMENT HANDLING------------------------------------------
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
            movementMachine.ChangeState(states[PooTypes.Gas], this);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Entering liquid state");
            movementMachine.ChangeState(states[PooTypes.Liquid], this);
        }

        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Entering solid state");
            movementMachine.ChangeState(states[PooTypes.Solid], this);
        }
        //----------------------------------------------------------------
    }
}
