using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour {

    private Rigidbody rb;

    //speed that player moves
    public float speed;
    //max speed player can move at
    public float maxVelocity;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}
	
	// Update is called once per frame
	void Update () {
        //Movement
        PlayerMovement();
        LimitMaxSpeed(rb.velocity.magnitude);

    }

    void PlayerMovement()
    {
        //Right
        if (Input.GetKey(KeyCode.W))
        {
            rb.AddForce(Vector3.forward * speed);
        }

        //Left
        if (Input.GetKey(KeyCode.S))
        {
            rb.AddForce(Vector3.forward * -speed);
        }

        //Right
        if (Input.GetKey(KeyCode.D))
        {
            rb.AddForce(Vector3.right * speed);
        }

        //Left
        if (Input.GetKey(KeyCode.A))
        {
            rb.AddForce(Vector3.right * -speed);
        }

    }

    void LimitMaxSpeed(float movespeed)
    {
        if (movespeed > maxVelocity)
            rb.velocity = rb.velocity.normalized * maxVelocity;
    }
    
}
