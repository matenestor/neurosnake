using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationLights : MonoBehaviour
{
    public float lightSpeed = 3f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, lightSpeed * Time.deltaTime);
    }
}
