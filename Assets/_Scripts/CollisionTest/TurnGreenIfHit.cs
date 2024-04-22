using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnGreenIfHit : MonoBehaviour
{
    public Material greenMat;

    private void OnCollisionEnter(Collision collision)
    {
        GetComponent<Renderer>().material = greenMat;
    }

    private void OnTriggerEnter(Collider other)
    {
        GetComponent<Renderer>().material = greenMat;
    }
}
