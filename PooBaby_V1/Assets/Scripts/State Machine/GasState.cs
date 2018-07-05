using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasState : State {

    

    public override State Enter(Player owner)
    {
        base.Enter(owner);
        //----------Unique state attributes-----------------
        CalculateGravity(8, 4, .9f);                                 //Set jump for this state.
        owner.gameObject.transform.localScale = new Vector3(2, 1, 0);//Set size for this state.
        spriteRenderer.sprite = owner.gasPoo;                        //Set sprite for this state.
        moveSpeed = 3f;                                              //Set movement speed for this state.
        controller.allowPassThrough = true;                          //Set weather passthough ability is alloud on this state.
        //---------------------------------------------------
        return this;
    }

    public override void Execute()
    {
        base.Execute();

        //Implement
    }
}
