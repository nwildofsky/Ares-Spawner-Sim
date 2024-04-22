using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class RigidbodyMove : MonoBehaviour
{
    public float speed;
    Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        rb.MovePosition(new Vector3(transform.position.x + speed * Time.fixedDeltaTime, transform.position.y, transform.position.z));
    }
}
