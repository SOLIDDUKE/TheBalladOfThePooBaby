using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidState : State {



    public override State Enter(Player owner)
    {
        base.Enter(owner);
        //----------Unique state attributes-----------------
        CalculateGravity(4,1,.4f);                                   //Set jump for this state.
        owner.gameObject.transform.localScale = new Vector3(1, 2, 0);//Set size for this state.
        spriteRenderer.sprite = owner.solidPoo;                      //Set sprite for this state.
        moveSpeed = 6f;                                              //Set movement speed for this state.
        controller.allowPassThrough = false;                         //Set weather passthough ability is alloud on this state.
        //---------------------------------------------------
        return this;
    }

    public override void Execute()
    {
        base.Execute();

        //Implement
    }
}
