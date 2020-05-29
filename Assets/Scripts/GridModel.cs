using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Threading;

public class GridModel : MonoBehaviour
{
    public bool[,] tileWalkable;
    public Character[] charList;
    public Character DemonMage;
    public Character Amy;
    public Character Altarez;
    public GridModel OtherGrid;

    public new CameraController camera;
    private static MyNode root;
    private static MyNode currNode;

    public float timer;
    public bool simulation, doSimul, simulating;
    private bool doingSelection, selectionEnded, begin;

    public float timeSpeed;

    public Projectile arrowDown, arrowLeft, arrowUp, arrowRight, tornado;
    public Projectile fireBallDown, fireBallLeft, fireBallUp, fireBallRight;
    public GameObject earthspikes, icespikes, demonspikes, fireblast, icetacle, torrentacle, lightning, shield, snake;

    public GameObject dmgTxt;

    public bool spell1Used, spell2Used;

    public Animator actionAnim;

    public int whoseTurn;
    public int stepsLeft, actionLeft, commandLeft;
    public int spedUp;

    public static System.Random rnd;
    public int seed;

    public int divider;

    public GameObject victory,defeat;
    public bool end;

    public GameObject hourglass;

    private bool doEnemyActions;
    private MyNode enemyActionSave;

    private int simCounter;

    void Awake()
    {
        enemyActionSave = null;
        doEnemyActions = false;
        hourglass.SetActive(false);
        end = false;
        divider = 300;
        System.Random rndGen = new System.Random();
        seed = rndGen.Next();
        OtherGrid.seed = this.seed;
        rnd = new System.Random(seed);

        spedUp = 100;
        whoseTurn = 0;
        stepsLeft = 2;
        actionLeft = 1;
        commandLeft = 1;

        spell1Used = false;
        spell2Used = false;
        timeSpeed = 1;
        timer = 0;
        doingSelection = false;
        begin = true;
        if(simulation) {
            root = new MyNode("action", false, false);
            currNode = root;
        }
        doSimul = false;
        tileWalkable = new bool[50, 50];
        charList = new Character[10];

        String map = "";
        //Set value of obstacles to false
        for (int i = 0; i < 50; i++)
        {
            for (int j = 0; j < 50; j++)
            {
                if (Physics2D.OverlapCircle(new Vector2(i + .5f, j + .5f), .05f, LayerMask.GetMask("CollideLayer", "Friendlies", "Enemies")))
                {
                    tileWalkable[i, j] = false;
                    map += "0 ";        
                }
                else
                {
                    tileWalkable[i, j] = true;
                    map += "1 ";   
                }
            }
            map += "\n";
        }

        createStartCharacters();
    }

void Update(){

    if(!simulation && Input.GetKeyDown("i")){
        OtherGrid.doTesting();
    }

        if (!simulation && doSimul && charList[0].canProceed() && charList[1].canProceed())
        {
            hourglass.SetActive(true);
            doSimul = false;
            if (!OtherGrid.simulating) //start simulation
            {
                //Debug.Log("starting simulation");
                OtherGrid.simulating = true;
                OtherGrid.timer = 200;
                timeSpeed = spedUp;
                OtherGrid.timeSpeed = spedUp;
            }
        }

        if(simulation && doEnemyActions) {
            //Debug.Log("do node: " + enemyActionSave.getData());
            timeSpeed = OtherGrid.timeSpeed = Time.timeScale = 1;

            OtherGrid.actionToCharacters(enemyActionSave);

            doEnemyActions = false;

        }

        if(simulation) return;

        //if(Input.GetKeyDown("u") && !script.simulation) Debug.Log(GridModel.MyNode.printAllNodes());

        if(Input.GetKeyDown("t")){
            if(camera.FollowTarget == charList[0]) camera.FollowTarget = OtherGrid.charList[0];
            else camera.FollowTarget = charList[0];
        }
        if(Input.GetKeyDown("r")){
            seed = 100;
            OtherGrid.seed = seed;
            GridModel.rnd = new System.Random(seed);
        }
        if(Input.GetKeyDown("g") && !simulation){
            if(Time.timeScale > 1){
                timeSpeed = 1;
                OtherGrid.timeSpeed = 1;
                Time.timeScale = 1; 
                for(int i=0;i<4;i++){
                    OtherGrid.charList[i].gameObject.transform.position = OtherGrid.charList[i].moveTo;
                }               
            }
            else {
                timeSpeed = spedUp;
                timeSpeed = spedUp;  
                Time.timeScale = spedUp;
            }
        } 
        if(Input.GetKeyDown("escape")) Application.Quit(); 

        if(whoseTurn != 0 || OtherGrid.simulating || end) return;

        bool w = Input.GetKeyDown("w");
        bool a = Input.GetKeyDown("a");
        bool s = Input.GetKeyDown("s");
        bool d = Input.GetKeyDown("d");
        float v = 0, h = 0;
        if (w) v = 1;
        if (s) v = -1;
        if (a) h = -1;
        if (d) h = 1;

        bool q = Input.GetKeyDown("q");
        bool e = Input.GetKeyDown("e");
        if (q && !charList[0].simChar) {charList[0].selectedAction--; /*Debug.Log("minus");*/}
        if (e && !charList[0].simChar) {charList[0].selectedAction++; /*Debug.Log("plus");*/}
        
        if(charList[0].selectedAction == 5) charList[0].selectedAction = 0;
        if(charList[0].selectedAction == -1) charList[0].selectedAction = 4;
        actionAnim.SetFloat("action", charList[0].selectedAction + 1);

        //if there is keyboard input in a direction, check for selected action and execute it in given direction
        if (!charList[0].moving && (Math.Abs(h) > 0.5f || Math.Abs(v) > 0.5f))
        {
            switch (charList[0].selectedAction)
            {
                case 0:
                    if(stepsLeft < 1) break;
                    charList[0].moveTo += new Vector3(h, v, 0);
                    stepsLeft--;
                    break;
                case 1:
                    if(actionLeft < 1) break;
                    charList[0].attackDir(h, v);
                    actionLeft--;
                    break;
                case 2:
                    if(actionLeft < 1) break;
                    charList[0].spell1Dir(h, v);
                    actionLeft--;
                    break;
                case 3:
                    if(actionLeft < 1) break;
                    charList[0].spell2Dir(h, v);
                    actionLeft--;
                    break;
                case 4:
                    if(commandLeft < 1) break;
                    controlAltarez(h, v);
                    commandLeft--;
                    break;
            }
        }
        
    }

