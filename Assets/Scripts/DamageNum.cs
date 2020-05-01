using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageNum : MonoBehaviour
{
    public int damage;
    public int x,y;
    void OnGUI () 
    {
        GUI.Label (new Rect (x, y, 20, 20), damage.ToString());
    }

    // Start is called before the first frame update
    void Start()
    {
        x = y = damage = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
