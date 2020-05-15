using System;
using UnityEngine;

public class AltarezController : Character
{
    public bool moveRandomly = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void FixedUpdate()
    {        
        anim.SetFloat("Health", health);
        if(!script.simulation)  {
            if(script.whoseTurn != 1) return;
            doActions(actions); 
            actions[0]=0; 
            actions[1]=0; 
            actions[2]=0;
        }
        base.FixedUpdate();
    }

    public override bool spell1Dir(float h, float v) //Heal Amy
    {
        if(script.charList[0].health > 90) return false;
        float AmyX = script.charList[0].transform.position.x;
        float AmyY = script.charList[0].transform.position.y;
        if(Math.Abs(AmyX - gameObject.transform.position.x) < 2 && Math.Abs(AmyY - gameObject.transform.position.y) < 2){
                script.attackEnemy(-10, script.charList[0]);
                script.makeSpell("shield", script.charList[0].transform.position);
                casting = true;
                anim.SetBool("IsCasting", casting);
                anim.SetFloat("AttackX", 0);
                anim.SetFloat("AttackY", -1);
                return true;
        }
        return false;
    }

    public override bool spell2Dir(float h, float v) //Targeted in 7x7 area
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
            script.attackEnemy(20, ch);
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
            script.attackEnemy(50, ch);
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
