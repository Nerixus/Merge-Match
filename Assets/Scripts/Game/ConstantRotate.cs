using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotate : MonoBehaviour
{
    public Vector3 rotationVector = new Vector3(0,360,0);
    void Update()
    {
        transform.Rotate(rotationVector * Time.deltaTime);
    }
}
