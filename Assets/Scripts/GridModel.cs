using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class GridModel : MonoBehaviour
{
    public bool[,] tileWalkable;
    public Character[] charList;
    public LayerMask whatStopsMovement;
    private Character obstacle;
    private Character free;
    public Character DemonMage;
    public Character Amy;
    public Character Altarez;
    public GridModel OtherGrid;
    public GameObject ActionSelector;

    public new CameraController camera;
    private MyNode root;
    private MyNode currNode;

    public float timer;
    public bool simulation, doSimul, simulating;
    private bool doingSelection, begin;
    int simCounter;

    public float timeSpeed;

    // Start is called before the first frame update
    void Start()
    {
        timeSpeed = 1;
        timer = 0;
        doingSelection = false;
        begin = true;
        root = new MyNode("action", false, false);
        currNode = root;
        doSimul = false;
        tileWalkable = new bool[50, 50];
        charList = new Character[10];

        //Set value of obstacles to false
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                if (Physics2D.OverlapCircle(new Vector2(i + .5f, j + .5f), .05f, LayerMask.GetMask("CollideLayer", "Friendlies", "Enemies")))
                {
                    tileWalkable[i, j] = false;
                }
                else
                {
                    tileWalkable[i, j] = true;
                }                
            }
        }

        createStartCharacters();
    }

    // Update is called once per frame
    void Update()
    {
        Time.timeScale = timeSpeed;
        if (!simulation && doSimul)
        {
            doSimul = false;
            if (!OtherGrid.simulating) //start simulation
            {
                Debug.Log("starting simulation");
                OtherGrid.simulating = true;
                OtherGrid.timer = 10 * Time.timeScale;
            }
        }

        if (simulation && simulating)
        {
            if (begin)
            {
                copyOriginal();
                currNode = root;
				doingSelection = true;
                begin = false;
            }
            if (canProceed())
            {
                currNode = nextStep(currNode);
                actionToCharacters(currNode);
            }
            if(timer <= 0)
            {
                OtherGrid.actionToCharacters(bestChild(root));
                Debug.Log("Selected node for passing: " + bestChild(root).getData());
                simulating = false;
            }
        }

        if (timer > 0) timer -= Math.Abs(Time.deltaTime);
    }

    private bool canProceed()
    {
        bool ret = true;
        for (int i = 0; i < 4; i++)
        {
            Character ch = charList[i];
            if (ch.canProceed() == false)
            {
                ret = false;
            }
            
        }
        return ret;
    }


    private void createStartCharacters()
    {
        if (simulation)
        {
            charList[0] = Instantiate(Amy, new Vector3(124, 25, 1), new Quaternion(0, 0, 0, 0));
            charList[1] = Instantiate(Altarez, new Vector3(123, 26, 1), new Quaternion(0, 0, 0, 0));
            charList[2] = Instantiate(DemonMage, new Vector3(126, 25, 1), new Quaternion(0, 0, 0, 0));
            charList[3] = Instantiate(DemonMage, new Vector3(127, 24, 1), new Quaternion(0, 0, 0, 0));

            charList[0].simChar = true;
            charList[1].simChar = true;
            charList[2].simChar = true;
            charList[3].simChar = true;
        }
        else
        {
            charList[0] = Instantiate(Amy, new Vector3(24, 25, 1), new Quaternion(0, 0, 0, 0));
            charList[1] = Instantiate(Altarez, new Vector3(23, 26, 1), new Quaternion(0, 0, 0, 0));
            charList[2] = Instantiate(DemonMage, new Vector3(26, 25, 1), new Quaternion(0, 0, 0, 0));
            charList[3] = Instantiate(DemonMage, new Vector3(27, 24, 1), new Quaternion(0, 0, 0, 0));

            charList[0].simChar = false;
            charList[1].simChar = false;
            charList[2].simChar = false;
            charList[3].simChar = false;

            camera.FollowTarget = charList[0];
        }

        for(int i = 0; i < 4; i++)
        {
            charList[i].script = this;
        }

        tileWalkable[24,25] = false;
        tileWalkable[23,26] = false;
        tileWalkable[26,25] = false;
        tileWalkable[27,24] = false;
    }

    //Player's attacking functions
    /*
    public Character checkEnemy(GameObject importedGO, Vector2 direction, int range)
    {
        int currI = (int)Math.Truncate(importedGO.transform.position.x) % 100;
        int currJ = (int)Math.Truncate(importedGO.transform.position.y);

        Character ch = null;

        for (int i = 1; i < range; i++)
        {
            if (direction.x == 1)
            {
                ch = checkEnemyInPosition(importedGO, new Vector2(currI + i, currJ));
            }

            if (direction.x == -1)
            {
                ch =  checkEnemyInPosition(importedGO, new Vector2(currI - i, currJ));
            }

            if (direction.y == 1)
            {
                ch =  checkEnemyInPosition(importedGO, new Vector2(currI, currJ + i));
            }

            if (direction.y == -1)
            {
                ch =  checkEnemyInPosition(importedGO, new Vector2(currI, currJ - i));
            }

            if(ch != null) break;
        }

        return ch;
    }
    */

