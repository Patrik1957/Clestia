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
    public bool simulation, simul, simulating;

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
                if (Physics2D.OverlapCircle(new Vector2(i + .5f, j + .5f), .05f, LayerMask.GetMask("CollideLayer", "Friendlies", "Enemies")))
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
        int currX = (int)curr.x % 100;
        int currY = (int)curr.y % 100;
        int min = field[currX, currY];
        int retX = currX, retY = currY;

        /*
        Debug.Log("field in current: " + field[currX, currY]
            + ", field above: " + field[currX, currY + 1] + ", field below: " + field[currX, currY - 1]
            + ", field to the right: " + field[currX + 1, currY] + ", field to the left: " + field[currX - 1, currY]);
        */

        if (field[currX + 1, currY] < min && field[currX + 1, currY] != -1)
        {
            min = field[currX + 1, currY];
            retX = currX + 1;
            retY = currY;
        }
        if (field[currX, currY + 1] < min && field[currX, currY + 1] != -1)
        {
            min = field[currX, currY + 1];
            retX = currX;
            retY = currY + 1;
        }
        if (field[currX - 1, currY] < min && field[currX - 1, currY] != -1)
        {
            min = field[currX - 1, currY];
            retX = currX - 1;
            retY = currY;
        }
        if (field[currX, currY - 1] < min && field[currX, currY - 1] != -1)
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

        //return null if target is an obstacle
        if (field[(int)Target.x, (int)Target.y] == -1) { Debug.Log("error: invalid target location: " + (int)Target.x + "," + (int)Target.y); }

        //Fill in field values
        fillFieldValues(field, currI, currJ);

        field[currI, currJ] = 1;
        //show generated field
        String deb = "";
        for(int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                deb += field[j, i];
                deb += ' ';
            }
            deb += "#";
        }
        Debug.Log(deb);


        //Determine path from current to target tile
        bool found = false;
        int currMove = 1;
        path[0] = new Vector3((int)Target.x, (int)Target.y, 1);
        Vector2 curr = Target;
        while (!found && currMove<50)
        {
            path[currMove] = findMin(curr, field);
            path[currMove].z = 1;
            curr = path[currMove];
            //Debug.Log("curr: " + curr.x + "," + curr.y + ", target: " + (current.x % 100) + "," + current.y);
            if (((int)curr.x % 100) == ((int)current.x % 100) && ((int)curr.y % 100) == ((int)current.y) % 100) { found = true;  Debug.Log("match"); }
            currMove++;
        }
        
        //change obstacle field
        tileWalkable[currI, currJ] = true;
        characters[currI, currJ] = free;
        tileWalkable[(int)Math.Floor(Target.x), (int)Math.Floor(Target.y)] = false;
        characters[(int)Math.Floor(Target.x), (int)Math.Floor(Target.y)] = recievedCharacter;


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

        public void Destroy()
        {
            this.parent = null;
            this.childNodes = null;
        }

        /*
        public int[] actionInts()
        {
            String charActions = nodeAction.Split("&");
            String indActions = charActions.Split("#");
            
        }*/
    }

    //Monte Carlo Tree Search functions

    public MyNode treeSearch()
    {
        Debug.Log("treesearch");
        MyNode root = new MyNode("action", true, false);
        while (timer > 0)
        {
            copyOriginal();
            MyNode leaf = selection(root);
            MyNode sim_res = simulate(leaf);
            backPropogate(sim_res,sim_res.wins != 0);
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
            while (node != null && node.childNodes != null && node.childNodes.Count > 0)
            {//Debug.Log("selected action: " + node.nodeAction);
                node = selectChild(node);
                actionToCharacters(node);
            }
        }
        node = makeRandomChild(node,true);
        Debug.Log("selection ended: " + node.nodeAction);
        return node; //or pick_unvisited(node.children) ???
    }

    private MyNode simulate(MyNode node)
    {
        int counter = 5;
        while(node != null && node.visits != 1 && counter>0)
        {
            MyNode child = makeRandomChild(node,false);
            node = child;
            counter--;
        }
        return node;
    }

    private void backPropogate(MyNode node, bool win)
    {
        if (node != null)
        {
            if (node.parent is null) return;
            if (win) node.wins += 1;
            node.visits += 1;
            MyNode parent = node.parent;
            if(node.simNode)
                node.Destroy();
            backPropogate(parent, win);
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
        double childValue = 0;
        //pick child with best result from formula
        Debug.Log("node children:");
        if(node != null)
        foreach (MyNode child in node.childNodes)
        {
            Debug.Log(child.nodeAction);
            if (child.visits > 0 && node.visits > 0)
            {
                //Debug.Log("child.wins: " + child.wins + ",child.visits: " + child.visits + ",c: " + c + ",node.visits: " + node.visits);
                childValue = child.wins / child.visits + c * Math.Sqrt(Math.Log(node.visits) / child.visits) + 0;
            }
            else childValue = 0;
            Debug.Log("childValue: " + childValue + ", highest: " + highest);
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
    private MyNode makeRandomNode(MyNode node)
    {
        string actionString = "";
        
        if (!node.enemyAction)
        {
            actionString += convertActionToString(randomAction(charList[0]));
            actionString += convertActionToString(randomAction(charList[1]));
            //execAction(action, true);
        }
        else
        {
            actionString += convertActionToString(randomAction(charList[2]));
            actionString += convertActionToString(randomAction(charList[3]));
            //execAction(action, false);
        }
        //actionString = convertActionToString(action);
        MyNode ret = new MyNode(actionString, !node.enemyAction, true);
        Debug.Log("made random node: " + ret.nodeAction);
        return ret;
    }
    
    private MyNode makeRandomChild(MyNode node, bool expansion)
    {
        if (expansion)
        {
            MyNode expChild = makeRandomNode(node);
            node.addChildNode(expChild);
            return expChild;
        }

        MyNode child = makeRandomNode(node);
        bool exists = false;
        if(node.childNodes != null && node.childNodes.Count > 0)
        foreach(MyNode ch in node.childNodes)
        {
            if (child.nodeAction == ch.nodeAction)
            {
                exists = true;
                child = ch;
                break;
            }
        }
        if (!exists)
            node.addChildNode(child);
        return child;
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
                    charList[0].attackRandomly();
                    break;
                case (2):
                    charList[0].spell1Randomly();
                    break;
                case (3):
                    charList[0].spell2Randomly();
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
        //Debug.Log(action[0]);
        string ret = "";
        foreach(int element in action)
        {
            switch (element)
            {
                case (1):
                    ret += "up#";
                    break;
                case (2):
                    ret += "right#";
                    break;
                case (3):
                    ret += "down#";
                    break;
                case (4):
                    ret += "left#";
                    break;
                case (5):
                    ret += "attack&";
                    break;
                case (6):
                    ret += "spell1&";
                    break;
                case (7):
                    ret += "spell2&";
                    break;
            }
        }
        return ret;
    }

     private int[] convertStringToAction(string action)
    {
        if (action.Length == 0) return null;
        String[] actions = action.Split('#');
        int n = actions.GetLength(0);
        int[] ret = new int[n];
        int i = 0;
        foreach(String element in actions)
        {
            switch (element)
            {
                case ("up"):
                    ret[i] = 1;
                    i++;
                    break;
                case ("right"):
                    ret[i] = 2;
                    i++;
                    break;
                case ("down"):
                    ret[i] = 3;
                    i++;
                    break;
                case ("left"):
                    ret[i] = 4;
                    i++;
                    break;
                case ("attack"):
                    ret[i] = 5;
                    i++;
                    break;
                case ("spell1"):
                    ret[i] = 6;
                    i++;
                    break;
                case ("spell2"):
                    ret[i] = 7;
                    i++;
                    break;
            }
        }
        return ret;
    }

    /*
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
    */
    private int[] randomAction(Character ch)
    {
        bool good = false;
        int move1, move2, action;
        int[] ret = new int[3];
        while(good == false)
        {
            System.Random rnd = new System.Random();
            move1 = UnityEngine.Random.Range(1, 5);
            move2 = UnityEngine.Random.Range(1, 5);
            action = UnityEngine.Random.Range(5, 8);
            ret[0] = move1;
            ret[1] = move2;
            ret[2] = action;
            ch.actions = ret;
            good = checkAction(ch, ret);
        }
        return ret;
    }

    private bool checkAction(Character ch, int[] ret)
    {
        Vector3 movement = new Vector3(0, 0, 0);

        foreach (int element in ret)
        {
            switch (element)
            {
                case (1):
                    movement += new Vector3(0, 1, 0);
                    break;
                case (2):
                    movement += new Vector3(1, 0, 0);
                    break;
                case (3):
                    movement += new Vector3(0, -1, 0);
                    break;
                case (4):
                    movement += new Vector3(-1, 0, 0);
                    break;
            }
        }

        //Debug.Log("x: " + (int)Math.Floor(ch.transform.position.x + movement.x) % 100 + "y: " + (int)Math.Floor(ch.transform.position.y + movement.y) % 100);

        if (tileWalkable[(int)Math.Floor(ch.transform.position.x + movement.x) % 100, (int)Math.Floor(ch.transform.position.y + movement.y) % 100])
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void actionToCharacters(MyNode node)
    {
        String[] charActions = node.nodeAction.Split('&');
        foreach (String str in charActions) Debug.Log("Action is " + str);
        if (node.enemyAction)
        {
            for (int i = 2; i < 4; i++)
            {
                charList[i].doActions(convertStringToAction(charActions[i]));
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                charList[i].doActions(convertStringToAction(charActions[i]));
            }
        }

        for(int i = 0; i < 4; i++)
        {
            Character ch = charList[i];
            while (ch.moving || ch.attacking)
            {
                Debug.Log("waiting");
            }
        }
    }
}
