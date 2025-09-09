using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public Transform target;
    public UnityEngine.Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position + offset;
    }
}
