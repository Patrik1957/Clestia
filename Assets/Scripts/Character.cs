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
    public GridModel script;
    public int health;
    public int range;
    public int damage;
    public int layer;
    public int readyAction;
    public bool attacking;
    public bool casting;
    public int[] actions;
    public bool proceed;

    public bool simChar;
   

    // Start is called before the first frame update
    protected virtual void Start()
    {
        proceed = true;
        layer = gameObject.layer;
        moveSpeed = 5f;
        anim = GetComponent<Animator>();
        targetTile = transform.position;
        moving = false;
        moveTo = transform.position;
        //script = FindObjectOfType<GridModel>();
        health = 100;
        range = 3;
        damage = 10;
        readyAction = 0;
        attacking = false;
        actions = new int[3];
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        this.moveTo.x = (int)Math.Truncate(this.moveTo.x);
        this.moveTo.y = (int)Math.Truncate(this.moveTo.y);
        attacking = false;
        casting = false;
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
            if (Math.Abs(vec.x) < 0.1 * Time.timeScale && Math.Abs(vec.y) < 0.1 * Time.timeScale)
            {
                transform.position = targetTile;
            }


            //move character to target tile 
            if (transform.position != moveTo)
            {
                if (moving == false)
                {
                    path = script.pathFinding(this, new Vector2(moveTo.x, moveTo.y));
                    if (path == null || path.Length == 0) { /*Debug.Log("Invalid move target, no path returned for character");*/ moveTo = transform.position; return;}
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

            //do action recieved from simulation
            if(!moving && readyAction != 0)
            {
                tryAttacking();
                readyAction = 0;
            }
			if(!moving && readyAction == 0)
				proceed = true;
			
            //set animation
            if(anim != null) {
                anim.SetBool("IsMoving", moving);
                anim.SetFloat("LastMoveX", lastMove.x);
                anim.SetFloat("LastMoveY", lastMove.y);
                anim.SetBool("IsAttacking", attacking);
                anim.SetBool("IsCasting", casting);
            }
        }
    }

    public void doActions(int[] action)
    {
        switch (action[0])
        {
            case (1):
                this.addMoveTo(0, 0.5f, 0);
                break;
            case (2):
                this.addMoveTo(0.5f, 0, 0);
                break;
            case (3):
                this.addMoveTo(0, -0.5f, 0);
                break;
            case (4):
                this.addMoveTo(-0.5f, 0, 0);
                break;
        }
        switch (action[1])
        {
            case (1):
                this.addMoveTo(0, 0.5f, 0);
                break;
            case (2):
                this.addMoveTo(0.5f, 0, 0);
                break;
            case (3):
                this.addMoveTo(0, -0.5f, 0);
                break;
            case (4):
                this.addMoveTo(-0.5f, 0, 0);
                break;
        }
        switch (action[2])
        {
            case (5):
                this.attackRandomly();
                break;
            case (6):
                this.spell1Randomly();
                break;
            case (7):
                this.spell2Randomly();
                break;
        }
        proceed = false;
    }

    public bool canProceed()
    {
        if (this.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            return proceed;
        }
        return false;
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
        //Debug.Log("Health Added: " + x);
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
    public void addMoveTo(float x, float y, float z)
    {
        this.moveTo += new Vector3(x, y, z);
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

    public virtual bool attackDir(float h, float v)
    {
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        Character ch = null;

        for (int i = 1; i < range; i++)
        {
            if (dirX == 1)
            {
                ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y));
            }

            if (dirX == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x - i, gameObject.transform.position.y));
            }

            if (dirY == 1)
            {
                ch =  script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + i));
            }

            if (dirY == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - i));
            }

            if(ch != null) break;
        }

        if (ch != null)
        {
            attacking = true;
            //Debug.Log("Attacking");
            anim.SetBool("IsAttacking", attacking);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(10, ch);
            return true;
        }
        
        return false;
    }

    public virtual bool spell1Dir(float h, float v)
    {
        return false;
    }

    public virtual bool spell2Dir(float h, float v)
    {
        return false;
    }

    public virtual void tryAttacking(){
        bool success = attackDir(0, 1);
        if (!success) success = attackDir(1, 0);
        if (!success) success = attackDir(0, -1);
        if (!success) success = attackDir(-1, 0);
    }
}
