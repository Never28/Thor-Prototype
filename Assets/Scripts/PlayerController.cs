using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//movement variables
	public float runSpeed;
	//jumping
	bool grounded = false;
	Collider[] groundCollisions;
	float groundCheckRadius = 0.2f;
	public LayerMask groundLayer;
	public Transform groundCheck;
	public float jumpHeight; 

	Rigidbody myRigidbody;
	Animator myAnimator;

	bool facingRight;

	// Use this for initialization
	void Start () {
		myRigidbody = GetComponent<Rigidbody> ();
		myAnimator = GetComponent<Animator> ();
		facingRight = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate(){
		if (grounded && Input.GetAxis ("Jump") > 0) {
			grounded = false;
			myRigidbody.AddForce(Vector3.up * jumpHeight);
		}


		groundCollisions = Physics.OverlapSphere (groundCheck.position, groundCheckRadius, groundLayer);
		grounded = groundCollisions.Length > 0;

		myAnimator.SetBool ("grounded", grounded);

		float move = Input.GetAxis ("Horizontal");
		myAnimator.SetFloat ("speed", Mathf.Abs (move));

		if ((move > 0 && !facingRight) || (move < 0 && facingRight))
			Flip ();

		myRigidbody.velocity = new Vector3 (move * runSpeed, myRigidbody.velocity.y, 0);
	}

	void Flip(){
		facingRight = !facingRight;
		/*Vector3 scale = transform.localScale;
		scale.z *= -1;
		transform.localScale = scale;*/
		Quaternion rotation = transform.localRotation;
		rotation.y *= -1;
		transform.localRotation = rotation;
	}
}
