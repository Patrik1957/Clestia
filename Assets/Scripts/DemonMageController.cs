using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DemonMageController : Character
{
    public bool moveRandomly = false;

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        base.Update();
        if (moveRandomly)
        {
            System.Random rnd = new System.Random();
            int randomX = rnd.Next(-3, 3);
            int randomY = rnd.Next(-3, 3);
            moveTo = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);
            moveRandomly = false;
        }

        

        /*
        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);
        anim.SetBool("IsMoving", !canMove);*/
    }
}
