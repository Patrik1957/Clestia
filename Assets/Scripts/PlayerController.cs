using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : Character
{

    /* Moved to GridModel
    void checkNeighbors(int[,] field,int currMove, int i, int j)
    {
        if(field[i+1,j] == 0)
        {
            field[i + 1, j] = currMove;
        }
        if (field[i - 1, j] == 0)
        {
            field[i - 1, j] = currMove;
        }
        if (field[i, j + 1] == 0)
        {
            field[i, j + 1] = currMove;
        }
        if (field[i, j - 1] == 0)
        {
            field[i, j - 1] = currMove;
        }

    }

    Vector2 findMin(Vector2 curr, int[,] field)
    {
        int retX = 0, retY = 0;
        int min = 100;
        int currX = (int) Math.Floor(curr.x);
        int currY = (int) Math.Floor(curr.y);

        if(field[currX + 1,currY] < min)
        {
            min = field[currX + 1, currY];
            retX = currX + 1;
            retY = currY;
        }
        if (field[currX, currY + 1] < min)
        {
            min = field[currX, currY + 1];
            retX = currX;
            retY = currY + 1;
        }
        if (field[currX - 1, currY] < min)
        {
            min = field[currX - 1, currY];
            retX = currX - 1;
            retY = currY;
        }
        if (field[currX, currY - 1] < min)
        {
            min = field[currX, currY - 1];
            retX = currX;
            retY = currY - 1;
        }
        return new Vector2(retX, retY);
    }

    Vector2[] pathFinding (Vector2 current, Vector2 Target)
    {
        bool found = false;
        int[,] field = new int[50,50];
        int currI = (int) Math.Floor(current.x);
        int currJ = (int) Math.Floor(current.y);
        field[currI, currJ] = 1;
        int currMove = 0;

        //Set value of obstacles to -1
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                if (Physics2D.OverlapCircle(new Vector2(i, j), .02f, whatStopsMovement))
                {
                    field[i, j] = -1;
                }
            }
        }

        //Fill in field values until target tile reached or deemed unreachable
        while (!found && currMove<100)
        {
            currMove++;
            for(int i=0; i<50; i++)
            {
                for (int j = 0; j < 50; j++)
                {
                    checkNeighbors(field, currMove, i, j);
                }
            }
            if (field[(int) Target.x, (int) Target.y] > 0) found = true;
        }
        Vector2[] path = new Vector2[currMove];

        //Determine path from current to target tile if it has been succesfully found
        if (found)
        {
            found = false;
            Vector2 curr = Target;
            while (!found)
            {
                path[currMove] = findMin(curr, field);
                curr = path[currMove];
                if (curr == current) found = true;
                currMove--;
            }
        }

        return path;
    }
    */

    // Start is called before the first frame update
    void Start()
    {
        base.Start();
        range = 3;
        damage = 10;
    }

    // Update is called once per frame
    void Update()
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

        /*if player can move and there is keyboard input, set target tile according to input
        if (vec.x < 1.3f && vec.y < 1.3f && vec.x > -1.3f && vec.y > -1.3f && canMove) {
            if (h > 0.5f || h < -0.5f)
            {
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(Math.Sign(h), 0, 0), .02f, whatStopsMovement))
                targetTile = transform.position + new Vector3(Math.Sign(h) , 0, 0);
            }

            else if (v > 0.5f || v < -0.5f)
            {
                if (!Physics2D.OverlapCircle(transform.position + new Vector3(Math.Sign(h), 0, 0), .02f, whatStopsMovement))
                    targetTile = transform.position + new Vector3(0, Math.Sign(v) , 0);
            }
        }*/


        //if there is keyboard input in a direction, check for target in that direction and attack it
        if (!moving && (Math.Abs(h)>0.5f || Math.Abs(v)>0.5f))
        {
            int dirX = 0, dirY = 0;
            if (h > 0.5f) dirX = 1;
            if (h < -0.5f) dirX = -1;
            if (v > 0.5f) dirY = 1;
            if (v < -0.5f) dirY = 1;

            Character target = script.checkEnemy(gameObject, new Vector2(dirX,dirY), range);
            if (target)
            {
                Debug.Log("Attacking");
                anim.SetBool("IsAttacking",true);
                anim.SetFloat("AttackX", dirX);
                anim.SetFloat("AttackY", dirY);
                script.attackEnemy(this, target);
            }
        }
        else anim.SetBool("IsAttacking", false);
        /*
        {
            if (asd < 1)
            {
                asd += Time.deltaTime;
            }
            else
            {
                anim.SetBool("IsAttacking", false);
                asd = 0;
            }
        }*/

        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);
        anim.SetBool("IsMoving", !canMove);
    }
}
