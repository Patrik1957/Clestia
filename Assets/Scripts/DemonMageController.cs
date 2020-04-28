using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DemonMageController : Character
{
    public bool moveRandomly = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
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
    }
}
