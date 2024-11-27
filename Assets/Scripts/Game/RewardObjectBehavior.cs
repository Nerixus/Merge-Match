using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardObjectBehavior : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0, 180 * Time.deltaTime, 0);
    }
}
