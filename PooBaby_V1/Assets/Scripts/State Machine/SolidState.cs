using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolidState : State {



    public override State Enter(Player owner)
    {
        base.Enter(owner);
        owner.gameObject.transform.localScale = new Vector3(1, 2, 0);
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = owner.solidPoo;
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
