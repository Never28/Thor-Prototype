using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	//Public variables
	public float maxSpeed = 20;
	public float acceleration = 64;
	public float jumpSpeed = 8;
	public float jumpDuration = 150;

	//Input variables
	public float horizontal;
	public float vertical;
	public float jumpInput;

	//Internal variables
	private bool onTheGround;
	private float jumpTimer;
	private bool jumpKeyDown = false;
	private bool canVariableJump = false;
	private float movement_Anim;

	Rigidbody rigidbody;
	Animator animator;
	LayerMask layerMask;
	Transform modelTransform;

	public Vector3 lookPosition;

	// Use this for initialization
	void Start () {
		rigidbody = GetComponent<Rigidbody> ();
		SetupAnimator ();

		layerMask = ~(1 << 8);

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		InputHandler ();
		UpdateRigidbodyValues ();
		MovementHandler ();
		//HandleRotation ();
		HandleAimingPosition ();
		HandleAnimations ();
	}

	void InputHandler(){
		horizontal = Input.GetAxis ("Horizontal");
		vertical = Input.GetAxis ("Vertical");
		jumpInput = Input.GetAxis ("Jump");
	}

	void UpdateRigidbodyValues(){
		if (onTheGround) {
			rigidbody.drag = 4;
		} else {
			rigidbody.drag = 0;
		}
	}

	void MovementHandler(){
		onTheGround = IsOnGround ();

		if (horizontal < -0.1f) {
			if (rigidbody.velocity.x > -this.maxSpeed) {
				rigidbody.AddForce (new Vector3 (-this.acceleration, 0, 0));
			} else {
				rigidbody.velocity = new Vector3 (-this.maxSpeed, rigidbody.velocity.y, 0);
			}
		} else if (horizontal > 0.1f) {
			if (rigidbody.velocity.x < this.maxSpeed) {
				rigidbody.AddForce (new Vector3 (this.acceleration, 0, 0));
			} else {
				rigidbody.velocity = new Vector3 (this.maxSpeed, rigidbody.velocity.y, 0);
			}
		}

		if (jumpInput > 0.1f) {
			if (!jumpKeyDown) {
				jumpKeyDown = true;

				if (onTheGround) {
					rigidbody.velocity = new Vector3 (rigidbody.velocity.y, this.jumpSpeed, 0);
					jumpTimer = 0.0f; 
				}
			}
		} else if (canVariableJump) {
			jumpTimer += Time.deltaTime;

			if (jumpTimer < jumpDuration / 1000) {
				rigidbody.velocity = new Vector3 (rigidbody.velocity.x, this.jumpSpeed, 0);
			} 
		} else {
			jumpKeyDown = false;
		}
	}

	void HandleRotation(){
		Vector3 directionToLook = lookPosition - transform.position;
		directionToLook.y = 0;
		Quaternion targetRotation = Quaternion.LookRotation (directionToLook);

		transform.rotation = Quaternion.Slerp (transform.rotation, targetRotation, Time.deltaTime * 15);
	}

	void HandleAimingPosition(){
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);

		RaycastHit hit;

		if (Physics.Raycast (ray, out hit, Mathf.Infinity)) {
			Vector3 position = hit.point;
			position.z = transform.position.z;
			lookPosition = position;
		}
	}

	void HandleAnimations(){
		animator.SetBool ("OnAir", !onTheGround);
		float animatorValue = horizontal;

		if (lookPosition.x < transform.position.x) {
			animatorValue = -animatorValue;
		}

		animator.SetFloat ("Movement", animatorValue, .1f, Time.deltaTime);
	}

	void SetupAnimator(){
		animator = GetComponent<Animator> ();

		foreach (Animator childAnimator in GetComponentsInChildren<Animator>()) {
			if (childAnimator != animator) {
				animator.avatar = childAnimator.avatar;
				modelTransform = childAnimator.transform;
				Destroy (childAnimator);
				break;
			}
		}
	}

	private bool IsOnGround(){
		bool returnValue = false;
		float lenghtToSearch = 1.5f;

		Vector3 lineStart = transform.position + Vector3.up;
		Vector3 vectorToSearch = -Vector3.up;

		RaycastHit hit;

		if(Physics.Raycast(lineStart, vectorToSearch, out hit, lenghtToSearch, layerMask)){
			returnValue = true;
		}

		return returnValue;

	}
}
