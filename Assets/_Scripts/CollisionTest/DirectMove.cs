using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectMove : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(transform.position.x + speed * Time.deltaTime,
                                transform.position.y,
                                transform.position.z);
    }
}