public Character checkEnemyInPosition(GameObject go, Vector2 position) //ch returns self for some reason
    {
        int layer = go.layer;
        Character ch = null;
        for(int i = 0; i < 4; i++){
            Character cha = charList[i];
            /*Debug.Log("\ncha.transform.position = " + cha.transform.position.x + "," + cha.transform.position.y +
            "\nposition = " + position.x + "," + position.y);*/
            if((cha.transform.position.x-position.x) == 0 && (cha.transform.position.y-position.y) == 0){
                ch = cha;
            }
        }
        if(ch == null) 
        {
            //Debug.Log("no character in position " + (int)Math.Truncate(position.x) + "," + (int)Math.Truncate(position.y));
            return null;
        }
        //Debug.Log("layer is " + layer + ", ch.layer is " + ch.layer + " in position " + position.x + position.y);
        if(layer == 10) //friendly
        {
            if(ch.layer == 9) return ch;
        }
        else            //enemy
        {
            if(ch.layer == 10) return ch;
        }
        return null;
    }

    public void attackEnemy(int damage, Character target)
    {
        target.addHP(-1 * damage);
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
        field[(int)Math.Truncate(currList[0].x), (int)Math.Truncate(currList[0].y)] = 1;

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
                    i = (int)Math.Truncate(element.x);
                    j = (int)Math.Truncate(element.y);
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
        if (min == -1) min = 100;

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

        Vector2 current = new Vector2(recievedCharacter.transform.position.x, recievedCharacter.transform.position.y);
        Vector3[] path = new Vector3[50];
        int[,] field = new int[50, 50];
        int currI = (int)current.x % 100;
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

        //return null if target is an obstacle
        if (field[(int)Target.x, (int)Target.y] == -1) { /*Debug.Log("error: invalid target location: " + (int)Target.x + "," + (int)Target.y);*/ return null;}

        //Fill in field values
        fillFieldValues(field, currI, currJ);

        field[currI, currJ] = 1;

        //show generated field
        String deb = "";
        for(int i = 0; i < 50; i++)
        {
            for (int j = 49; j >= 0; j--)
            {
                deb += field[j, i];
                deb += ' ';
            }
            deb += "#";
        }
        //Debug.Log(deb);


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
            if (((int)curr.x % 100) == ((int)current.x % 100) && ((int)curr.y % 100) == ((int)current.y) % 100) { found = true; }
            currMove++;
        }
        
        //change obstacle field
        tileWalkable[currI, currJ] = true;
        tileWalkable[(int)Math.Truncate(Target.x), (int)Math.Truncate(Target.y)] = false;

        if (recievedCharacter.simChar == true)
        {
            for (int i = 0; i < currMove; i++)
            {
                path[i].x += 100;
            }
        }

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
            if (this.childNodes == null) this.childNodes = new List<MyNode>();
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

        public String getData()
        {
            String ret = "";
            if (nodeAction != null)
                ret += ("action: (" + nodeAction + ")");
            ret += ", enemyAction, simNode: (" + enemyAction + "," + simNode + ")";
            ret += ", " + "wins, visits: (" + wins + "," + visits + ")";
            if (parent != null)
                ret += ", parents action: (" + parent.nodeAction + ")";
            ret += ", fullyExpanded: (" + fullyExpanded + ")";
            if (childNodes != null && childNodes.Count > 0)
            {
                ret += ", childnodes actions: ";
                foreach(MyNode node in childNodes)
                {
                    ret += (node.nodeAction + ", ");
                }
                ret = ret.Remove(ret.Length - 3, 3);
            }   
            return ret;
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
        int counter = 5;
        Debug.Log("treesearch");
        while (timer > 0 && counter > 0)
        {
            copyOriginal();
            MyNode leaf = selection(root);
            MyNode sim_res = simulate(leaf);
            backPropogate(sim_res,sim_res.wins != 0);
            timer -= Time.deltaTime;
            counter--;
        }
        //return root;
        return bestChild(root);
    }

    private MyNode selection(MyNode node)
    {
        //Debug.Log("selection start: " + node.getData());
        if (node.childNodes != null && node.childNodes.Count > 0)
        {
            while (node != null && node.childNodes != null && node.childNodes.Count > 0)
            {
                //Debug.Log("selected action: " + node.getData());
                node = selectChild(node);
                actionToCharacters(node);
            }
        }
        node = makeRandomChild(node,true);
        //Debug.Log("selection ended: " + node.getData());
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
            Debug.Log("simulated node: " + node.getData());
        }
        return node;
    }

    private MyNode nextStep(MyNode node)
    {
        MyNode ret = root;
        if (doingSelection)
        {
            if (node.childNodes != null && node.childNodes.Count > 0)
            {
                ret = selectChild(node);
                Debug.Log("selected action: " + node.getData());
            }
            else
            {
                ret = makeRandomChild(node, true);
                Debug.Log("selection ended, created child: " + node.getData());
                doingSelection = false;
                simCounter = 10;
            }
            actionToCharacters(ret);
        }
        else if(simCounter > 0)
        {
            ret = makeRandomChild(node, false);
            Debug.Log("simulated node: " + node.getData());
            simCounter--;
            actionToCharacters(ret);
        }
        else
        {
            backPropogate(currNode,enemyWins());
            begin = true;
            doingSelection = true;
        }
        return ret;
    }

    private bool enemyWins()
    {
        int goodHealth = charList[0].health + charList[1].health;
        int badHealth = 0;
        for(int i = 2; i < 4; i++)
        {
            badHealth += charList[i].health;
        }
        //Debug.Log("badhealth = " + badHealth + ", goodhealth = " + goodHealth);
        return badHealth > goodHealth;
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
        //Debug.Log("node children:");
        if(node != null)
        foreach (MyNode child in node.childNodes)
        {
            //Debug.Log(child.nodeAction);
            if (child.visits > 0 && node.visits > 0)
            {
                //Debug.Log("child.wins: " + child.wins + ",child.visits: " + child.visits + ",c: " + c + ",node.visits: " + node.visits);
                childValue = child.wins / child.visits + c * Math.Sqrt(Math.Log(node.visits) / child.visits) + 0;
            }
            else childValue = 0;
            //Debug.Log("childValue: " + childValue + ", highest: " + highest);
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
                if(this.charList[i] != null)
                {
                    this.charList[i].setAttrTo(OtherGrid.charList[i]);
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
        }
        else
        {
            actionString += convertActionToString(randomAction(charList[2]));
            actionString += convertActionToString(randomAction(charList[3]));
        }
        MyNode ret = new MyNode(actionString, !node.enemyAction, true);
        //Debug.Log("made random node: " + ret.nodeAction);
        return ret;
    }
    
    private MyNode makeRandomChild(MyNode node, bool expansion)
    {
        if (expansion)
        {
            MyNode expChild = makeRandomNode(node);
            expChild.simNode = false;
            node.addChildNode(expChild);
            return expChild;
        }

        MyNode child = makeRandomNode(node);
        bool exists = false;
        if(node.childNodes != null && node.childNodes.Count > 0)
        foreach(MyNode ch in node.childNodes)
        {
            if (child.nodeAction.Equals(ch.nodeAction))
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

    private string convertActionToString(int[] action)
    {
        if (action.Length == 0) return "default";
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

        //Debug.Log("checking for walkability: ||| x: " + ch.transform.position.x + "+" + movement.x + ", y: " + ch.transform.position.y + "+" + movement.y);

        if (tileWalkable[(int)Math.Truncate(ch.transform.position.x % 100 + movement.x) , (int)Math.Truncate(ch.transform.position.y + movement.y)])
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
		if(node == null) {
			Debug.Log("no node");
			return;
		}
		if(node.nodeAction.Equals("action")){
			Debug.Log("Action is action");
			return;
		}
        String[] charActions = node.nodeAction.Split('&');
        //foreach (String str in charActions) if(str != null) Debug.Log("Action is " + str);
        if (node.enemyAction)
        {
            for (int i = 2; i < 4; i++)
            {
                charList[i].doActions(convertStringToAction(charActions[i - 2]));
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                charList[i].doActions(convertStringToAction(charActions[i]));
            }
        }
    }
}
