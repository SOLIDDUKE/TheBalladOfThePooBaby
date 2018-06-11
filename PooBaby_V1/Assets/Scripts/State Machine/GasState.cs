using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasState : State {

    

    public override State Enter(Player owner)
    {
        base.Enter(owner);
        spriteRenderer.color = Color.green;
        moveSpeed = 1f;
        controller.allowPassThrough = true;
        return this;
    }

    public override void Execute()
    {
        base.Execute();

        //Implement
    }
}
