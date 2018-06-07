using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//  follow script for poo baby
public class CameraController : MonoBehaviour
{
    private Transform lookAt; //look at the precious poo baby
    public float boundX = 0.15f; //bound horizontally
    public float boundY = 0.5f; // bind verticallyt

    private void Start()
    {
        lookAt = GameObject.Find("Player").transform;
    }

    private void LateUpdate() //good to use, camera is moved after the player is done moving as calcualtion is alraady done. creates a smoother camera motion Ryan finds, unless theres issues with the sprites im using.

    {
        Vector3 delta = Vector3.zero; //set each to 0

        //check  poo baby is inside the barrier of the x axis
        float deltaX = lookAt.position.x - transform.position.x;
        if (deltaX > boundX || deltaX < -boundX)
        {
            if (transform.position.x < lookAt.position.x)
            {
                delta.x = deltaX - boundX;
            }
            else
            {
                delta.x = deltaX + boundX;
            }
        }

        //check poo baby is inside the barrier of the y axis
        float deltaY = lookAt.position.y - transform.position.y;
        if (deltaY > boundY || deltaY < -boundY)
        {
            if (transform.position.y < lookAt.position.y)
            {
                delta.y = deltaY - boundY;
            }
            else
            {
                delta.y = deltaY + boundY;
            }
        }

        transform.position += new Vector3(delta.x, delta.y, 0);
    }
}

