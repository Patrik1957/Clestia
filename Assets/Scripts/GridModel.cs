using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridModel : MonoBehaviour
{
    public bool[,] tileWalkable;
    public Character[] charList;
    public LayerMask whatStopsMovement;
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

    public Projectile arrowDown,arrowLeft,arrowUp,arrowRight,tornado;
    public Projectile fireBallDown,fireBallLeft,fireBallUp,fireBallRight;
    public GameObject earthspikes,icespikes,demonspikes,fireblast,icetacle,torrentacle,lightning,shield,snake;

    public GameObject dmgTxt;
    private bool selectionEnded;

    public bool spell1Used, spell2Used;

    public Animator actionAnim;

    public int whoseTurn;
    public int steps;
    public int actionLeft;
    public int commandLeft;

    private int spedUp;

    // Start is called before the first frame update
    void Start()
    {
        spedUp = 100;
        whoseTurn = 0;
        steps = 2;
        actionLeft = 1;
        commandLeft = 1;

        spell1Used = false; 
        spell2Used = false;
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
        if(!simulation){
            if(whoseTurn == 0 && !OtherGrid.simulating){
                if(charList[0].selectedAction == 0){
                    if(Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d")){
                        steps--;
                    }
                }
                if(charList[0].selectedAction == 1 || charList[0].selectedAction == 2 || charList[0].selectedAction == 3){
                    if(Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d")){
                        actionLeft--;
                    }
                }
                if(charList[0].selectedAction == 4){
                    if(Input.GetKeyDown("w") || Input.GetKeyDown("a") || Input.GetKeyDown("s") || Input.GetKeyDown("d")){
                        commandLeft--;
                    }
                }
                if(steps<1 && commandLeft<1 && actionLeft<1){
                    steps = 3; commandLeft = 2; actionLeft = 2; whoseTurn = 1;
                }
            }

            if(whoseTurn == 1 && charList[1].actions[2] == 0 && charList[1].readyAction == 0 && charList[1].canProceed()) whoseTurn = 2;

            if(whoseTurn == 2 && !OtherGrid.simulating){
                doSimul = true;
                whoseTurn = 0;
            }
        }


        actionAnim.SetFloat("action", charList[0].selectedAction + 1);
        /*
        for(int i=0; i<4; i++){
            if(charList[i] != null && charList[i].health < 1){
                Debug.Log("position is " + charList[i].transform.position.x + "," + charList[i].transform.position.y +
                "\ntruncated is " + (int)Math.Truncate(charList[i].transform.position.x) + "," + (int)Math.Truncate(charList[i].transform.position.y));
                tileWalkable[(int)Math.Truncate(charList[i].transform.position.x),(int)Math.Truncate(charList[i].transform.position.y)] = true;
                //charList[i] = null;
            }
        }
        */
        if(OtherGrid.spell1Used) spell1Used = true;
        if(OtherGrid.spell2Used) spell2Used = true;
        Time.timeScale = timeSpeed;
        if (!simulation && doSimul)
        {
            doSimul = false;
            if (!OtherGrid.simulating) //start simulation
            {
                Debug.Log("starting simulation");
                OtherGrid.simulating = true;
                OtherGrid.timer = 10 * Time.timeScale;
                timeSpeed = spedUp;
                OtherGrid.timeSpeed = spedUp;
            }
        }

        if (simulation && simulating)
        {
            if (begin)
            {
                copyOriginal();
                selectionEnded = false;
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
                root.destroyTree();
                simulating = false;
                timeSpeed = 1;
                OtherGrid.timeSpeed = 1;
                begin = true;
            }
        }

        /*
        for(int i=0; i<4; i++){
            if(charList[i].gameObject.activeSelf){
                if(tileWalkable[(int)Math.Truncate(charList[i].transform.position.x) % 100,(int)Math.Truncate(charList[i].transform.position.y)])
                    Debug.Log("set field " + (int)Math.Truncate(charList[i].transform.position.x % 100) + "'" +  (int)Math.Truncate(charList[i].transform.position.y) + " to not walkable");
                tileWalkable[(int)Math.Truncate(charList[i].transform.position.x) % 100,(int)Math.Truncate(charList[i].transform.position.y)] = false;
                charList[i].gameObject.SetActive(true);
            }
            else{
                if(!tileWalkable[(int)Math.Truncate(charList[i].transform.position.x) % 100,(int)Math.Truncate(charList[i].transform.position.y)])
                    Debug.Log("set field " + (int)Math.Truncate(charList[i].transform.position.x % 100) + "'" +  (int)Math.Truncate(charList[i].transform.position.y) + " to walkable");
                tileWalkable[(int)Math.Truncate(charList[i].transform.position.x % 100),(int)Math.Truncate(charList[i].transform.position.y)] = true;
                charList[i].gameObject.SetActive(false);                
            }
        }*/

        if (timer > 0) timer -= Math.Abs(Time.deltaTime) / Time.timeScale;
    }

    private bool canProceed()
    {
        bool ret = true;
        for (int i = 0; i < 4; i++)
        {
            if(charList[i].gameObject.activeSelf){
                Character ch = charList[i];
                if (ch.canProceed() == false)
                {
                    ret = false;
                }
            }

            
        }
        return ret;
    }


    private void createStartCharacters()
    {
        if (simulation)
        {
            charList[0] = Instantiate(Amy, new Vector3(124, 25, 1), Quaternion.identity);
            charList[1] = Instantiate(Altarez, new Vector3(123, 26, 1), Quaternion.identity);
            charList[2] = Instantiate(DemonMage, new Vector3(126, 25, 1), Quaternion.identity);
            charList[3] = Instantiate(DemonMage, new Vector3(127, 24, 1), Quaternion.identity);

            charList[0].simChar = true;
            charList[1].simChar = true;
            charList[2].simChar = true;
            charList[3].simChar = true;
        }
        else
        {
            charList[0] = Instantiate(Amy, new Vector3(24, 25, 1), Quaternion.identity);
            charList[1] = Instantiate(Altarez, new Vector3(23, 26, 1), Quaternion.identity);
            charList[2] = Instantiate(DemonMage, new Vector3(26, 25, 1), Quaternion.identity);
            charList[3] = Instantiate(DemonMage, new Vector3(27, 24, 1), Quaternion.identity);

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


public Character checkEnemyInPosition(int layer, Vector2 position)
    {
        Character ch = null;
        for(int i = 0; i < 4; i++){
            if(charList[i].gameObject.activeSelf){
                Character cha = charList[i];
                /*Debug.Log("\ncha.transform.position = " + cha.transform.position.x + "," + cha.transform.position.y +
                "\nposition = " + position.x + "," + position.y);*/
                if((cha.transform.position.x-position.x) == 0 && (cha.transform.position.y-position.y) == 0){
                    ch = cha;                
                }
            }
        }
        if(ch == null) 
        {
            //Debug.Log("no character in position " + (int)Math.Truncate(position.x) + "," + (int)Math.Truncate(position.y));
            return null;
        }
        //Debug.Log("layer is " + layer + ", ch.layer is " + ch.layer + " in position " + position.x + "," + position.y);
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
        damage *= -1;
        target.addHP(damage);
        target.transform.GetChild(0).localScale = new Vector3(target.health * 0.6f,10,1);
        GameObject dmg = Instantiate(dmgTxt, new Vector3(target.transform.position.x + .5f, target.transform.position.y + 1.15f, -0.5f), Quaternion.identity);
        dmg.GetComponent<TextMesh>().text = damage.ToString();

        if(damage > 0) dmg.GetComponent<TextMesh>().color = Color.green;
        else dmg.GetComponent<TextMesh>().color = Color.yellow;

        Destroy(dmg,1);
        dmg = null;
        
        if(target.health < 1){
            tileWalkable[(int)Math.Truncate(target.transform.position.x) % 100,(int)Math.Truncate(target.transform.position.y)] = true;
            for(int i=0; i<4; i++){
                if(charList[i] != null && charList[i] == target){
                    charList[i].gameObject.SetActive(false);
                }
            }
        }
        
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
        for (int i = 1; i < 49; i++)
        {
            for (int j = 1; j < 49; j++)
            {
                if (!tileWalkable[i, j])
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
        //Print field
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

        public void destroyTree(){
            if(this.childNodes != null && this.childNodes.Count > 0){
                foreach(MyNode node in this.childNodes){
                    node.destroyTree();
                }
            }
            this.Destroy();
        }

        /*
        public int[] actionInts()
        {
            String charActions = nodeAction.Split("&");
            String indActions = charActions.Split("#");
            
        }*/
    }

    //Monte Carlo Tree Search functions
/*
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
*/

    private MyNode nextStep(MyNode node)
    {
        MyNode ret = root;
        if (doingSelection)
        {
            if (node.childNodes != null && node.childNodes.Count > 0 && !selectionEnded)
            {
                ret = selectChild(node);
                if(ret == node) selectionEnded = true;
                Debug.Log("selected node: " + node.getData());
            }
            else
            {
                ret = makeRandomChild(node, true);
                Debug.Log("selection ended, created child: " + ret.getData());
                doingSelection = false;
                //simCounter = 10;
            }
            actionToCharacters(ret);
        }
        else if((charList[0].gameObject.activeSelf || charList[1].gameObject.activeSelf) && (charList[2].gameObject.activeSelf || charList[3].gameObject.activeSelf))
        {
            ret = makeRandomChild(node, false);
            Debug.Log("simulated node: " + ret.getData());
            //simCounter--;
            actionToCharacters(ret);
        }
        else
        {
            Debug.Log("someone dead");
            backPropogate(currNode,enemyWins());
            begin = true;
            doingSelection = true;
        }
        return ret;
    }

    private bool enemyWins()
    {
        int goodHealth = 0, badHealth = 0;;
        if(charList[0].gameObject.activeSelf) goodHealth += charList[0].health; 
        if(charList[1].gameObject.activeSelf) goodHealth += charList[1].health;
        if(charList[2].gameObject.activeSelf) badHealth += charList[2].health;
        if(charList[3].gameObject.activeSelf) badHealth += charList[3].health;
        //Debug.Log("badhealth = " + badHealth + ", goodhealth = " + goodHealth);
        return badHealth > goodHealth;
    }

    private void backPropogate(MyNode node, bool win)
    {
        if (node != null)
        {
            if (win) node.wins += 1;
            node.visits += 1;
            if(!node.simNode) Debug.Log("backpropogated node: " + node.getData());
            MyNode parent = node.parent;
            if(node.simNode)
                node.Destroy();
            if (parent is null) return;
            //if(!node.simNode) Debug.Log("backprop parent");
            backPropogate(parent, win);
        }
    }

    private MyNode bestChild(MyNode node)
    {
        int highest = 0;
        MyNode retNode = node;
        //pick child with highest number of visits
        if(node != null && node.childNodes != null && node.childNodes.Count > 0)
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
        MyNode retNode = node;
        double value;
        if(node.visits != 0)
            if(node.parent != null)
                value = node.wins / node.visits + c * Math.Sqrt(Math.Log(node.parent.visits) / node.visits);
            else
                value = node.wins / node.visits;
        //pick child with best result from formula
        //Debug.Log("node children:");
        if(node != null)
        foreach (MyNode child in node.childNodes)
        {
            //Debug.Log(child.nodeAction);
            if (child.visits > 0 && node.visits > 0)
            {
                //Debug.Log("child.wins: " + child.wins + ",child.visits: " + child.visits + ",c: " + c + ",node.visits: " + node.visits);
                value = child.wins / child.visits + c * Math.Sqrt(Math.Log(node.visits) / child.visits);
            }
            else value = 0;
            //Debug.Log("value: " + value + ", highest: " + highest);
            if(value > highest)
            {
                retNode = child;
                highest = value;
            }
        }
        return retNode;
    }

    private void copyOriginal()
    {
        if (simulation)
        {
            for (int i = 0; i<4; i++)
            {
                if(OtherGrid.charList[i].gameObject.activeSelf)
                {
                    this.charList[i].gameObject.SetActive(true);
                    this.charList[i].setAttrTo(OtherGrid.charList[i]);
                }
                else this.charList[i].gameObject.SetActive(false);
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
        //Debug.Log("converting action: " + action);
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
        int move = 0;
        List<int> usedMoves = new List<int>();
        int[] ret = new int[3];

        while(!good){
            System.Random rnd = new System.Random();
            while(usedMoves.Contains(move)){
                move = UnityEngine.Random.Range(1, 9);
                if(usedMoves.Count == 8) move = 10;
            }
            usedMoves.Add(move);
            ret = rndToAction(move);
            good = checkAction(ch, ret);            
        }
        if(usedMoves.Count == 8) {ret[0] = 2; ret[1] = 4;}
        /*
        int counter = 10;
        while(!good && counter>0)
        {
            System.Random rnd = new System.Random();
            move1 = UnityEngine.Random.Range(1, 5);
            move2 = UnityEngine.Random.Range(1, 5);
            action = UnityEngine.Random.Range(5, 8);
            ret[0] = move1;
            ret[1] = move2;
            ret[2] = action;

            good = checkAction(ch, ret);
            if(!good){
                Debug.Log("original action: " + convertActionToString(ret));
                ret[0] = (ret[0] + 2) % 5; if(ret[0]<2) ret[0]++;
                ret[1] = (ret[1] + 2) % 5; if(ret[1]<2) ret[1]++;
                ret[2] = action;
                Debug.Log("new action: " + convertActionToString(ret));
                good = checkAction(ch, ret);
            }
            counter--;
        }
        */
        //ch.actions = ret;
        Debug.Log("found position");
        if(!good) {Debug.Log("ranout"); Debug.Break();}
        return ret;
    }

    private int[] rndToAction (int move){
        int[] ret = new int[3];
        switch(move){
            case 1:
                ret[0] = 1; ret[1] = 1;
                break;
            case 2:
                ret[0] = 1; ret[1] = 2;
                break;
            case 3:
                ret[0] = 2; ret[1] = 2;
                break;
            case 4:
                ret[0] = 2; ret[1] = 3;
                break;
            case 5:
                ret[0] = 3; ret[1] = 3;
                break;
            case 6:
                ret[0] = 3; ret[1] = 4;
                break;
            case 7:
                ret[0] = 4; ret[1] = 4;
                break;
            case 8:
                ret[0] = 4; ret[1] = 1;
                break;
            default:
                ret[0] = 4; ret[1] = 2;
                break;
        }
        ret[2] = 6;
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

        float charPosX = ch.transform.position.x % 100;
        float charPosY = ch.transform.position.y;
        int checkX = (int)Math.Truncate(charPosX + movement.x);
        int checkY = (int)Math.Truncate(charPosY + movement.y);

        Debug.Log("checking position " + checkX + "," + checkY);

        //check for each other character, if any of them is more than 7 tiles away, and the action would move them even farther apart, return false
        for(int i=0; i<4; i++){
            if(charList[i].gameObject.activeSelf){
                float iPosX = charList[i].transform.position.x % 100;
                float iPosY = charList[i].transform.position.y;

                if((Math.Abs(iPosX - charPosX) > 7 || Math.Abs(iPosY - charPosY) > 7) && 
                    Math.Abs(iPosX - charPosX) < Math.Abs(iPosX - checkX) || Math.Abs(iPosY - charPosY) < Math.Abs(iPosY - checkY)
                ) return false;                
            }
        }
        if (tileWalkable[checkX , checkY])
        {
            for(int i=0; i<4;i++){
                if(charList[i].gameObject.activeSelf && charList[i] != ch && charList[i].moveTo == new Vector3(checkX,checkY,1)){
                    return false;
                }
            }
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
			//Debug.Log("Action is action");
			return;
		}
        String[] charActions = node.nodeAction.Split('&');
        //foreach (String str in charActions) if(str != null) Debug.Log("Action is " + str);
        if (node.enemyAction)
        {
            if(charList[2].gameObject.activeSelf) charList[2].doActions(convertStringToAction(charActions[0]));
            if(charList[3].gameObject.activeSelf) charList[3].doActions(convertStringToAction(charActions[1]));
        }
        else
        {
            if(charList[0].gameObject.activeSelf) charList[0].doActions(convertStringToAction(charActions[0]));
            if(charList[1].gameObject.activeSelf) charList[1].doActions(convertStringToAction(charActions[1]));
        }
    }

    public GameObject makeSpell(String str, Vector3 pos){
        pos += new Vector3(0,0,1);
        GameObject ret = null;
        switch(str){
            case "fireblast":
                ret = Instantiate(fireblast, pos, Quaternion.identity);
                break;
            case "icetacle":
                ret = Instantiate(icetacle, pos, Quaternion.identity);
                break;
            case "torrentacle":
                ret = Instantiate(torrentacle, pos, Quaternion.identity);
                break;
            case "lightning":
                ret = Instantiate(lightning, pos, Quaternion.identity);
                break;
            case "shield":
                ret = Instantiate(shield, pos, Quaternion.identity);
                break;
            case "snake":
                ret = Instantiate(snake, pos, Quaternion.identity);
                break;     
            case "demonspikes":
                ret = Instantiate(demonspikes, pos, Quaternion.identity);
                break;    
            case "earthspikes":
                ret = Instantiate(earthspikes, pos, Quaternion.identity);
                break;                
            
        }
        Destroy(ret,1);
        return ret;
    }


        public Projectile makeProjectile(Character origin, Character target, String type, int dmg, float speed){
        Projectile proj = null;
        float startX = origin.transform.position.x;
        float startY = origin.transform.position.y;        
        float endX = target.transform.position.x;
        float endY = target.transform.position.y;
        
        if(startY == endY){
            if(startX > endX){
                if(type.Equals("arrow"))
                    proj = Instantiate(arrowLeft, new Vector3(startX,startY,2), Quaternion.identity);
                 if(type.Equals("fireball"))
                    proj = Instantiate(fireBallLeft, new Vector3(startX,startY,2), Quaternion.identity);                   
            }
            else {
                if(type.Equals("arrow"))
                    proj = Instantiate(arrowRight, new Vector3(startX,startY,2), Quaternion.identity);
                 if(type.Equals("fireball"))
                    proj = Instantiate(fireBallRight, new Vector3(startX,startY,2), Quaternion.identity); 
            }
        }
        else{
            if(startY > endY){
                if(type.Equals("arrow"))
                    proj = Instantiate(arrowDown, new Vector3(startX,startY,2), Quaternion.identity);
                 if(type.Equals("fireball"))
                    proj = Instantiate(fireBallDown, new Vector3(startX,startY,2), Quaternion.identity); 
            }
            else {
                if(type.Equals("arrow"))
                    proj = Instantiate(arrowUp, new Vector3(startX,startY,2), Quaternion.identity);
                 if(type.Equals("fireball"))
                    proj = Instantiate(fireBallUp, new Vector3(startX,startY,2), Quaternion.identity); 
            }
        }

        if(type.Equals("tornado"))
            proj = Instantiate(tornado, new Vector3(startX,startY,2), Quaternion.identity);  

        proj.changeSpeed(speed);
        proj.changeTargetChar(target);
        proj.changeDmg(dmg);  

        return proj;
    }

    public void controlAltarez(float h, float v){
        if(!charList[1].gameObject.activeSelf) return;
        if(h>.5f){
            Character ch = getClosestEnemy();
            moveAltarezTowardsCharacter(ch);
        }
        if(h<-.5f){
            if(charList[0].gameObject.activeSelf) moveAltarezTowardsCharacter(charList[0]);
        }
        if(v>.5f){
            int[] noaction = null; noaction[0] = 0; noaction[1] = 0; noaction[2] = 5;
            charList[1].actions = noaction;
        }
        if(v<-.5f){
            System.Random rnd = new System.Random();
            int random1 = rnd.Next(0, 5);
            int random2 = rnd.Next(0, 5);
            int random3 = rnd.Next(5, 8);
            int[] action = new int[] { random1, random2, random3 };
            charList[1].actions = action;
        }
    }

    private Character getClosestEnemy(){
        if(!charList[2].gameObject.activeSelf && charList[3].gameObject.activeSelf) return charList[3];
        if(!charList[3].gameObject.activeSelf && charList[2].gameObject.activeSelf) return charList[2];
        double diff1 = Math.Sqrt(Math.Pow(charList[2].transform.position.x - charList[1].transform.position.x,2) - Math.Pow(charList[2].transform.position.y - charList[1].transform.position.y,2));
        double diff2 = Math.Sqrt(Math.Pow(charList[3].transform.position.x - charList[1].transform.position.x,2) - Math.Pow(charList[3].transform.position.y - charList[1].transform.position.y,2));
        if(diff1 > diff2) return charList[2];
        else return charList[3];
    }

    private void moveAltarezTowardsCharacter(Character ch){
        Debug.Log("moveAltarez to character in " + ch.transform.position.x + "," + ch.transform.position.y);
        //do nothing if already next to character
        if(Math.Abs(charList[1].transform.position.x - ch.transform.position.x) < 2 && Math.Abs(charList[1].transform.position.y - ch.transform.position.y) < 2) return;
        
        int[] action = new int[3];

        float diffX = Math.Abs(charList[1].transform.position.x - ch.transform.position.x);
        float diffY = Math.Abs(charList[1].transform.position.y - ch.transform.position.y);

        //if the distance is greater in X, move along X, otherwise move along Y
        //if it's equal, move along both X and Y
        if(diffX > diffY){
            Debug.Log("x>y");
            if(charList[1].transform.position.x < ch.transform.position.x)
                action[0] = 2;
            else
                action[0] = 4;
        }

        if(diffX < diffY){
            Debug.Log("x<y");
            if(charList[1].transform.position.y < ch.transform.position.y)
                action[0] = 1;
            else
                action[0] = 3;
        }

        if(diffX == diffY) {
            Debug.Log("x=y");
            if(charList[1].transform.position.x < ch.transform.position.x)
                action[0] = 2;
            else
                action[0] = 4;

            if(charList[1].transform.position.y < ch.transform.position.y)
                action[1] = 1;
            else
                action[1] = 3;
        }
        else action[1] = action[0];
        
        System.Random rnd = new System.Random();
        int random = rnd.Next(5, 8);
        action[2] = random;

        Debug.Log("action[0] is " + action[0]);
        charList[1].actions = action;
    }
}
