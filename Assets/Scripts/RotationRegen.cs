using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationRegen : MonoBehaviour
{
    // how fast should regeneration object rotate
    private float rotationSpeed = 45f;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
    }
}
