﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DemonMageController : Character
{
    public bool moveRandomly = false;

    // Start is called before the first frame update
    override protected void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    override protected void Update()
    {
        anim.SetFloat("Health", health);
        base.Update();
        attacking = false;
        casting = false;
        if (moveRandomly)
        {
            System.Random rnd = new System.Random();
            int randomX = rnd.Next(-3, 3);
            int randomY = rnd.Next(-3, 3);
            moveTo = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);
            moveRandomly = false;
        }
    }

    public override bool spell1Dir(float h, float v) //Diagonal
    {
        int dirX = 0, dirY = 0;
        if (h > 0.5f) dirX = 1;
        if (h < -0.5f) dirX = -1;
        if (v > 0.5f) dirY = 1;
        if (v < -0.5f) dirY = 1;

        if(dirX == 0 || dirY == 0) return false;

        Character ch = null;

        for(int i = -5; i < 6; i++){
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX *  1, gameObject.transform.position.y + dirY * i));
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX * -1, gameObject.transform.position.y + dirY * i));
            if (ch != null) break;
        }

        if (ch != null)
        {
            casting = true;
            //Debug.Log("Casting Spell1");
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
        Character ch = null;

        for(int i = -3; i < 4; i++){
            for(int j = -3; j < 4; j++){
                ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + i, gameObject.transform.position.y + j));
                if (ch != null) break;
            }
        }

        if (ch != null)
        {
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

        for(int i=1; i<4 && ch == null; i++){
            ch = script.checkEnemyInPosition(gameObject.layer, new Vector2(gameObject.transform.position.x + dirX * i, gameObject.transform.position.y + dirY * i));
        }
        
        if (ch != null)
        {
            this.projectiles.Add(script.makeProjectile(this, ch, "fireball", 15, 10));
            attacking = true;
            //Debug.Log("Attacking");
            anim.SetBool("IsAttacking", attacking);
            anim.SetFloat("AttackX", dirX);
            anim.SetFloat("AttackY", dirY);
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
                if (!success) success = spell1Dir(i, j);
            }
        }

        for (int i = -1; i < 2; i++)
        {
            if (!success) success = attackDir(i, 0);
        }
        for (int j = -1; j < 2; j++)
        {
            if (!success) success = attackDir(0, j);
        }
        
        if (!success) success = spell2Dir(0,0);
    }
}
