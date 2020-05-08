using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : Character
{


    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
        range = 7;
    }

    // Update is called once per frame
    override protected void Update()
    {
        base.Update();
        attacking = false;
        casting = false;
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
        if (q) selectedAction--;
        if (e) selectedAction++;

        if(selectedAction == 5) selectedAction = 0;
        if(selectedAction == -1) selectedAction = 4;


        //if there is keyboard input in a direction, check for target in that direction and attack it
        if (!moving && (Math.Abs(h) > 0.5f || Math.Abs(v) > 0.5f) && !script.simulation)
        {
            switch (selectedAction)
            {
                case 0:
                    moveTo += new Vector3(h, v, 0);
                    break;
                case 1:
                    attackDir(h, v);
                    break;
                case 2:
                    spell1Dir(h, v);
                    break;
                case 3:
                    spell2Dir(h, v);
                    break;
                case 4:
                    script.controlAltarez(h, v);
                    break;
            }
        }


        anim.SetFloat("LastMoveX", lastMove.x);
        anim.SetFloat("LastMoveY", lastMove.y);
        anim.SetBool("IsMoving", !canMove);
    }

    public override bool spell1Dir(float h, float v) //Diagonal
    {
        script.spell1Used = true;
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = -1;

        //if(dirX == 0 || dirY == 0) return false;

        Character ch = null;

        for (int i = 0; i < 6 && ch == null; i++)
        {
            ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + i));
            if (ch == null) ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x - i, gameObject.transform.position.y + i));
            if (ch == null) ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y - i));
            if (ch == null) ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x - i, gameObject.transform.position.y - i));        
        }

        if (ch != null)
        {
            script.makeSpell("fireblast", ch.transform.position);
            casting = true;
            Debug.Log("Casting Spell1");
            anim.SetBool("IsCasting", casting);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            script.attackEnemy(15, ch);
            return true;
        }

        return false;
    }

    public override bool spell2Dir(float h, float v) //Targeted, AOE (not yet)
    {
        script.spell2Used = true;
        Character ch = null;

        for (int i = -3; i < 4 && ch == null; i++)
        {
            for (int j = -3; j < 4 && ch == null; j++)
            {
                ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j));
            }
        }

        if (ch != null)
        {
            script.makeSpell("tornado", ch.transform.position);
            casting = true;
            Debug.Log("Casting spell2");
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
        if (v < -0.5f) dirY = -1;

        Character ch = null;

        for (int i = 1; i < range && ch == null; i++)
        {
            ch = script.checkEnemyInPosition(gameObject, new Vector2(gameObject.transform.position.x + dirX * i, gameObject.transform.position.y + dirY * i));
        }


        if (ch != null)
        {
            Projectile proj = script.makeProjectile(this, ch, "arrow");
            this.projectiles.Add(proj);
            attacking = true;
            Debug.Log("Attacking target at " + ch.transform.position.x + "," + ch.transform.position.y);
            anim.SetBool("IsAttacking", attacking);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            return true;
        }

        return false;
    }

    public override void tryAttacking()
    {
        bool success = false;

        if (script.spell1Used)
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (!success) success = spell1Dir(i, j);
                }
            }

        if (script.spell2Used)
            if (!success) success = spell2Dir(0, 0);

        for (int i = -1; i < 2; i++)
        {
            if (!success) success = attackDir(i, 0);
        }
        for (int j = -1; j < 2; j++)
        {
            if (!success) success = attackDir(0, j);
        }
    }
}