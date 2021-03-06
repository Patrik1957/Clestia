﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : Character
{


    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

   

    // Update is called once per frame
    protected override void FixedUpdate()
    {
        anim.SetFloat("Health", health);
        base.FixedUpdate();
    }

    public override bool spell1Dir(float h, float v) //tornado, at both enemies
    {
        script.spell1Used = true;
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = -1;

        //if(dirX == 0 || dirY == 0) return false;

        Character ch = null;
        bool found = false;

        for (int i = 1; i < 4; i++)
        {
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX * i, gameObject.transform.position.y + dirY * i));
            if(ch != null){
                this.projectiles.Add(script.makeProjectile(this, ch, "tornado", 20, 3));
                found = true;
                ch = null;
            }
        }

        for (int i = 1; i < 4; i++)
        {
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX * i + dirY, gameObject.transform.position.y + dirY * i + dirX));
            if(ch != null){
                this.projectiles.Add(script.makeProjectile(this, ch, "tornado", 20, 3));
                found = true;
                ch = null;
            }
        }

        for (int i = 1; i < 4; i++)
        {
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX * i - dirY, gameObject.transform.position.y + dirY * i - dirX));
            if(ch != null){
                this.projectiles.Add(script.makeProjectile(this, ch, "tornado", 20, 3));
                found = true;
                ch = null;
            }
        }

        if (found)
        {
            casting = true;
            //Debug.Log("Casting Spell1");
            anim.SetBool("IsCasting", casting);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            return true;
        }

        return false;
    }

    public override bool spell2Dir(float h, float v) //Nearby AOE
    {
        script.spell2Used = true;
        Character ch1 = null;
        Character ch2 = null;

        for (int i = -2; i < 3 && ch2 == null; i++)
        {
            for (int j = -2; j < 3 && ch2 == null; j++)
            {
                if(ch1 == null) ch1 = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j));
                else ch2 = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j));
            }
        }

        if (ch1 != null)
        {
            for (int i = -2; i < 3; i++)
            {
                for (int j = -2; j < 3; j++)
                {
                    if(!(i==0 && j==0))
                    this.spells.Add(script.makeSpell("earthspikes", gameObject.transform.position + new Vector3(i,j,0)));
                }
            }
            casting = true;
            //Debug.Log("Casting spell2");
            anim.SetBool("IsCasting", casting);
            anim.SetFloat("AttackX", 0);
            anim.SetFloat("AttackY", -1);
            if(ch1 != null) script.attackEnemy(70, ch1);
            if(ch2 != null) script.attackEnemy(70, ch2);
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

        for (int i = 1; i < 7 && ch == null; i++)
        {
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX * i, gameObject.transform.position.y + dirY * i));
        }


        if (ch != null)
        {
            this.projectiles.Add(script.makeProjectile(this, ch, "arrow", 35, 20));
            attacking = true;
            anim.SetBool("IsAttacking", true);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
            return true;
        }

        return false;
    }

    public override bool tryAttacking()
    {
        bool success = false;

        if (script.spell2Used)
            if (!success) success = spell2Dir(0, 0);

        if (script.spell1Used){
            if (!success) success = spell1Dir(1, 1);
            if (!success) success = spell1Dir(1, -1);
            if (!success) success = spell1Dir(-1, 1);
            if (!success) success = spell1Dir(-1, -1);            
        }

        if (!success) success = attackDir(1, 0);
        if (!success) success = attackDir(0, 1);
        if (!success) success = attackDir(-1, 0);
        if (!success) success = attackDir(0, -1);

        return success;
    }
}