using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Character FollowTarget;
    private Vector3 TargetPos;
    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        TargetPos = new Vector3(FollowTarget.transform.position.x, FollowTarget.transform.position.y,transform.position.z);
        transform.position = Vector3.Lerp(transform.position, TargetPos, MoveSpeed * Time.deltaTime);
    }
}
