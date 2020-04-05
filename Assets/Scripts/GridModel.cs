using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GridModel : MonoBehaviour
{
    public bool[,] tileWalkable;
    public Character[,] characters;
    public LayerMask whatStopsMovement;
    private Character obstacle;
    private Character free;
    public Character DemonMage;
    public Character Knob;
    public Character Checkmark;

    // Start is called before the first frame update
    void Start()
    {
        tileWalkable = new bool[50, 50];
        characters = new Character[50, 50];
        obstacle = Instantiate(Knob, new Vector3(-1, -1, 1), new Quaternion(0, 0, 0, 0));
        free = Instantiate(Checkmark, new Vector3(-1, -1, 1), new Quaternion(0, 0, 0, 0));

        //Set value of obstacles to false
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                if (Physics2D.OverlapCircle(new Vector2(i + .5f, j + .5f), .05f, whatStopsMovement))
                {
                    tileWalkable[i, j] = false;
                    characters[i, j] = obstacle;
                }
                else
                {
                    tileWalkable[i, j] = true;
                    characters[i, j] = free;
                }                
            }
        }

        characters[26,25] = Instantiate(DemonMage, new Vector3(26,25,1), new Quaternion(0,0,0,0));
    }

    
    public Character checkEnemy(GameObject importedGO, Vector2 direction, int range)
    {
        int currI = (int)Math.Floor(importedGO.transform.position.x);
        int currJ = (int)Math.Floor(importedGO.transform.position.y);

        Debug.Log("Locating Target");

        for (int i = 0; i < range; i++)
        {
            if (direction.x == 1)
            {
                if(characters[currI + i, currJ].layer == 9)
                {
                    return (characters[currI + i, currJ]);
                }
            }

            if (direction.x == -1)
            {
                if (characters[currI - i, currJ].layer == 9)
                {
                    return (characters[currI - i, currJ]);
                }
            }

            if (direction.y == 1)
            {
                if (characters[currI, currJ + i].layer == 9)
                {
                    return (characters[currI, currJ + i]);
                }
            }

            if (direction.y == -1)
            {
                if (characters[currI, currJ - i].layer == 9)
                {
                    return (characters[currI, currJ - i]);
                }
            }
        }

        Debug.Log("target lost");

        return null;
    }

    public void attackEnemy(Character attacker, Character target)
    {
        Character att = characters[(int)Math.Ceiling(attacker.transform.position.x), (int)Math.Ceiling(attacker.transform.position.y)];
        Character targ = characters[(int)Math.Floor(target.transform.position.x), (int)Math.Floor(target.transform.position.y)];
        target.addHP(-1 * attacker.damage);
    }

    private void checkNeighbors(int[,] field, int ii, int jj)
    {
        //változók definíciója
        int currMove = 2, n, i, j;
        Vector2[] nextList, currList;
        bool keepGoing = true;

        //kiinduló pozíció
        currList = new Vector2[1];
        currList[0] = new Vector2(ii, jj);
        field[(int)Math.Floor(currList[0].x), (int)Math.Floor(currList[0].y)] = 1;

        //a listát currMove-onként újra feltöltjük, és megvizsgáljuk a szomszédokat az összes listabeli elemre
        while(keepGoing)
        {
            nextList = new Vector2[150];
            keepGoing = false;
            n = 0;
            foreach( Vector2 element in currList)
            {
                if(element.x != 0 && element.y != 0)
                {
                    i = (int)Math.Floor(element.x);
                    j = (int)Math.Floor(element.y);
                    if (field[i + 1, j] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i + 1, j] = currMove;
                        nextList[n] = new Vector2(i + 1, j);n++;
                        keepGoing = true;
                    }
                    if (field[i - 1, j] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i - 1, j] = currMove;
                        nextList[n] = new Vector2(i - 1, j);n++;
                        keepGoing = true;
                    }
                    if (field[i, j + 1] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i, j + 1] = currMove;
                        nextList[n] = new Vector2(i, j + 1);n++;
                        keepGoing = true;
                    }
                    if (field[i, j - 1] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i, j - 1] = currMove;
                        nextList[n] = new Vector2(i, j - 1);n++;
                        keepGoing = true;
                    }
                }
            }
            currMove++;
            currList = nextList;
        }

        /*
        if (i > 2 && i < 48 && j > 2 && j < 48)
        {
            if (field[i + 1, j] == 0)
            {
                field[i + 1, j] = currMove;
                checkNeighbors(field, currMove + 1, i + 1, j);
            }
            if (field[i - 1, j] == 0)
            {
                field[i - 1, j] = currMove;
                checkNeighbors(field, currMove + 1, i - 1, j);
            }
            if (field[i, j + 1] == 0)
            {
                field[i, j + 1] = currMove;
                checkNeighbors(field, currMove + 1, i, j + 1);
            }
            if (field[i, j - 1] == 0)
            {
                field[i, j - 1] = currMove;
                checkNeighbors(field, currMove + 1, i, j - 1);
            }
        }
        */
    }

    private Vector3 findMin(Vector2 curr, int[,] field)
    {
        int retX = 0, retY = 0;
        int min = 100;
        int currX = (int)curr.x;
        int currY = (int)curr.y;

        if (field[currX + 1, currY] < min)
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
        return new Vector3(retX, retY, 1);
    }


    public Vector3[] pathFinding(Character recievedCharacter, Vector2 Target)
    {


        Vector2 current = new Vector2(recievedCharacter.transform.position.x, recievedCharacter.transform.position.y);
        bool found = false;
        Vector3[] path = new Vector3[50];
        int[,] field = new int[50, 50];
        int currI = (int)current.x;
        int currJ = (int)current.y;

        if (currI < 0 || currJ < 0) return null;

        //Set value of obstacles to -1
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                if (tileWalkable[i, j] == false)
                {
                    field[i, j] = -1;
                }
            }
        }
        field[currI, currJ] = 1;

        //return null if target is an obstacle
        if (field[(int)Target.x, (int)Target.y] == -1) { Debug.Log("error: invalid target location"); }

        //Fill in field values
        checkNeighbors(field, currI, currJ);

       /*
        //show generated field
        String deb = "";
        for(int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                deb += field[i, j];
                deb += ' ';
            }
            deb += "#";
        }
        Debug.Log(deb);*/


        //Determine path from current to target tile
        found = true;
        int currMove = 1;
        path[0] = new Vector3((int)Target.x, (int)Target.y, 1);
        path[0].z = 1;
        Vector2 curr = Target;
        while (found && currMove<50)
        {
            path[currMove] = findMin(curr, field);
            path[currMove].z = 1;
            curr = path[currMove];
            if (curr == current) found = false;
            currMove++;
        }
        /*
        //change obstacle field
        tileWalkable[currI, currJ] = true;
        characters[currI, currJ] = free;
        tileWalkable[(int)Math.Floor(Target.x), (int)Math.Floor(Target.y)] = false;
        characters[(int)Math.Floor(Target.x), (int)Math.Floor(Target.y)] = recievedCharacter;*/


        return path;
    }

    
}
