﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    public float moveSpeed;

    protected Animator anim;

    public bool canMove;

    protected Vector2 lastMove;

    public Vector3 targetTile;
    protected Vector3 zerovec;


    public Vector3 moveTo;

    public bool moving;
    public Vector3[] path;
    protected int pfn;

    public GameObject grid;
    protected GridModel script;

    public int health;
    public int range;
    public int damage;
    public int layer;

    

    // Start is called before the first frame update
    protected void Start()
    {
        grid = GameObject.Find("Grid");
        layer = gameObject.layer;
        moveSpeed = 2.5f;
        anim = GetComponent<Animator>();
        targetTile = transform.position;
        zerovec = new Vector3(0, 0, 0);
        moving = false;
        moveTo = transform.position;
        script = FindObjectOfType<GridModel>();
        health = 100;
    }

    // Update is called once per frame
    protected void Update()
    {
        moving = false;
        if (gameObject)
        {
            canMove = true;
            Vector3 vec = transform.position - targetTile;

            //if target position not reached, move towards it
            if (transform.position != targetTile)
            {
                canMove = false;
                if (transform.position.x == targetTile.x)
                {
                    Move(0, Math.Sign(targetTile.y - transform.position.y));
                }
                if (transform.position.y == targetTile.y)
                {
                    Move(Math.Sign(targetTile.x - transform.position.x), 0);
                }
            }

            //if character is close to target, set position to target position
            vec = transform.position - targetTile;
            if (vec.x < 0.05 && vec.y < 0.05 && vec.x > -0.05 && vec.y > -0.05)
            {
                transform.position = targetTile;
            }


            //move character to target tile 
            if (transform.position != moveTo)
            {
                if (moving == false)
                {
                    path = script.pathFinding((Character)this, new Vector2(moveTo.x, moveTo.y));
                    if (path == null) { Debug.Log("Invalid move target");  }
                    pfn = 0;
                    while (path[pfn].x != 0 && path[pfn].y != 0) pfn++;
                    moving = true;
                    pfn--; pfn--;
                }

                if(moving == true)
                {
                    
                    if (path[pfn].x != 0 && path[pfn].y != 0)
                    {
                        Debug.Log(pfn);
                        targetTile = path[pfn];
                        Debug.Log(targetTile);
                        if (transform.position != targetTile) canMove = false;
                        if (canMove && pfn > 0) pfn--;
                    }
                }
            }
            else moving = false;

            if(anim) {
                anim.SetBool("IsMoving", moving);
                anim.SetFloat("LastMoveX", lastMove.x);
                anim.SetFloat("LastMoveY", lastMove.y);
            }
        }
    }

    public void Move(float x, float y)
    {
        transform.Translate(new Vector3(x * moveSpeed * Time.deltaTime, y * moveSpeed * Time.deltaTime, 0));
        canMove = false;
        lastMove = new Vector2(x, y);
        moving = true;
    }

    public void addHP(int x)
    {
        health += x;
        Debug.Log("Health Added: " + x);
    }

    public void setAttrTo(Character ch)
    {
        targetTile = ch.targetTile;
        moveTo = ch.moveTo;
        health = ch.health;
        gameObject.transform.position = ch.transform.position;
    }

    public void setMoveTo(int x, int y, int z)
    {
        this.moveTo = new Vector3(x, y, z);
    }
    public void addMoveTo(int x, int y, int z)
    {
        this.moveTo += new Vector3(x, y, z);
    }

    public void attackRandomly()
    {

    }

    public void spell1Randomly()
    {

    }

    public void spell2Randomly()
    {

    }
}
