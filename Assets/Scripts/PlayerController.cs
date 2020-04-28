using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : Character
{

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();

    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        bool w = Input.GetKeyDown("w");
        bool a = Input.GetKeyDown("a");
        bool s = Input.GetKeyDown("s");
        bool d = Input.GetKeyDown("d");
        float v = 0, h = 0;
        if (w) v = 1;
        if (s) v = -1;
        if (a) h = -1;
        if (d) h = 1;

        
        //if there is keyboard input in a direction, check for target in that direction and attack it
        if (!moving && (Math.Abs(h)>0.5f || Math.Abs(v)>0.5f))
        {
            attackDir(h, v);
            //moveDir(h, v);
        }
        

        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);
        anim.SetBool("IsMoving", !canMove);
    }

    public override bool spell1Dir(float h, float v)
    {
        return true;
    }

    public override bool spell2Dir(float h, float v)
    {
        return true;
    }
}