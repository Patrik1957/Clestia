using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    public Character FollowTarget;
    public float MoveSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 TargetPos = new Vector3(FollowTarget.transform.position.x, FollowTarget.transform.position.y,transform.position.z);
        transform.position = Vector3.Lerp(transform.position, TargetPos, MoveSpeed * Time.deltaTime);
    }
}
