using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

public class PlayerController : MonoBehaviour
{
    #region Variables
    //Components
    Rigidbody rb;
    protected Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    //References
    public Camera sceneCamera;
    public GameObject target;
    
    //NavMesh variables
    public bool useNavMesh;
    private float navMeshSpeed;
    public Transform goal;

    //Jumping variables
    public float gravity = -9.8f;
    bool canJump;
    bool isJumping = false;
    bool isGrounded;
    bool isFalling;
    bool startFall;
    float fallingVelocity = -1f;
    public float jumpSpeed = 12;
    public float doubleJumpSpeed = 12;
    bool doubleJumping = true;
    bool canDoubleJump = false;
    bool isDoubleJumping = false;
    bool doubleJumped = false;

    //Used for continuing momentm while in air
    public float inAirSpeed = 8f;
    float maxVelocity = 2f;
    float minVelocity = -2f;

    //Rolling variables
    public float rollSpeed = 8;
    bool isRolling = false;
    public float rollDuration;
    private Vector3 targetDashDirection;

    //Movement variables
    bool canMove = true;
    public float walkSpeed = 1.35f;
    float moveSpeed;
    public float runSpeed = 6f;
    float rotationSpeed = 40f;

    //Action variables
    bool canAction = true;
    bool isStrafing = false;
    bool isDead = false;
    bool isBlocking = false;
    bool isKnockback;

    float x;
    float z;
    float dv;
    float dh;
    Vector3 inputVector;
    Vector3 newVelocity;

    #endregion

    #region Initialization

    void Start() {
        //set the animator component
		animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
		if (!agent) {
			agent = gameObject.AddComponent<UnityEngine.AI.NavMeshAgent> ();
		}
        agent.enabled = !useNavMesh;
    }

    #endregion

    #region UpdateAndInput

    void Update() {
        if (animator) {
			if(canMove && !isBlocking && !isDead && !useNavMesh){
				CameraRelativeMovement();
			} 
			//Rolling();
			Jumping();
        }else {
            Debug.Log("ERROR: There is no animator for characeter.");
        }

        if (useNavMesh) {
            navMeshSpeed = agent.velocity.magnitude;
            agent.destination = goal.position;
        }
    }

    #endregion

    #region Fixed/Late Updates

    void FixedUpdate() {
        CheckForGrounded();
        //apply gravity force
        rb.AddForce(0, gravity, 0, ForceMode.Acceleration);
        AirControl();
        //check if character can move
        if (canMove && !isBlocking && !isDead) {
            moveSpeed = UpdateMovement();
        }
        //check if falling
        if (rb.velocity.y < fallingVelocity && !useNavMesh) {
            isFalling = true;
			animator.SetInteger(Statics.Jumping, Statics.GetJumpType(JumpType.fall));
            canJump = false;
        }else {
            isFalling = false;
        }
    }

    //get velocity of rigid body and pass the value to the animator to control the animations
    void LateUpdate() {
        if (!useNavMesh) {
            //Get local velocity of character
            float velocityXel = transform.InverseTransformDirection(rb.velocity).x;
            float velocityZel = transform.InverseTransformDirection(rb.velocity).z;
            //Update animator with movement values
			animator.SetFloat(Statics.VelocityX, velocityXel / runSpeed);
			animator.SetFloat(Statics.VelocityZ, velocityZel / runSpeed);
            //if characeter is alive and can move, set our animator
            if (!isDead && canMove) {
                if (moveSpeed > 0) {
					animator.SetBool(Statics.Moving, true);
                }
                else {
					animator.SetBool(Statics.Moving, false);
                }
            }
            else {
				animator.SetFloat(Statics.VelocityX, agent.velocity.sqrMagnitude);
				animator.SetFloat(Statics.VelocityZ, agent.velocity.sqrMagnitude);
                if (navMeshSpeed > 0) {
					animator.SetBool(Statics.Moving, true);
                } else {
					animator.SetBool(Statics.Moving, true);
                }
            }
        }
    }
    #endregion

    #region UpdateMovement

    void CameraRelativeMovement() {
		float inputDashVertical = Input.GetAxisRaw(Statics.DashVertical);
		float inputDashHorizontal = Input.GetAxisRaw(Statics.DashHorizontal);
		float inputHorizontal = Input.GetAxisRaw(Statics.Horizontal);
		float inputVertical = Input.GetAxisRaw(Statics.Vertical);

        //converts control input vectors into camera facing vectors
        Transform cameraTransform = sceneCamera.transform;
        //Forward vector relative to the camera along the x-z plane
        Vector3 forward = cameraTransform.TransformDirection(Vector3.forward);
        forward.y = 0;
        forward = forward.normalized;
        //Right vector relative to the camera always orthogonal to the forward vector
        Vector3 right = new Vector3(forward.z, 0, -forward.x);
        //direction inputs
        dv = inputDashVertical;
        dh = inputDashHorizontal;
        if (!isRolling) {
            targetDashDirection = dh * right + dv * -forward;
        }
        x = inputHorizontal;
        z = inputVertical;
        inputVector = x * right + z * forward;
    }

