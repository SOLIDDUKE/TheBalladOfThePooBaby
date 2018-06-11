using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidState : State {



    public override State Enter(Player owner)
    {
        base.Enter(owner);
        spriteRenderer.color = Color.white;
        controller.allowPassThrough = false;
        moveSpeed = 6f;
        return this;
    }

    public override void Execute()
    {
        base.Execute();

        //Implement
    }
}
