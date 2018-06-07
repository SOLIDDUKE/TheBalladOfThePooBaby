using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoxieInteract : Interactable {

    public int delay;
    MeshRenderer meshRenderer;
    Color originalColor;

    public override void Interact()
    {
        base.Interact();
        // originalColor = GetComponent<MeshRenderer>().material.color;
        // GetComponent<MeshRenderer>().material.color = Color.red;
        GetComponent<Animesh>().m_CurrentAnimesh = "Walk";
        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(delay);
        // GetComponent<MeshRenderer>().material.color = originalColor;
        GetComponent<Animesh>().m_CurrentAnimesh = "Spike";
    }

}