    //rotate character towards direction moved
    void RotateTowardMovementDir() {
        if (inputVector != Vector3.zero && !isStrafing && !isRolling & !isBlocking) {
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(inputVector), Time.deltaTime * rotationSpeed);
        }
    }

    float UpdateMovement() {
        CameraRelativeMovement();
        Vector3 motion = inputVector;
        if (isGrounded) {
            //reduce input for diagonal movement
            if (motion.magnitude > 1) {
                motion.Normalize();
            }
            if (canMove && !isBlocking) {
                //set speed by walking / running
                if (isStrafing) {
                    newVelocity = motion * walkSpeed;
                }else {
                    newVelocity = motion * runSpeed;
                }
                //if rolling use rolling speed and direction
                if (isRolling) {
                    //force the dash movement to 1
                    targetDashDirection.Normalize();
                    newVelocity = rollSpeed * targetDashDirection;
                }
            }
        }else {
            //if we are falling use momentum
            newVelocity = rb.velocity;
        }
        if(!isStrafing || !canMove) {
            RotateTowardMovementDir();
        }
        if (isStrafing) {
            //make character point at target
            Quaternion targetRotation;
            Vector3 targetPos = target.transform.position;
            targetRotation = Quaternion.LookRotation(targetPos - new Vector3(transform.position.x, 0, transform.position.z));
            transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
        }
        newVelocity.y = rb.velocity.y;
        rb.velocity = newVelocity;
        //return a movement value for the animator
        return inputVector.magnitude;
    }

    #endregion

    #region Jumping

    //checks if characeter is within a certain distance from the ground, and markes it IsGrounded
    void CheckForGrounded() {
        if (!useNavMesh) {
            float distanceToGround;
            float threshold = .45f;
            RaycastHit hit;
            Vector3 offset = new Vector3(0, .4f, 0);
            if (Physics.Raycast((transform.position + offset), -Vector3.up, out hit, 100f)) {
                distanceToGround = hit.distance;
                if (distanceToGround < threshold) {
                    isGrounded = true;
                    canJump = true;
                    startFall = false;
                    doubleJumped = false;
                    canDoubleJump = false;
                    isFalling = false;
                    if (!isJumping) {
						animator.SetInteger(Statics.Jumping, Statics.GetJumpType(JumpType.jump));
                    }
                }
            }else {
                isGrounded = false;
            }
        }else {
            isGrounded = true;
        }
    }

    void Jumping() {
        if (isGrounded) {
			if (canJump && Input.GetButtonDown(Statics.Jump)) {
                //StartCoroutine(_Jump());
            }
        }else {
            canDoubleJump = true;
            canJump = false;
            if (isFalling) {
                //set the animation back to falling
				animator.SetInteger(Statics.Jumping, Statics.GetJumpType(JumpType.fall));
                //prevent from going into land animation while in air
                if (!startFall) {
					animator.SetTrigger(Statics.JumpTrigger);
                    startFall = true;
                }
            }
			if (canDoubleJump && doubleJumping && Input.GetButtonDown(Statics.Jump) && !doubleJumped && isFalling) {
                //apply the current movement to launch velocity
                rb.velocity += doubleJumpSpeed * Vector3.up;
				animator.SetInteger(Statics.Jumping, Statics.GetJumpType(JumpType.doubleJump));
                doubleJumped = true;
            }
        }
    }

    void AirControl() {
        if (!isGrounded) {
            CameraRelativeMovement();
            Vector3 motion = inputVector;
            motion *= (Mathf.Abs(inputVector.x) == 1 && Mathf.Abs(inputVector.z) == 1) ? 0.7f : 1;
            rb.AddForce(motion * inAirSpeed, ForceMode.Acceleration);
            //limit the amount of velocity we can achieve
            float velocityX = 0;
            float velocityZ = 0;
            if (rb.velocity.x > maxVelocity) {
                velocityX = rb.velocity.x - maxVelocity;
                if (velocityX < 0) {
                    velocityX = 0;
                }
                rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
            }
            if (rb.velocity.x < minVelocity) {
                velocityX = rb.velocity.x - minVelocity;
                if (velocityX > 0) {
                    velocityX = 0;
                }
                rb.AddForce(new Vector3(-velocityX, 0, 0), ForceMode.Acceleration);
            }
            if (rb.velocity.z > maxVelocity)
            {
                velocityZ = rb.velocity.z - maxVelocity;
                if (velocityZ < 0)
                {
                    velocityZ = 0;
                }
                rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
            }
            if (rb.velocity.z < minVelocity)
            {
                velocityZ = rb.velocity.z - minVelocity;
                if (velocityZ > 0)
                {
                    velocityZ = 0;
                }
                rb.AddForce(new Vector3(0, 0, -velocityZ), ForceMode.Acceleration);
            }
        }
    }
    
    #endregion


	#region AnimationEvents
	void Hit()
	{

	}

	void FootL()
	{

	}

	void FootR()
	{

	}

	void Jump()
	{

	}

	void Land()
	{

	}
	#endregion
}
