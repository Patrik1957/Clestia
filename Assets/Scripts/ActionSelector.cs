using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelector : MonoBehaviour
{
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(Player.transform.position.x - 5, Player.transform.position.y + 3, 1);
        bool q = Input.GetKeyDown("q");
        bool e = Input.GetKeyDown("e");
    }
}
