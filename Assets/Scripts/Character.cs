using System.Collections;
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
    private int readyAction;
    public bool attacking;
    public int[] actions;
    

    // Start is called before the first frame update
    protected void Start()
    {
        grid = GameObject.Find("Grid");
        layer = gameObject.layer;
        moveSpeed = 2.5f;
        anim = GetComponent<Animator>();
        targetTile = transform.position;
        moving = false;
        moveTo = transform.position;
        script = FindObjectOfType<GridModel>();
        health = 100;
        range = 3;
        damage = 10;
        readyAction = 0;
        attacking = false;
        actions = new int[3];
    }

    // Update is called once per frame
    protected void Update()
    {
        moving = false;
        attacking = false;
        if (gameObject)
        {
            canMove = true;
            Vector3 vec = transform.position - targetTile;

            //if target tile not reached, move towards it
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
                    if (path == null || path.Length == 0) { Debug.Log("Invalid move target");  }
                    pfn = 0;
                    while (path[pfn].x != 0 && path[pfn].y != 0) pfn++;
                    moving = true;
                    pfn--; pfn--;
                }

                if(moving == true)
                {
                    
                    if (path[pfn].x != 0 && path[pfn].y != 0)
                    {
                        //Debug.Log(pfn);
                        targetTile = path[pfn];
                        //Debug.Log(targetTile);
                        if (transform.position != targetTile) canMove = false;
                        if (canMove && pfn > 0) pfn--;
                    }
                }
            }
            else moving = false;

            if(!moving && readyAction != 0)
            {
                if (readyAction == 1) attackRandomly();
                if (readyAction == 2) spell1Randomly();
                if (readyAction == 3) spell2Randomly();
                readyAction = 0;
                attacking = true;
            }

            if(anim) {
                anim.SetBool("IsMoving", moving);
                anim.SetFloat("LastMoveX", lastMove.x);
                anim.SetFloat("LastMoveY", lastMove.y);
                anim.SetBool("IsAttacking", attacking);
            }
        }
    }

    public void doActions(int[] action)
    {
        switch (action[0])
        {
            case (1):
                this.addMoveTo(0, 1, 0);
                break;
            case (2):
                this.addMoveTo(1, 0, 0);
                break;
            case (3):
                this.addMoveTo(0, -1, 0);
                break;
            case (4):
                this.addMoveTo(-1, 0, 0);
                break;
        }
        switch (action[1])
        {
            case (1):
                this.addMoveTo(0, 1, 0);
                break;
            case (2):
                this.addMoveTo(1, 0, 0);
                break;
            case (3):
                this.addMoveTo(0, -1, 0);
                break;
            case (4):
                this.addMoveTo(-1, 0, 0);
                break;
        }
        switch (action[2])
        {
            case (1):
                this.attackRandomly();
                break;
            case (2):
                this.spell1Randomly();
                break;
            case (3):
                this.spell2Randomly();
                break;
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
        this.health = ch.health;
        this.targetTile = ch.targetTile + new Vector3(100, 0, 0);
        this.moveTo = (ch.moveTo + new Vector3(100, 0, 0));
        gameObject.transform.position = ch.transform.position + new Vector3(100,0,0);
    }

    public void setMoveTo(int x, int y, int z)
    {
        this.moveTo = new Vector3(x, y, z);
    }
    public void addMoveTo(int x, int y, int z)
    {
        this.moveTo += new Vector3(x, y, z);
    }

    public void readyAttack()
    {
        bool success = attackDir(0, 1);
        if (!success) success = attackDir(1, 0);
        if (!success) success = attackDir(0, -1);
        if (!success) success = attackDir(-1, 0);
    }

    public void readySpell1()
    {
        readyAttack(); //temp
    }


    public void readySpell2()
    {
        readyAttack(); //temp
    }

    public void attackRandomly()
    {
        readyAction = 1;
    }

    public void spell1Randomly()
    {
        readyAction = 2;
    }

    public void spell2Randomly()
    {
        readyAction = 3;
    }

    public bool attackDir(float h, float v)
    {
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        Character target = script.checkEnemy(gameObject, new Vector2(dirX, dirY), range);
        if (target)
        {
            Debug.Log("Attacking");
            anim.SetBool("IsAttacking", true);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(this, target);
            return true;
        }
        
        return false;
    }

}
