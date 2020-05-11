using System;
using UnityEngine;

public class AltarezController : Character
{
    public bool moveRandomly = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        this.range = 1;
    }

    // Update is called once per frame
    override protected void Update()
    {
            attacking = false;
            casting = false;
            if(anim != null) {
                anim.SetBool("IsMoving", moving);
                anim.SetFloat("LastMoveX", lastMove.x);
                anim.SetFloat("LastMoveY", lastMove.y);
                anim.SetBool("IsAttacking", attacking);
                anim.SetBool("IsCasting", casting);
                anim.SetFloat("Health", health);
            }
        if(!script.simulation && script.whoseTurn != 1) return;
        if(!script.simulation) {doActions(actions); actions[0]=0; actions[1]=0; actions[2]=0;}
        base.Update();
        if (moveRandomly)
        {
            System.Random rnd = new System.Random();
            int randomX = rnd.Next(-3, 3);
            int randomY = rnd.Next(-3, 3);
            moveTo = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);
            moveRandomly = false;
        }
    }

    public override bool spell1Dir(float h, float v) //Heal Amy
    {
        if(script.charList[0].health > 99) return false;
        float AmyX = script.charList[0].transform.position.x;
        float AmyY = script.charList[0].transform.position.y;
        if(Math.Abs(AmyX - gameObject.transform.position.x) < 2 && Math.Abs(AmyY - gameObject.transform.position.y) < 2){
                script.attackEnemy(-15, script.charList[0]);
                script.makeSpell("shield", script.charList[0].transform.position);
                casting = true;
                anim.SetBool("IsCasting", casting);
                anim.SetFloat("AttackX", 0);
                anim.SetFloat("AttackY", -1);
                return true;
        }
        return false;
    }

    public override bool spell2Dir(float h, float v) //Targeted, AOE (not yet)
    {
        Character ch = null;

        for(int i = -3; i < 4; i++){
            for(int j = -3; j < 4; j++){
                ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j));
                if (ch != null) break;
            }
        }

        if (ch != null)
        {
            script.makeSpell("snakebite",ch.transform.position);
            casting = true;
            //Debug.Log("Casting spell2");
            anim.SetBool("IsCasting", casting);
            anim.SetFloat("AttackX", 0);
            anim.SetFloat("AttackY", -1);
            script.attackEnemy(10, ch);
            return true;
        }
        
        return false;
    }

 public override bool attackDir(float h, float v)
    {
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        Character ch = null;

        ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX, gameObject.transform.position.y + dirY));

        if (ch != null)
        {
            attacking = true;
            //Debug.Log("Attacking");
            anim.SetBool("IsAttacking", attacking);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(25, ch);
            return true;
        }
        
        return false;
    }

    public override void tryAttacking(){
        bool success = false;

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if (!success) success = attackDir(i, j);
            }
        }
        
        if (!success) success = spell2Dir(0,0);

        for(int i = -1; i < 2; i++)
        {
            for(int j = -1; j < 2; j++)
            {
                if (!success) success = spell1Dir(i, j);
            }
        }
    }

}
