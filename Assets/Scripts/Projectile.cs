using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 target;
    public float wait;

    public Character targetChar;
    public int damage;


    // Start is called before the first frame update
    void Start()
    {
        wait = .5f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(wait>0){
            wait -= Time.deltaTime;
            return;
        }
        int x = Math.Sign(this.target.x - gameObject.transform.position.x);
        int y = Math.Sign(this.target.y - gameObject.transform.position.y);
        if(Math.Abs(gameObject.transform.position.x - this.target.x) > 0.1f || Math.Abs(gameObject.transform.position.y - this.target.y) > 0.1f){
            gameObject.transform.Translate(new Vector3(x * moveSpeed * Time.deltaTime, y * moveSpeed * Time.deltaTime, 0));
        }
        else{
            GridModel gm = (GridModel)GameObject.FindObjectOfType(typeof(GridModel));
            gm.attackEnemy(damage,targetChar);
            Destroy(gameObject);
            Destroy(this);
            //Debug.Log("destroy");           
        }

        Vector3 vec = this.transform.position - this.target;
        if (Math.Abs(vec.x) < 0.1 * Time.timeScale* Time.timeScale && Math.Abs(vec.y) < 0.1 * Time.timeScale* Time.timeScale)
        {
            gameObject.transform.position = this.target;
        }
    }

    public void changeTargetChar(Character targ){
        this.targetChar = targ;
        this.target = targetChar.transform.position;
    }

    public void changeSpeed(float s){
        this.moveSpeed = s;
    }

    public void changeDmg(int i){
        this.damage = i;
    }
}
