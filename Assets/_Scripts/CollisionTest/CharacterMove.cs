using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMove : MonoBehaviour
{
    public float speed;
    CharacterController cc;

    private void Awake()
    {
        cc = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        cc.Move(new Vector3(speed * Time.deltaTime, 0, 0));
    }
}
