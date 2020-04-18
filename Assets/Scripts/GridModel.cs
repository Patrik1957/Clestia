using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GridModel : MonoBehaviour
{
    public bool[,] tileWalkable;
    public Character[,] characters;
    public Character[] charList;
    public LayerMask whatStopsMovement;
    private Character obstacle;
    private Character free;
    public Character DemonMage;
    public Character Knob;
    public Character Checkmark;
    public Character Amy;
    public Character Altarez;
    public GridModel OtherGrid;
    public GameObject ActionSelector;

    public float timer;
    public bool simulation;

    public bool simul,simulating;

    // Start is called before the first frame update
    void Start()
    {
        timer = 5;
        simul = false;
        tileWalkable = new bool[50, 50];
        characters = new Character[50, 50];
        charList = new Character[10];
        if (!simulation)
        {
            obstacle = Instantiate(Knob, new Vector3(-1, -1, 1), new Quaternion(0, 0, 0, 0));
            free = Instantiate(Checkmark, new Vector3(-1, -1, 1), new Quaternion(0, 0, 0, 0));
            //ActionSelector = Instantiate(ActionSelector, new Vector3(0, 0, 1), new Quaternion(0, 0, 0, 0));
            //OtherGrid = GameObject.Find("SimGrid");
        }
        else
        {
            //OtherGrid = GameObject.Find("Grid");
        }

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

        createStartCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        if (!simulation && simul)
        {
            simul = false;
            if (!simulating) //start simulation
            {
                Debug.Log("starting simulation");
                Debug.Break();
                MyNode result;
                result = OtherGrid.treeSearch();
                //Debug.Log(result.nodeAction);
            }
            else            //stop simulation
            {

            }
        }
    }

    private void createStartCharacters()
    {
        if (simulation)
        {
            characters[26, 25] = Instantiate(DemonMage, new Vector3(126, 25, 1), new Quaternion(0, 0, 0, 0));
            characters[27, 24] = Instantiate(DemonMage, new Vector3(127, 24, 1), new Quaternion(0, 0, 0, 0));
            characters[24, 25] = Instantiate(Amy, new Vector3(124, 25, 1), new Quaternion(0, 0, 0, 0));
            characters[23, 26] = Instantiate(Altarez, new Vector3(123, 26, 1), new Quaternion(0, 0, 0, 0));
        }
        else
        {
            characters[26, 25] = Instantiate(DemonMage, new Vector3(26, 25, 1), new Quaternion(0, 0, 0, 0));
            characters[27, 24] = Instantiate(DemonMage, new Vector3(27, 24, 1), new Quaternion(0, 0, 0, 0));
            characters[24, 25] = Instantiate(Amy, new Vector3(24, 25, 1), new Quaternion(0, 0, 0, 0));
            characters[23, 26] = Instantiate(Altarez, new Vector3(23, 26, 1), new Quaternion(0, 0, 0, 0));
        }
        charList[0] = characters[24, 25];
        charList[1] = characters[23, 26];
        charList[2] = characters[26, 25];
        charList[3] = characters[27, 24];
    }

    //Player's attacking functions
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

        Debug.Log("Target not found");

        return null;
    }

    public Character checkTarget(GameObject importedGO, Vector2 direction, int range)
    {
        int currI = (int)Math.Floor(importedGO.transform.position.x);
        int currJ = (int)Math.Floor(importedGO.transform.position.y);

        Debug.Log("Locating Target");

        for (int i = 0; i < range; i++)
        {
            if (characters[currI + i * (int)Math.Floor(direction.x), currJ + i * (int)Math.Floor(direction.y)] != null)
            {
                return (characters[currI + i * (int)Math.Floor(direction.x), currJ + i * (int)Math.Floor(direction.y)]);
            }
        }

        Debug.Log("Target not found");

        return null;
    }

    public void attackEnemy(Character attacker, Character target)
    {
        /*
        Character att = characters[(int)Math.Ceiling(attacker.transform.position.x), (int)Math.Ceiling(attacker.transform.position.y)];
        Character targ = characters[(int)Math.Floor(target.transform.position.x), (int)Math.Floor(target.transform.position.y)];
        */
        target.addHP(-1 * attacker.damage);
    }

    //Pathfinding functions
    private void fillFieldValues(int[,] field, int ii, int jj)
    {
        //variable definitions
        int currMove = 2, n, i, j;
        Vector2[] nextList, currList;
        bool keepGoing = true;

        //set starting position data
        currList = new Vector2[1];
        currList[0] = new Vector2(ii, jj);
        field[(int)Math.Floor(currList[0].x), (int)Math.Floor(currList[0].y)] = 1;

        //iterate through elements of currList
        //add an increasing currMove value to the neighbors of the elements 
        //store the elements in nextMove
        //replace currList with the previous elements using nextList
        //repeat until no more neighbors are found, the entire field is filled
        while(keepGoing)
        {
            nextList = new Vector2[150];
            keepGoing = false;
            n = 0;
            foreach(Vector2 element in currList)
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
        Target.x = Target.x % 100;
        Target.y = Target.y % 100;

        Vector2 current = new Vector2(recievedCharacter.transform.position.x, recievedCharacter.transform.position.y);
        bool found = false;
        Vector3[] path = new Vector3[50];
        int[,] field = new int[50, 50];
        int currI = (int)current.x % 100;
        int currJ = (int)current.y % 100;

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
        fillFieldValues(field, currI, currJ);

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



    //Node class and functions for MCTS

    public class MyNode
    {
        public string nodeAction;
        public int wins;
        public int visits;
        public List<MyNode> childNodes;
        public MyNode parent;
        public bool fullyExpanded;
        public bool enemyAction;
        public bool simNode;

        public MyNode(string _nodeAction, bool _enemyAction, bool _simNode,
            int _wins = 0, int _visits = 0, List<MyNode> _childNodes = null, MyNode _parent = null, bool _fullyExpanded = false)
        {
            nodeAction = _nodeAction;
            enemyAction = _enemyAction;
            simNode = _simNode;
            wins = _wins;
            visits = _visits;
            fullyExpanded = _fullyExpanded;
            this.setChildNodes(_childNodes);
            this.setParent(_parent);
            childNodes = new List<MyNode>();
        }

        public void setChildNodes(List<MyNode> childNodes)
        {
            if (childNodes != null)
            {
                this.childNodes = childNodes;
                foreach (MyNode child in childNodes)
                {
                    child.parent = this;
                }
            }
            
        }

        public void addChildNode(MyNode child)
        {
            this.childNodes.Add(child);
            child.parent = this;
        }

        public void setParent(MyNode parentNode)
        {
            if (parentNode != null && parentNode.childNodes != null)
            {
                this.parent = parentNode;
                parentNode.childNodes.Add(this);
            }
        }
    }

    //Monte Carlo Tree Search functions

    public MyNode treeSearch()
    {
        MyNode root = new MyNode("action", true, false);
        while (timer > 0)
        {
            MyNode leaf = selection(root);
            MyNode sim_res = simulate(leaf);
            backprop(leaf, sim_res);
            timer -= Time.deltaTime;
        }
        return root;
        return bestChild(root);
    }

    private MyNode selection(MyNode node)
    {
        Debug.Log("selection start: " + node.nodeAction);
        if (node.childNodes != null && node.childNodes.Count > 0)
        {
            Debug.Log("node has child");
            while (node != null && node.childNodes != null && node.childNodes.Count > 0)
            {//Debug.Log("selected action: " + node.nodeAction);
                node = selectChild(node);
            }
        }
        Debug.Log("selection ended: " + node.nodeAction);
        return node; //or pick_unvisited(node.children) ???
    }

    private MyNode simulate(MyNode node)
    {
        int counter = 5;
        copyOriginal();
        while(node != null && node.visits != 1 && counter>0)
        {
            MyNode child = pickRandomChild(node);
            node.addChildNode(child);
            node = child;
            counter--;
        }
        return node;
    }

    private void backprop(MyNode node, MyNode sim_res)
    {
        if (node != null)
        {
            if (node.parent is null) return;
            if (sim_res != null && sim_res.wins == 1) node.wins += 1;
            node.visits += 1;
            backprop(node.parent, sim_res);
        }
    }

    private MyNode bestChild(MyNode node)
    {
        int highest = 0;
        MyNode retNode = null;
        //pick child with highest number of visits
        if(node != null)
        foreach(MyNode child in node.childNodes)
        {
            if(child.visits > highest)
            {
                retNode = child;
                highest = child.visits;
            }
        }
        return retNode;
    }

    private MyNode selectChild(MyNode node)
    {
        ///////////////////////////////////////constant for formula///////////////////////////////////////////////////////////
        double c = Math.Sqrt(2);
        double highest = -1;
        MyNode retNode = null;
        double childValue;
        //pick child with best result from formula
        if(node != null)
        foreach (MyNode child in node.childNodes)
        {
            if (child.visits > 0)
            {
                childValue = child.wins / child.visits + c * Math.Sqrt(Math.Log(node.visits) / child.visits);
            }
            else childValue = 0;
            if(childValue > highest)
            {
                retNode = child;
                highest = childValue;
            }
        }
        return retNode;
    }

    private void copyOriginal()
    {
        if (simulation)
        {
            for (int i = 0; i<10; i++)
            {
                if(charList[i] != null)
                {
                    charList[i].setAttrTo(OtherGrid.charList[i]);
                }
            }
        }
    }

    ////////////////////////////////////////////////////////////////Simulation to be done here///////////////////////////
    private MyNode pickRandomChild(MyNode node)
    {
        int[] action;
        string actionString = "";
        bool plA;
        
        if (!node.enemyAction)
        {
            action = randomPlayerAction();
            execAction(action, true);
        }
        else
        {
            action = randomEnemiesAction();
            execAction(action, false);
        }
        actionString = convertActionToString(action);
        MyNode ret = new MyNode(actionString, !node.enemyAction, true);
        Debug.Log("picked child action: " + ret.nodeAction);
        return ret;
    }

    private void execAction(int[] action, bool playerAction)
    {
        if (playerAction)
        {
            switch (action[0])
            {
                case (1):
                    charList[0].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[0].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[0].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[0].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[1])
            {
                case (1):
                    charList[0].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[0].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[0].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[0].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[2])
            {
                case (1):
                    charList[0].readyAttack();
                    break;
                case (2):
                    charList[0].readySpell1();
                    break;
                case (3):
                    charList[0].readySpell2();
                    break;
            }
            switch (action[3])
            {
                case (1):
                    charList[1].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[1].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[1].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[1].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[4])
            {
                case (1):
                    charList[1].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[1].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[1].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[1].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[5])
            {
                case (1):
                    charList[1].attackRandomly();
                    break;
                case (2):
                    charList[1].spell1Randomly();
                    break;
                case (3):
                    charList[1].spell2Randomly();
                    break;
            }
        }
        else
        {
            switch (action[0])
            {
                case (1):
                    charList[2].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[2].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[2].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[2].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[1])
            {
                case (1):
                    charList[2].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[2].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[2].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[2].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[2])
            {
                case (1):
                    charList[2].attackRandomly();
                    break;
                case (2):
                    charList[2].spell1Randomly();
                    break;
                case (3):
                    charList[2].spell2Randomly();
                    break;
            }
            switch (action[3])
            {
                case (1):
                    charList[3].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[3].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[3].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[3].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[4])
            {
                case (1):
                    charList[3].addMoveTo(0, 1, 0);
                    break;
                case (2):
                    charList[3].addMoveTo(1, 0, 0);
                    break;
                case (3):
                    charList[3].addMoveTo(0, -1, 0);
                    break;
                case (4):
                    charList[1].addMoveTo(-1, 0, 0);
                    break;
            }
            switch (action[5])
            {
                case (1):
                    charList[3].attackRandomly();
                    break;
                case (2):
                    charList[3].spell1Randomly();
                    break;
                case (3):
                    charList[3].spell2Randomly();
                    break;
            }
        }
    }
    
    private string convertActionToString(int[] action)
    {
        if (action.Length == 0) return "default";
        Debug.Log(action[0]);
        string ret = "";
        foreach(int element in action)
        {
            switch (element)
            {
                case (1):
                    ret += "up";
                    break;
                case (2):
                    ret += "right";
                    break;
                case (3):
                    ret += "down";
                    break;
                case (4):
                    ret += "left";
                    break;
                case (5):
                    ret += "attack";
                    break;
                case (6):
                    ret += "spell1";
                    break;
                case (7):
                    ret += "spell2";
                    break;
            }
        }
        Debug.Log("actionstring: " + ret);
        return ret;
    }

    private int[] randomPlayerAction()
    {
        int[] ret = new int[6];

        int[] amyAction = randomAction();
        int[] altarezAction = randomAction();
        
        ret[0] = amyAction[0];
        ret[1] = amyAction[1];
        ret[2] = amyAction[2];
        ret[3] = altarezAction[0];
        ret[4] = altarezAction[1];
        ret[5] = altarezAction[2];
        return ret;
    }

    private int[] randomEnemiesAction()
    {   int[] ret = new int[6];

        int[] dm1Action = randomAction();
        int[] dm2Action = randomAction();
        
        ret[0] = dm1Action[0];
        ret[1] = dm1Action[1];
        ret[2] = dm1Action[2];
        ret[3] = dm2Action[0];
        ret[4] = dm2Action[1];
        ret[5] = dm2Action[2];
        return ret;
    }

    private int[] randomAction()
    {
        System.Random rnd = new System.Random();
        int move1 = rnd.Next(1, 5);
        int move2 = rnd.Next(1, 5);
        int action = rnd.Next(5, 8);
        int[] ret = new int[3];
        ret[0] = move1;
        ret[1] = move2;
        ret[2] = action;
        Debug.Log("random: " + ret[0]);
        return ret;
    }
}
