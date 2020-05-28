using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Character : MonoBehaviour
{
    public int health;
    protected float moveSpeed;
    public bool canMove;
    protected Vector2 lastMove;
    public Vector3 targetTile;
    public Vector3 moveTo;
    public Vector3[] path;
    protected int pfn;
    public int readyAction;
    public int selectedAction;
    public int[] actions;
    public bool moving;
    public bool attacking;
    public bool casting;
    public bool proceed;
    public bool simChar;
    public GridModel script;
    protected Animator anim;
    public int layer;


    public List<GameObject> spells;
    public List<Projectile> projectiles;

    public int attackingCounter;
   

    // Start is called before the first frame update
    protected virtual void Start()
    {
        attackingCounter = 50;
        selectedAction = 0;
        projectiles = new List<Projectile>();
        spells = new List<GameObject>();
        proceed = true;
        layer = gameObject.layer;
        moveSpeed = 3f;
        anim = GetComponent<Animator>();
        targetTile = transform.position;
        moving = false;
        moveTo = transform.position;
        //script = FindObjectOfType<GridModel>();
        health = 100;
        readyAction = 0;
        attacking = false;
        actions = new int[3];
    }

    // Update is called once per frame
    protected virtual void FixedUpdate()
    {
        //if(Time.timeScale > 1) {gameObject.transform.position = moveTo; moveTo = gameObject.transform.position;}
        spells.RemoveAll(item => item == null);
        projectiles.RemoveAll(item => item == null);
        this.moveTo.x = (int)Math.Truncate(this.moveTo.x);
        this.moveTo.y = (int)Math.Truncate(this.moveTo.y);
        attacking = false;
        casting = false;
        canMove = true;
        if (gameObject != null)
        {
            //if character is close to target tile, set position to target position
            Vector3 vec = gameObject.transform.position - targetTile;
            if (Math.Abs(vec.x) < 0.1 * Math.Pow(Time.timeScale,8) && Math.Abs(vec.y) < 0.1 * Math.Pow(Time.timeScale,8))
            {
                transform.position = targetTile;
            }

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

            //move character to target tile 
            if (transform.position != moveTo)
            {
                if (moving == false)
                {
                    path = script.pathFinding(this, new Vector2(moveTo.x, moveTo.y));
                    if (path == null || path.Length == 0) { /*Debug.Log("Invalid move target, no path returned for character");*/ moveTo = transform.position; return;}
                    pfn = 0;
                    while (path[pfn] != null && path[pfn].x != 0 && path[pfn].y != 0){
                        //Debug.Log(path[pfn]);
                        pfn++;
                    } 
                    moving = true;
                    pfn--; pfn--;
                }

                if(moving == true)
                {
                    
                    if (path[pfn] != null && path[pfn].x != 0 && path[pfn].y != 0)
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
                if(attacking || casting){
                    anim.SetBool("IsAttacking", attacking);
                    anim.SetBool("IsCasting", casting);                      
                }
                else if(attackingCounter<1){
                    anim.SetBool("IsAttacking", attacking);
                    anim.SetBool("IsCasting", casting);   
                    attackingCounter = 50;
                }
                else attackingCounter--;
                anim.SetFloat("Health", health);
            }
        }
    }

    public void doActions(int[] action)
    {
        if(Time.timeScale > 1){
            switch (action[0])
            {
                case (1):
                    this.addMoveTo(0, 1, 0);
                    break;
                case (2):
                    this.addMoveTo(1, 0, 0);
                    break;
                case (3):
                    this.addMoveTo(0, -0.75f, 0);
                    break;
                case (4):
                    this.addMoveTo(-0.75f, 0, 0);
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
                    this.addMoveTo(0, -0.75f, 0);
                    break;
                case (4):
                    this.addMoveTo(-0.75f, 0, 0);
                    break;
            }  
        }
        if(Time.timeScale <= 1){
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
        }
        //Debug.Log("doActions: " + action[0] + "," + action[1]);
        
        this.readyAction = 1;
        proceed = false;
    }

    public bool canProceed()
    {
        if (this.anim != null && this.anim.GetCurrentAnimatorStateInfo(0).IsName("Idle") && this.spells.Count == 0 && this.projectiles.Count == 0)
        {
            return true;
        }
        return false;
    }

    public void Move(float x, float y)
    {
        float param = Time.timeScale;
        if(param == 0) param = 1;
        transform.Translate(new Vector3(x * moveSpeed * Time.deltaTime / param, y * moveSpeed * Time.deltaTime / param, 0));
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
    public void addMoveTo(float x, float y, float z)
    {
        this.moveTo += new Vector3(x, y, z);
    }

    public virtual bool attackDir(float h, float v)
    {
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        Character ch = null;

        for (int i = 1; i < 3; i++)
        {
            if (dirX == 1)
            {
                ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y));
            }

            if (dirX == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x - i, gameObject.transform.position.y));
            }

            if (dirY == 1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + i));
            }

            if (dirY == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - i));
            }

            if(ch != null) break;
        }

        if (ch != null)
        {
            attacking = true;
            //Debug.Log("Attacking");
            anim.SetBool("IsAttacking", true);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(10, ch);
            return true;
        }
        
        return false;
    }

    public virtual bool spell1Dir(float h, float v)
    {
int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        Character ch = null;

        for (int i = 1; i < 3; i++)
        {
            if (dirX == 1)
            {
                ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y));
            }

            if (dirX == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x - i, gameObject.transform.position.y));
            }

            if (dirY == 1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + i));
            }

            if (dirY == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - i));
            }

            if(ch != null) break;
        }

        if (ch != null)
        {
            attacking = true;
            //Debug.Log("Attacking");
            anim.SetBool("IsAttacking", true);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(10, ch);
            return true;
        }
        
        return false;
    }

    public virtual bool spell2Dir(float h, float v)
    {
int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        Character ch = null;

        for (int i = 1; i < 3; i++)
        {
            if (dirX == 1)
            {
                ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y));
            }

            if (dirX == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x - i, gameObject.transform.position.y));
            }

            if (dirY == 1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y + i));
            }

            if (dirY == -1)
            {
                ch =  script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x, gameObject.transform.position.y - i));
            }

            if(ch != null) break;
        }

        if (ch != null)
        {
            attacking = true;
            //Debug.Log("Attacking");
            anim.SetBool("IsAttacking", true);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(10, ch);
            return true;
        }
        
        return false;
    }

    public virtual bool tryAttacking(){
        bool success = attackDir(0, 1);
        if (!success) success = attackDir(1, 0);
        if (!success) success = attackDir(0, -1);
        if (!success) success = attackDir(-1, 0);
        return success;
    }
}