    void FixedUpdate()
    {
        Time.timeScale = timeSpeed;
        if(end) {
            timeSpeed = OtherGrid.timeSpeed = Time.timeScale = 0;
            return;
        }
        if(!simulation && charList[2].health<1 && charList[3].health<1 && charList[0].canProceed() && charList[1].canProceed() && charList[2].canProceed() && charList[3].canProceed()) {
            Instantiate(victory, new Vector3(camera.gameObject.transform.position.x,camera.gameObject.transform.position.y,-5), Quaternion.identity);
            end = true;
        }
        if(!simulation && charList[0].health<1 && charList[0].canProceed() && charList[1].canProceed() && charList[2].canProceed() && charList[3].canProceed()){
            Instantiate(defeat, new Vector3(camera.gameObject.transform.position.x,camera.gameObject.transform.position.y,-5), Quaternion.identity);
            end = true;           
        }
        
        if(Time.timeScale > 2 && simulation) {
            if(divider>0){
                divider--;
                return;
            }
            else divider = 300;            
        }

        if (!simulation)
        {
            if (whoseTurn == 0 && !OtherGrid.simulating)
            {
                if (stepsLeft < 1 && commandLeft < 1 && actionLeft < 1)
                {
                    stepsLeft = 2; commandLeft = 1; actionLeft = 1;
                    whoseTurn = 1;
                }
            }

            if (whoseTurn == 1 && charList[1].actions[2] == 0 && charList[1].readyAction == 0 && charList[0].canProceed() && charList[1].canProceed()) whoseTurn = 2;

            if (whoseTurn == 2 && !OtherGrid.simulating)
            {
                doSimul = true;
                whoseTurn = 0;
            }
        }

        if (OtherGrid.spell1Used) spell1Used = true;
        if (OtherGrid.spell2Used) spell2Used = true;


        if (simulation && simulating)
        {
            if (begin)
            {
                copyOriginal();
                for(int i=0; i<4; i++){
                    charList[i].moveTo = charList[i].gameObject.transform.position;
                }
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
            if(root.visits > 10 || timer < 1)
            {
                Time.timeScale = timeSpeed = OtherGrid.timeSpeed = 1;
                Debug.Log("Selected node for passing: " + bestChild(root).getData());
                enemyActionSave = new MyNode(bestChild(root).nodeAction, true, false);
                //OtherGrid.actionToCharacters(bestChild(root));
                Debug.Log(MyNode.printAllNodes());
                root.destroyTree();
                root = new MyNode("action", false, false);
                currNode = root;
                simulating = false;
                begin = true;
                //Debug.Log("all nodes: \n" + getChildren(root));
                hourglass.SetActive(false);
                doEnemyActions = true;
            }
        }

        if (timer > 0) timer -= Time.timeScale/100;
    }

    private string getChildren(MyNode node){
        string ret = node.getData() + "\n";
        if(node.childNodes!=null && node.childNodes.Count>0){
            foreach(MyNode child in node.childNodes){
                ret += getChildren(child);
            }
        }
        return ret;
    }

    private bool canProceed()
    {
        bool ret = true;
        for (int i = 0; i < 4; i++)
        {
            if (charList[i].gameObject.activeSelf)
            {
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
            charList[0] = Instantiate(Amy, new Vector3(123, 24, 1), Quaternion.identity);
            charList[1] = Instantiate(Altarez, new Vector3(123, 26, 1), Quaternion.identity);
            charList[2] = Instantiate(DemonMage, new Vector3(125, 26, 1), Quaternion.identity);
            charList[3] = Instantiate(DemonMage, new Vector3(125, 24, 1), Quaternion.identity);

            charList[0].simChar = true;
            charList[1].simChar = true;
            charList[2].simChar = true;
            charList[3].simChar = true;
        }
        else
        {
            charList[0] = Instantiate(Amy, new Vector3(23, 24, 1), Quaternion.identity);
            charList[1] = Instantiate(Altarez, new Vector3(23, 26, 1), Quaternion.identity);
            charList[2] = Instantiate(DemonMage, new Vector3(25, 26, 1), Quaternion.identity);
            charList[3] = Instantiate(DemonMage, new Vector3(25, 24, 1), Quaternion.identity);

            charList[0].simChar = false;
            charList[1].simChar = false;
            charList[2].simChar = false;
            charList[3].simChar = false;

            camera.FollowTarget = charList[0];
        }

        for (int i = 0; i < 4; i++)
        {
            charList[i].script = this;
        }

        tileWalkable[23, 24] = false;
        tileWalkable[23, 26] = false;
        tileWalkable[25, 26] = false;
        tileWalkable[25, 24] = false;
    }


    public Character checkEnemyInPosition(int layer, Vector2 position)
    {
        Character ch = null;
        for (int i = 0; i < 4; i++)
        {
            if (charList[i].gameObject.activeSelf)
            {
                Character cha = charList[i];
                if ((cha.transform.position.x - position.x) == 0 && (cha.transform.position.y - position.y) == 0)
                {
                    ch = cha;
                }
            }
        }
        if (ch == null)
        {
            //Debug.Log("no character in position " + (int)Math.Truncate(position.x) + "," + (int)Math.Truncate(position.y));
            return null;
        }
        //Debug.Log("layer is " + layer + ", ch.layer is " + ch.layer + " in position " + position.x + "," + position.y);
        if (layer == 10) //friendly
        {
            if (ch.layer == 9) return ch;
        }
        else            //enemy
        {
            if (ch.layer == 10) return ch;
        }
        return null;
    }

    public void attackEnemy(int damage, Character target)
    {
        damage *= -1;
        target.addHP(damage);
        if(target.health>0) target.transform.GetChild(0).localScale = new Vector3(target.health * 0.6f, 10, 1);
        else target.transform.GetChild(0).localScale = new Vector3(0, 10, 1);
        GameObject dmg = Instantiate(dmgTxt, new Vector3(target.transform.position.x + .5f, target.transform.position.y + 1.15f, -0.5f), Quaternion.identity);
        dmg.GetComponent<TextMesh>().text = damage.ToString();

        if (damage > 0) dmg.GetComponent<TextMesh>().color = Color.green;
        else dmg.GetComponent<TextMesh>().color = Color.yellow;

        Destroy(dmg, 1);
        dmg = null;

        if (target.health < 1)
        {
            tileWalkable[(int)Math.Truncate(target.transform.position.x) % 100, (int)Math.Truncate(target.transform.position.y)] = true;
            for (int i = 0; i < 4; i++)
            {
                if (charList[i] != null && charList[i] == target)
                {
                    charList[i].gameObject.SetActive(false);
                }
            }
        }

    }

    //Pathfinding functions
    private void fillFieldValues(int[,] field, int x, int y)
    {
        //variable definitions
        int currMove = 2, n, i, j;
        Vector2[] nextList, currList;
        bool keepGoing = true;

        //set starting position data
        currList = new Vector2[1];
        currList[0] = new Vector2(x, y);
        field[(int)Math.Truncate(currList[0].x), (int)Math.Truncate(currList[0].y)] = 1;

        //iterate through elements of currList
        //add an increasing currMove value to the neighbors of the elements 
        //store the elements in nextMove
        //replace currList with the previous elements using nextList
        //repeat until no more neighbors are found, the entire field is filled
        while (keepGoing)
        {
            nextList = new Vector2[150];
            keepGoing = false;
            n = 0;
            foreach (Vector2 element in currList)
            {
                if (element.x != 0 && element.y != 0)
                {
                    i = (int)Math.Truncate(element.x);
                    j = (int)Math.Truncate(element.y);
                    if (field[i + 1, j] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i + 1, j] = currMove;
                        nextList[n] = new Vector2(i + 1, j); n++;
                        keepGoing = true;
                    }
                    if (field[i - 1, j] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i - 1, j] = currMove;
                        nextList[n] = new Vector2(i - 1, j); n++;
                        keepGoing = true;
                    }
                    if (field[i, j + 1] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i, j + 1] = currMove;
                        nextList[n] = new Vector2(i, j + 1); n++;
                        keepGoing = true;
                    }
                    if (field[i, j - 1] == 0 && i > 2 && i < 48 && j > 2 && j < 48)
                    {
                        field[i, j - 1] = currMove;
                        nextList[n] = new Vector2(i, j - 1); n++;
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
        if (field[(int)Target.x, (int)Target.y] == -1) { /*Debug.Log("error: invalid target location: " + (int)Target.x + "," + (int)Target.y);*/ return null; }

        //Fill in field values
        fillFieldValues(field, currI, currJ);

        field[currI, currJ] = 1;

        //show generated field
        String deb = "";
        for (int i = 0; i < 50; i++)
        {
            for (int j = 49; j >= 0; j--)
            {
                deb += field[j, i];
                deb += ' ';
            }
            deb += "#";
        }
        //Print field
        //Debug.Log(deb);


        //Determine path from current to target tile
        bool found = false;
        int currMove = 1;
        path[0] = new Vector3((int)Target.x, (int)Target.y, 1);
        Vector2 curr = Target;
        while (!found && currMove < 50)
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
        public bool enemyAction;
        public bool simNode;

        public static List<MyNode> allNodes = new List<MyNode>();

        public MyNode(string _nodeAction, bool _enemyAction, bool _simNode,
            int _wins = 0, int _visits = 0, List<MyNode> _childNodes = null, MyNode _parent = null)
        {
            nodeAction = _nodeAction;
            enemyAction = _enemyAction;
            simNode = _simNode;
            wins = _wins;
            visits = _visits;
            this.setChildNodes(_childNodes);
            this.setParent(_parent);
            childNodes = new List<MyNode>();
            allNodes.Add(this);
        }

        public static String printAllNodes(){
            String ret = "Printing nodes\n";
            foreach(MyNode node in allNodes){
                ret += node.getData() + "\n";
            }

            return ret;
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
            allNodes.Remove(this);
            this.parent.childNodes.Remove(this);
            this.parent = null;
            if(childNodes != null && childNodes.Count>0) foreach(MyNode node in childNodes) node.parent = null;
            this.childNodes = null;
        }

        public String getData()
        {
            if(this == null) return "null";
            String ret = "";
            if (nodeAction != null)
                ret += ("action: (" + nodeAction + ")");
            ret += ", enemyAction, simNode: (" + enemyAction + "," + simNode + ")";
            ret += ", " + "wins, visits: (" + wins + "," + visits + ")";
            if (parent != null)
                ret += ", parents action: (" + parent.nodeAction + ")";
            if (childNodes != null && childNodes.Count > 0)
            {
                ret += ", childnodes actions: ";
                foreach (MyNode node in childNodes)
                {
                    ret += (node.nodeAction + ", ");
                }
                ret = ret.Remove(ret.Length - 3, 3);
            }
            return ret;
        }

        public void destroyTree()
        {
            if (this.childNodes != null && this.childNodes.Count > 0)
            {
                foreach (MyNode node in this.childNodes)
                {
                    node.destroyTree();
                }
            }
            MyNode.allNodes.Remove(this);
        }
    }

    private MyNode nextStep(MyNode node)
    {
        //Debug.Log("root visits: " + root.visits);
        MyNode ret = root;
        if (doingSelection)
        {
            if (node.childNodes != null && node.childNodes.Count > 0 && !selectionEnded)
            {
                ret = selectChild(node);
                if (ret == node) selectionEnded = true;
                //Debug.Log("selected node: " + node.getData());
            }
            else
            {
                ret = makeRandomChild(node, true);
                //Debug.Log("selection ended, created child: " + ret.getData());
                doingSelection = false;
                simCounter = 5;
            }
            actionToCharacters(ret);
        }
        else if (simCounter > 0 || ((charList[0].gameObject.activeSelf || charList[1].gameObject.activeSelf) && (charList[2].gameObject.activeSelf || charList[3].gameObject.activeSelf)))
        {
            ret = makeRandomChild(node, false);
            //Debug.Log("simulated node: " + ret.getData());
            simCounter--;
            actionToCharacters(ret);
        }
        else
        {
            backPropogate(currNode, enemyWins());
            begin = true;
            doingSelection = true;
        }
        return ret;
    }

    private bool enemyWins()
    {
        int goodHealth = 0, badHealth = 0;
        if (charList[0].gameObject.activeSelf) goodHealth += charList[0].health;
        if (charList[1].gameObject.activeSelf) goodHealth += charList[1].health;
        if (charList[2].gameObject.activeSelf) badHealth += charList[2].health;
        if (charList[3].gameObject.activeSelf) badHealth += charList[3].health;
        //Debug.Log("badhealth = " + badHealth + ", goodhealth = " + goodHealth);
        return badHealth > goodHealth;
    }

    private void backPropogate(MyNode node, bool win)
    {
        if (node != null)
        {
            if (win) node.wins += 1;
            node.visits += 1;
            //if (!node.simNode) Debug.Log("backpropogated node: " + node.getData());
            MyNode parent = node.parent;
            //if(!node.simNode && parent != null) Debug.Log("backprop parent: " + parent.getData());
            if (node.simNode)
                node.Destroy();
            if (parent is null) return;
            backPropogate(parent, win);
        }
    }

    private MyNode bestChild(MyNode node)
    {
        int highest = 0;
        MyNode retNode = node;
        //pick child with highest number of visits
        if (node != null && node.childNodes != null && node.childNodes.Count > 0)
            foreach (MyNode child in node.childNodes)
            {
                if (child.visits > highest)
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
        double highest;
        MyNode retNode = node;
        double value;
        if (node.visits != 0){
            if (node.parent != null)
                highest = node.wins / node.visits + c * Math.Sqrt(Math.Log(node.parent.visits) / node.visits);
            else
                highest = node.wins / node.visits;            
        }
        else highest = -1;

        //pick child with best result from formula
        //Debug.Log("selection; node children:");
        foreach (MyNode child in node.childNodes)
        {
            //Debug.Log("selection; " + child.nodeAction);
            if (child.visits > 0 && node.visits > 0)
            {
                //Debug.Log("selection; child.wins: " + child.wins + ",child.visits: " + child.visits + ",c: " + c + ",node.visits: " + node.visits);
                value = child.wins / child.visits + c * Math.Sqrt(Math.Log(node.visits) / child.visits);
            }
            else value = 0;
            //Debug.Log("selection; value: " + value + ", highest: " + highest);
            if (value > highest)
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
            for (int i = 0; i < 4; i++)
            {
                if (OtherGrid.charList[i].gameObject.activeSelf)
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

        if (node.enemyAction)
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
        if (node.childNodes != null && node.childNodes.Count > 0)
            foreach (MyNode ch in node.childNodes)
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
        foreach (int element in action)
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
        foreach (String element in actions)
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

   
    private int[] randomAction(Character ch)
    {
        bool good = false;
        bool[] usedMove = new bool[9];
        int[] ret = new int[3];

        int move = 0;
        bool filled = false;
        while (!good && !filled)
        {
            usedMove.Initialize();
            move = rnd.Next(0, 8);
            //Debug.Log("move: " + move);
            while (usedMove[move])
            {
                move = rnd.Next(0, 8);
                //Debug.Log("new move: " + move);
                filled = true;
                for(int i=0;i<8;i++){
                    if(!usedMove[i]) filled = false;
                }
                if (filled) break;
            }
            usedMove[move] = true;
            if(filled) move = 10;
            ret = rndToAction(move);
            good = checkAction(ch, ret);
        }
        if (filled) { ret[0] = 2; ret[1] = 4; }
        return ret;
    }

    private int[] rndToAction(int move)
    {
        int[] ret = new int[3];
        switch (move)
        {
            case 0:
                ret[0] = 4; ret[1] = 1;
                break;
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
            default:
                ret[0] = 4; ret[1] = 2;
                break;
        }
        ret[2] = 6;
        return ret;
    }

    private bool checkAction(Character ch, int[] arr)
    {
        if(!ch.gameObject.activeSelf) return true;
        Vector3 movement = new Vector3(0, 0, 0);

        foreach (int element in arr)
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

        float charPosX = ch.transform.position.x % 100;
        float charPosY = ch.transform.position.y;
        int checkX = (int)Math.Truncate(charPosX + movement.x);
        int checkY = (int)Math.Truncate(charPosY + movement.y);

        //check for each other character, if any of them is more than 7 tiles away, and the action would move them even farther apart, return false
        for (int i = 0; i < 4; i++)
        {
            
            if (charList[i].gameObject.activeSelf)
            {
                float iPosX = charList[i].transform.position.x % 100;
                float iPosY = charList[i].transform.position.y;

                if ((Math.Abs(iPosX - charPosX) > 7 || Math.Abs(iPosY - charPosY) > 7) &&
                    (Math.Abs(iPosX - charPosX) < Math.Abs(iPosX - checkX) || Math.Abs(iPosY - charPosY) < Math.Abs(iPosY - checkY))
                ) {return false;}
            }
            
            /*
            if((Math.Abs(charList[i].transform.position.x % 100 - ch.transform.position.x % 100) > 7 || Math.Abs(charList[i].transform.position.y - ch.transform.position.y) > 7)
                && (Math.Abs(charList[i].transform.position.x % 100 - checkX) > 8 || Math.Abs(charList[i].transform.position.y - checkY) > 8)) return false; 
            */
        }
        if (!tileWalkable[checkX, checkY])
        {
            return false;
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                if (charList[i].gameObject.activeSelf && charList[i] != ch && charList[i].moveTo == new Vector3(checkX, checkY, 1))
                {
                    return false;
                }
            }
            return true;            
        }
    }

    public void actionToCharacters(MyNode node)
    {
        if (node == null)
        {
            //Debug.Log("no node");
            return;
        }
        if (node.nodeAction.Equals("action"))
        {
            //Debug.Log("Action is action");
            return;
        }
        String[] charActions = node.nodeAction.Split('&');
        //foreach (String str in charActions) if(str != null) Debug.Log("Action is " + str);
        if (node.enemyAction)
        {
            if (charList[2].gameObject.activeSelf) charList[2].doActions(convertStringToAction(charActions[0]));
            if (charList[3].gameObject.activeSelf) charList[3].doActions(convertStringToAction(charActions[1]));
        }
        else
        {
            if (charList[0].gameObject.activeSelf) charList[0].doActions(convertStringToAction(charActions[0]));
            if (charList[1].gameObject.activeSelf) charList[1].doActions(convertStringToAction(charActions[1]));
        }
    }

    public GameObject makeSpell(String str, Vector3 pos)
    {
        pos += new Vector3(0, 0, -5);
        GameObject ret = null;
        switch (str)
        {
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
            case "snakebite":
                ret = Instantiate(snake, pos, Quaternion.identity);
                break;
            case "demonspikes":
                ret = Instantiate(demonspikes, pos, Quaternion.identity);
                break;
            case "earthspikes":
                ret = Instantiate(earthspikes, pos, Quaternion.identity);
                break;

        }
        Destroy(ret, 1);
        return ret;
    }


    public Projectile makeProjectile(Character origin, Character target, String type, int dmg, float speed)
    {
        Projectile proj = null;
        float startX = origin.transform.position.x;
        float startY = origin.transform.position.y;
        float endX = target.transform.position.x;
        float endY = target.transform.position.y;

        if (startY == endY)
        {
            if (startX > endX)
            {
                if (type.Equals("arrow"))
                    proj = Instantiate(arrowLeft, new Vector3(startX, startY, 2), Quaternion.identity);
                if (type.Equals("fireball"))
                    proj = Instantiate(fireBallLeft, new Vector3(startX + 0.5f, startY, 2), Quaternion.identity);
            }
            else
            {
                if (type.Equals("arrow"))
                    proj = Instantiate(arrowRight, new Vector3(startX, startY, 2), Quaternion.identity);
                if (type.Equals("fireball"))
                    proj = Instantiate(fireBallRight, new Vector3(startX + 0.5f, startY, 2), Quaternion.identity);
            }
        }
        else
        {
            if (startY > endY)
            {
                if (type.Equals("arrow"))
                    proj = Instantiate(arrowDown, new Vector3(startX, startY, 2), Quaternion.identity);
                if (type.Equals("fireball"))
                    proj = Instantiate(fireBallDown, new Vector3(startX, startY + 0.5f, 2), Quaternion.identity);
            }
            else
            {
                if (type.Equals("arrow"))
                    proj = Instantiate(arrowUp, new Vector3(startX, startY, 2), Quaternion.identity);
                if (type.Equals("fireball"))
                    proj = Instantiate(fireBallUp, new Vector3(startX, startY + 0.5f, 2), Quaternion.identity);
            }
        }

        if (type.Equals("tornado"))
            proj = Instantiate(tornado, new Vector3(startX, startY, 2), Quaternion.identity);

        proj.changeSpeed(speed);
        proj.changeTargetChar(target);
        proj.changeDmg(dmg);

        return proj;
    }

    public void controlAltarez(float h, float v)
    {
        //Debug.Log("control Altarez");
        if (!charList[1].gameObject.activeSelf) return;
        if (h > .5f)
        {
            Character ch = getClosestEnemy();
            moveAltarezTowardsCharacter(ch);
        }
        if (h < -.5f)
        {
            if (charList[0].gameObject.activeSelf) moveAltarezTowardsCharacter(charList[0]);
        }
        if (v < -.5f)
        {
            int[] noaction = new int[3]; noaction[0] = 0; noaction[1] = 0; noaction[2] = 5;
            charList[1].doActions(noaction);
        }
        if (v > .5f)
        {
            int[] action = randomAction(charList[1]);
            charList[1].doActions(action);
        }
    }

    private Character getClosestEnemy()
    {
        if (!charList[2].gameObject.activeSelf && charList[3].gameObject.activeSelf) return charList[3];
        if (!charList[3].gameObject.activeSelf && charList[2].gameObject.activeSelf) return charList[2];
        double diff1 = Math.Sqrt(Math.Pow(charList[2].transform.position.x - charList[1].transform.position.x, 2) - Math.Pow(charList[2].transform.position.y - charList[1].transform.position.y, 2));
        double diff2 = Math.Sqrt(Math.Pow(charList[3].transform.position.x - charList[1].transform.position.x, 2) - Math.Pow(charList[3].transform.position.y - charList[1].transform.position.y, 2));
        if (diff1 > diff2) return charList[2];
        else return charList[3];
    }

    private void moveAltarezTowardsCharacter(Character ch)
    {
        //Debug.Log("moveAltarez to character in " + ch.transform.position.x + "," + ch.transform.position.y);
        //do nothing if already next to character
        if (Math.Abs(charList[1].transform.position.x - ch.transform.position.x) <= 1 && Math.Abs(charList[1].transform.position.y - ch.transform.position.y) <= 1) return;

        int[] action = new int[3];

        float diffX = Math.Abs(charList[1].transform.position.x - ch.transform.position.x);
        float diffY = Math.Abs(charList[1].transform.position.y - ch.transform.position.y);

        //if the distance is greater in X, move along X, otherwise move along Y
        //if it's equal, move along both X and Y
        if (diffX > diffY)
        {
            //Debug.Log("x>y");
            if (charList[1].transform.position.x < ch.transform.position.x)
                action[0] = 2;
            else
                action[0] = 4;
        }

        if (diffX < diffY)
        {
            //Debug.Log("x<y");
            if (charList[1].transform.position.y < ch.transform.position.y)
                action[0] = 1;
            else
                action[0] = 3;
        }

        if (diffX == diffY)
        {
            //Debug.Log("x=y");
            if (charList[1].transform.position.x < ch.transform.position.x)
                action[0] = 2;
            else
                action[0] = 4;

            if (charList[1].transform.position.y < ch.transform.position.y)
                action[1] = 1;
            else
                action[1] = 3;
        }
        else action[1] = action[0];
        action[2] = 6;

        if(Math.Abs(charList[1].transform.position.x - ch.transform.position.x) + Math.Abs(charList[1].transform.position.y - ch.transform.position.y) == 2) action[1] = 0;

        //Debug.Log("action[0] is " + action[0]);
        charList[1].doActions(action);
    }

    private void doTesting(){
        copyOriginal();
        charList[0].moveTo += new Vector3(1,1,0);
        while(charList[0].gameObject.transform.position != charList[0].moveTo) ;
        charList[0].spell2Dir(0,0);
        charList[0].spell2Dir(0,0);
        while(!charList[0].canProceed()) ;
        Debug.Log("HP of enemies: " + charList[2].health + "," + charList[3].health);
        copyOriginal();
    }
}
