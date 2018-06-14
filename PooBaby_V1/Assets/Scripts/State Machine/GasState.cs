using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasState : State {

    

    public override State Enter(Player owner)
    {
        base.Enter(owner);
        owner.gameObject.transform.localScale = new Vector3(2, 1, 0);
        spriteRenderer.color = Color.green;
        spriteRenderer.sprite = owner.gasPoo;
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
