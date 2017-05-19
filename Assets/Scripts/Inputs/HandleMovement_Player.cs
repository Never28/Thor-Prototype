using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thor{
	public class HandleMovement_Player : MonoBehaviour {

		StateManager states;
		Rigidbody rb;

		public bool doAngleCheck = true;
		[SerializeField]
		float degreesRunThreshold = 8;
		[SerializeField]
		bool useDot = true;

		bool overrideForce;
		bool inAngle;

		float rotateTimer_;
		float velocityChange = 4;
		bool applyJumpForce;

		float turnAngle;
		float movement;
		Vector3 storeDirection;
		InputHandler ih;

		Vector3 curVelocity;
		Vector3 targetVelocity;
		float prevAngle;
		Vector3 prevDir;

		bool overrideCanInterupted;
		bool interuptOverride;
		Vector3 overrideDirection;
		float overrideSpeed;
		float forceOverrideTimer;
		float forceOverLife;
		bool stopVelocity;
		bool useForceCurve;
		AnimationCurve forceCurve;
		float fc_t;
		bool initVault;
		Vector3 startPosition;

		bool forceOverHasRan;
		delegate void ForceOverrideStart();
		ForceOverrideStart forceOverStart;
		delegate void ForceOverrideWrap();
		ForceOverrideWrap forceOverWrap;

		bool enableRootMovement;

		public void Init (StateManager st, InputHandler inh) {
			ih = inh;
			states = st;
			rb = st.rBody;
			states.anim.applyRootMotion = false;
		}

		public void Tick(){
			if (!overrideForce && !initVault) {
				HandleDrag ();
				MovementNormal ();
				HandleJump ();
			} else {
				states.horizontal = 0;
				states.vertical = 0;
				states.anim.SetFloat (Statics.horizontal, states.horizontal);
				states.anim.SetFloat (Statics.vertical, states.vertical);
				OverrideLogic ();
			}
		}

		void MovementNormal(){
			//for variable speed, not direction
			float abs_v = Mathf.Abs(states.vertical);
			float abs_h = Mathf.Abs (states.horizontal);
			movement = Mathf.Abs(Mathf.Clamp01(abs_v + abs_h));

			//inAngle = states.inAngle_MoveDir;
			inAngle = true;
			
			//for direction
			Vector3 v = ih.camManager.transform.forward * states.vertical;
			Vector3 h = ih.camManager.transform.right * states.horizontal;

			v.y = 0;
			h.y = 0;

			if (states.onGround) {
				//HandleRotation ();

				float targetSpeed = states.walk_f_speed;

				if (states.vertical < 0) {
					targetSpeed = states.walk_b_speed;
				}

				if (states.run && states.groundAngle == 0 && states.anim.GetBool(Statics.onSprint)) {
					targetSpeed = states.sprintSpeed;
				}

				if (states.crouching) {
					targetSpeed = states.walk_c_speed;
				}
				if (states.aiming) {
					targetSpeed = states.aimSpeed;
				}
				if (inAngle) {
					HandleVelocity_Normal (h, v, targetSpeed);
				} else {
					rb.velocity = Vector3.zero;
				}
			}

			HandleAnimations_Normal ();
		}

		void HandleVelocity_Normal(Vector3 h, Vector3 v, float speed){
			Vector3 curVelocity = rb.velocity;

			if (states.curStates == StateManager.CharStates.moving) {
				targetVelocity = (h + v).normalized * (speed * movement);
				velocityChange = 3;
			} else {
				velocityChange = 2;
				targetVelocity = Vector3.zero;
			}

			Vector3 vel = Vector3.Lerp (curVelocity, targetVelocity, Time.deltaTime * velocityChange);
			rb.velocity = vel;

			if (states.obstacleForward) {
				rb.velocity = Vector3.zero;
			}
		}

		void HandleRotation_Normal(Vector3 h, Vector3 v){
			if (states.curStates == StateManager.CharStates.moving) {
				storeDirection = (v + h).normalized;

				float targetAngle = Mathf.Atan2 (storeDirection.x, storeDirection.z) * Mathf.Rad2Deg;

				if (states.run && doAngleCheck) {
					if (!useDot) {
						if ((Mathf.Abs (prevAngle - targetAngle)) > degreesRunThreshold) {
							prevAngle = targetAngle;
							PlayAnimSpecial (AnimSpecials.runToStop, false);
							return;
						}
					} else {
						float dot = Vector3.Dot (prevDir, states.moveDirection);
						if (dot < 0) {
							prevDir = states.moveDirection;
							PlayAnimSpecial (AnimSpecials.runToStop, false);
							return;
						}
					}
				}

				prevDir = states.moveDirection;
				prevAngle = targetAngle;

				storeDirection += transform.position;
				Vector3 targetDir = (storeDirection - transform.position).normalized;
				targetDir.y = 0;
				if (targetDir == Vector3.zero) {
					targetDir = transform.forward;
				}

				Quaternion targetRot = Quaternion.LookRotation (targetDir);
				transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, velocityChange * Time.deltaTime);
			}
		}

		void HandleRotation(){
			float speed = 2;
			if (states.curStates == StateManager.CharStates.moving) {
				speed = 3;
			}

			Ray ray = new Ray (ih.camManager.camTrans.position, ih.camManager.camTrans.forward);
			Vector3 forwardPos = ray.GetPoint (50);

			Vector3 targetDir = (forwardPos - transform.position).normalized;
			targetDir.y = 0;
			if (targetDir == Vector3.zero) {
				targetDir = transform.forward;
			}

			turnAngle = Vector3.Angle (transform.forward, targetDir);

			Quaternion targetRot = Quaternion.LookRotation (targetDir);
			transform.rotation = Quaternion.Slerp (transform.rotation, targetRot, speed * Time.deltaTime);
		}

		void HandleAnimations_Normal(){
			Vector3 relativeDirection = transform.InverseTransformDirection (states.moveDirection);

			float h = relativeDirection.x;
			float v = relativeDirection.z;

			if (states.obstacleForward) {
				v = 0;
			}

			if (states.aiming || states.crouching) {
				h = Mathf.Clamp (h, -0.5f, 0.5f);
				v = Mathf.Clamp (v, -0.5f, 0.5f);
			}

			float turn = turnAngle / 45;

			states.anim.SetFloat (Statics.vertical, v, 0.2f, Time.deltaTime);
			states.anim.SetFloat (Statics.horizontal, h, 0.2f, Time.deltaTime);
			states.anim.SetBool (Statics.crouch_anim, states.crouching);
			states.anim.SetFloat (Statics.turn, turn, 0.2f, Time.deltaTime);
		}

		void HandleJump(){
			if (states.onGround && states.canJump_b) {
				if (states.jumpInput && !states.jumping && states.onLocomotion && states.curStates != StateManager.CharStates.hold && states.curStates != StateManager.CharStates.onAir) {
					if (states.curStates == StateManager.CharStates.idle) {
						states.anim.SetBool (Statics.special, true);
						states.anim.SetInteger (Statics.specialType, Statics.GetAnimSpecialType (AnimSpecials.jump_idle));
					}
					if (states.curStates == StateManager.CharStates.moving) {
						states.LegFront ();
						states.jumping = true;
						states.anim.SetBool (Statics.special, true);
						states.anim.SetInteger (Statics.specialType, Statics.GetAnimSpecialType (AnimSpecials.run_jump));
						states.curStates = StateManager.CharStates.hold;
						states.anim.SetBool (Statics.onAir, true);
						states.canJump_b = false;
					}
				}
			}
			if (states.jumping) {
				if (states.onGround) {
					if (!applyJumpForce) {
						StartCoroutine (AddJumpForce (0));
						applyJumpForce = true;
					}
				} else {
					states.jumping = false;
				}
			}

		}

		void HandleDrag(){
			if (states.horizontal != 0 || states.vertical != 0 || !states.onGround) {
				rb.drag = 0;
			} else {
				rb.drag = 4;
			}
		}

		public void PlayAnimSpecial(AnimSpecials t, bool sp = true){
			int n = Statics.GetAnimSpecialType (t);
			states.anim.SetBool (Statics.special, sp);
			states.anim.SetInteger (Statics.specialType, n);
			StartCoroutine (CloseSpecialOnAnim (0.4f));
		}

		IEnumerator CloseSpecialOnAnim(float delay){
			yield return new WaitForSeconds (delay);
			states.anim.SetBool (Statics.special, false);
		}

		IEnumerator AddJumpForce(float delay){
			yield return new WaitForSeconds (delay);
			rb.drag = 0;
			Vector3 vel = rb.velocity;
			Vector3 forward = transform.forward;
			vel = forward * 3;
			vel.y = states.jumpForce;
			rb.velocity = vel;
			StartCoroutine (CloseJump ());
		}

		IEnumerator CloseJump(){
			yield return new WaitForSeconds (0.3f);
			states.curStates = StateManager.CharStates.onAir;
			states.jumping = false;
			applyJumpForce = false;
			states.canJump_b = false;
			StartCoroutine (EnableJump ());
		}

		IEnumerator EnableJump(){
			yield return new WaitForSeconds (1.3f);
			states.canJump_b = true;
		}

		public void AddVelocity (Vector3 direction, float t, float force, bool clamp){
			forceOverLife = t;
			overrideSpeed = force;
			overrideForce = true;
			forceOverrideTimer = 0;
			overrideDirection = direction;
			rb.velocity = Vector3.zero;
			stopVelocity = clamp;
		}

		void OverrideLogic(){
			rb.drag = 0;
			rb.velocity = overrideDirection * overrideSpeed;

			forceOverrideTimer += Time.deltaTime;
			if (forceOverrideTimer > forceOverLife) {
				if (stopVelocity) {
					rb.velocity = Vector3.zero;
				}
				stopVelocity = false;
				overrideForce = false;
			}
		}
	}

}

