using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 target;
    public float wait;
    // Start is called before the first frame update
    void Start()
    {
        wait = .5f;
    }

    // Update is called once per frame
    void Update()
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
        if((Math.Abs(gameObject.transform.position.x - this.target.x) <= 0.1f && Math.Abs(gameObject.transform.position.y - this.target.y) <= 0.1f)){
            Destroy(gameObject);
            //Debug.Log("destroy");           
        }

    }

    public void changeTarget(Vector3 targ){
        this.target = targ;
    }

    public void changeSpeed(float s){
        this.moveSpeed = s;
    }
}
