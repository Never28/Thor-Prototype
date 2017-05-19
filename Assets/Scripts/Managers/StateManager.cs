using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Thor{

	public class StateManager : MonoBehaviour {

		[Header("Status")]
		public int health = 100;
		public bool isDead;

		[Header("Info")]
		public GameObject modelPrefab;
		public bool inGame; 
		public bool isPlayer;
		[HideInInspector]
		public LayerMask ignoreLayers;

		[Header("Stats")]
		public float groundDistance = 0.6f;
		public float groundOffset = 0;
		public float groundAngle;
		public float distanceToCheckForward = 1.3f;
		public float airTimeThreshold = 0.8f;

		[Header("Inputs")]
		public float horizontal;
		public float vertical;
		public bool jumpInput;

		[Header("States")]
		public bool obstacleForward;
		public bool groundForward;
		public bool jumping;
		public bool inAction;

		#region StateRequestes
		[Header("State Requestes")]
		public CharacterStates currentState;
		public bool onGround{
			get{ 
				bool r = false;
				if (currentState == CharacterStates.hold) {
					return false;
				}

				Vector3 origin = transform.position + (Vector3.up * 0.55f);

				RaycastHit hit = new RaycastHit ();
				bool isHit = false;
				FindGround (origin, ref hit, ref isHit);

				if (!isHit) {
					for (int i = 0; i < 4; i++) {
						Vector3 newOrigin = origin;

						switch (i) {
						case 0: //forward
							newOrigin += Vector3.forward / 3;	
							break;
						case 1: //backwards
							newOrigin -= Vector3.forward / 3;
							break;
						case 2: //left
							newOrigin -= Vector3.right / 3;
							break;
						case 3: //right
							newOrigin += Vector3.right / 3;
							break;
						}

						FindGround (newOrigin, ref hit, ref isHit);
						if (isHit) {
							break;
						}
					}
				}

				r = isHit;

				if (r) {
					Vector3 targetPosition = transform.position;
					targetPosition.y = hit.point.y + groundOffset;
					transform.position = targetPosition;
				}

				return r;
			}
		}
		#endregion

		#region Variables
		[HideInInspector]
		public Vector3 moveDirection;
		[HideInInspector]
		public float airTime;
		[HideInInspector]
		public bool prevGround;
		#endregion

		#region References
		[HideInInspector]
		public GameObject activeModel;
		[HideInInspector]
		public Animator animator;
		[HideInInspector]
		public Rigidbody rigidbody;
		[HideInInspector]
		BoneHelpers boneHelpers;
		[HideInInspector]
		public Collider collider;
		[HideInInspector]
		public AudioSource audioSource;
		#endregion

		bool hasRagdoll;
		List<Rigidbody> ragdollRigidbodies = new List<Rigidbody> ();
		List<Collider> ragdollColliders = new List<Collider> ();

		public enum CharacterStates{
			idle, moving, onAir, hold
		}

		#region Setup
		// Use this for initialization
		void Start () {
			isPlayer = true;
			inGame = true;

			SetupModel ();
			SetupAnimator ();
			AddComponents ();
			SetupLayer ();
			SetupRagdoll ();
		}

		void SetupModel(){
			activeModel = Instantiate (modelPrefab) as GameObject;
			activeModel.transform.parent = this.transform;
			activeModel.transform.localPosition = Vector3.zero;
			activeModel.transform.localEulerAngles = Vector3.zero;
			activeModel.transform.localScale = Vector3.one;
		}

		void SetupAnimator(){
			animator = GetComponent<Animator> ();
			Animator childAnimator = activeModel.GetComponent<Animator> ();
			animator.avatar = childAnimator.avatar;
			Destroy (childAnimator);
		}

		void AddComponents(){
			gameObject.AddComponent<Rigidbody> ();
			rigidbody = GetComponent<Rigidbody> ();
			rigidbody.angularDrag = 999;
			rigidbody.drag = 4;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotationZ | RigidbodyConstraints.FreezeRotationX;

			gameObject.AddComponent<BoneHelpers> ();
			boneHelpers = GetComponent<BoneHelpers> ();

			collider = GetComponent<Collider> ();

			gameObject.AddComponent<AudioSource> ();
			audioSource = GetComponent<AudioSource> ();
		}

		void SetupLayer(){
			gameObject.layer = 8;
			ignoreLayers = ~(1 << 2 | 1 << 8 | 1 << 9);
		}

		void SetupRagdoll(){
			Rigidbody[] rigidbodies = GetComponentsInChildren<Rigidbody> ();
			Collider[] colliders = GetComponentsInChildren<Collider> ();

			foreach (Rigidbody r in rigidbodies) {
				if (r != rigidbody) {
					ragdollRigidbodies.Add (r);
					r.isKinematic = true;
					r.gameObject.layer = 9;
				}
			}

			foreach (Collider c in colliders) {
				if (c != collider) {
					ragdollColliders.Add (c);
					c.isTrigger = true;
				}
			}

			hasRagdoll = (ragdollRigidbodies.Count > 2);
		}
		#endregion

		public void FixedTick(){
			if (currentState == CharacterStates.hold) {
				return;
			}
			//onGround = CheckOnGround ();

			UpdateObstacle ();
			UpdateState ();
			UpdateAirTime ();
		}

		public void RegularTick(){
			//onGround = CheckOnGround ();
			UpdateInAction ();
			if (reload) {
				//weaponManager.ReloadWeapon ();
			}
			if (switchWeapon) {
				//weaponManager.ChangeWeapon ();
			}
			if (!inAction) {
				//weaponManager.Tick ();
			}
		}

		public void LateTick(){
			//onGround = CheckOnGround ();
		}

		void UpdateObstacle(){
			obstacleForward = false;
			groundForward = false;
			if (onGround) {
				Vector3 origin = transform.position;
				origin += Vector3.up * 0.75f;
				IsClear (origin, transform.forward, distanceToCheckForward, ref obstacleForward);
				if (!obstacleForward) {
					origin += transform.forward * 0.6f;
					IsClear (origin, -Vector3.up, groundDistance * 3, ref groundForward);
				} else {
					if (Vector3.Angle (transform.forward, moveDirection) > 30) {
						obstacleForward = false;
					}
				}
			}
		}

		void UpdateState(){
			if (currentState == CharacterStates.hold) {
				return;
			}
			if (horizontal != 0 && vertical != 0) {
				currentState = CharacterStates.moving;
			} else {
				currentState = CharacterStates.idle;
			}
			if (!onGround) {
				currentState = CharacterStates.onAir;
			}
		}

		public bool CheckOnGround(){
			bool r = false;
			if (currentState == CharacterStates.hold) {
				return false;
			}
		
			Vector3 origin = transform.position + (Vector3.up * 0.55f);

			RaycastHit hit = new RaycastHit ();
			bool isHit = false;
			FindGround (origin, ref hit, ref isHit);

			if (!isHit) {
				for (int i = 0; i < 4; i++) {
					Vector3 newOrigin = origin;

					switch (i) {
					case 0: //forward
						newOrigin += Vector3.forward / 3;	
						break;
					case 1: //backwards
						newOrigin -= Vector3.forward / 3;
						break;
					case 2: //left
						newOrigin -= Vector3.right / 3;
						break;
					case 3: //right
						newOrigin += Vector3.right / 3;
						break;
					}

					FindGround (newOrigin, ref hit, ref isHit);
					if (isHit) {
						break;
					}
				}
			}

			r = isHit;

			if (r) {
				Vector3 targetPosition = transform.position;
				targetPosition.y = hit.point.y + groundOffset;
				transform.position = targetPosition;
			}

			return r;
		}

		void FindGround(Vector3 origin, ref RaycastHit hit, ref bool isHit){
			Debug.DrawRay (origin, -Vector3.up * 0.5f, Color.red);
			if (Physics.Raycast (origin, -Vector3.up, out hit, groundDistance, ignoreLayers)) {
				isHit = true;
			}
		}

		void IsClear(Vector3 origin, Vector3 direction, float distance, ref bool isHit){
			RaycastHit hit = new RaycastHit ();

			int numberOfHits = 0;
			for (int i = -1; i < 2; i++) {
				Vector3 targetOrigin = origin;
				targetOrigin += transform.right * (i * 0.3f);
				Debug.DrawRay (targetOrigin, direction * distance, Color.green);
				if (Physics.Raycast (targetOrigin, direction, out hit, distance, ignoreLayers)) {
					numberOfHits++;
				}
			}
			if (numberOfHits > 2) {
				isHit = true;
			} else {
				isHit = false;
			}

			if (obstacleForward) {
				Vector3 incomingVec = hit.point - origin;
				Vector3 reflectVec = Vector3.Reflect (incomingVec, hit.normal);
				float angle = Vector3.Angle (incomingVec, reflectVec);

				if (angle < 70) {
					obstacleForward = false;
				} else {
					//Moved to Controller_Extras
				}
			}

			if (groundForward) {
				if (currentState == CharacterStates.moving) {
					Vector3 p1 = transform.position;
					Vector3 p2 = hit.point;
					float diffY = p1.y - p2.y;
					groundAngle = diffY;
				}
				float targetIncline = 0;

				if (Mathf.Abs (groundAngle) > 0.2f) {
					if (groundAngle < 0) {
						targetIncline = 1;
					} else {
						targetIncline = -1;
					}
				} else {
					groundAngle = 0;
				}

				if (groundAngle == 0) {
					targetIncline = 0;
				}

				animator.SetFloat (Statics.incline, targetIncline, 0.3f, Time.deltaTime);
			}
		}

		void UpdateAirTime(){
			if (!jumping) {
				animator.SetBool (Statics.onAir, !onGround);
			}
			if (onGround) {
				if (prevGround != onGround) {
					animator.SetInteger (Statics.jumpType, (airTime > airTimeThreshold) ? (currentState == CharacterStates.moving) ? 2 : 1 : 0);
				}

				airTime = 0;
			} else {
				airTime += Time.deltaTime;
			}

			prevGround = onGround;
		}

		void UpdateInAction(){
			/*inAction = false;
			if (switchingWeapon) {
				inAction = true;
			}
			if (vaulting) {
				inAction = true;
			}

			bool inIdle = animator.GetBool (Statics.inIdle);

			if (inIdle && !hold) {
				reloading = false;
				switchingWeapon = false;
				anim.SetBool (Statics.closeIK, false);
			}*/
		}

		public void SubtractHealth(int value){
			health -= value;
			if (health <= 0) {
				KillCharacter ();
			}
		}

		void KillCharacter(){
			if (isDead) {
				return;
			}	

			isDead = true;

			animator.CrossFade (Statics.death, 0.3f);
			MonoBehaviour[] monos = GetComponents<MonoBehaviour> ();
			foreach (MonoBehaviour m in monos) {
				m.enabled = false;
			}

			if (hasRagdoll) {
				foreach (Rigidbody r in ragdollRigidbodies) {
					r.isKinematic = false;
					r.gameObject.layer = 9;
				}

				foreach (Collider c in ragdollColliders) {
					c.isTrigger = false; 
				}
			} else {
				CapsuleCollider cap = GetComponent<CapsuleCollider> ();
				Vector3 cen = Vector3.up * 0.5f;
				cap.center = cen;
			}

			animator.enabled = true;

			StartCoroutine ("DestroyCharacter");
		}

		IEnumerator DestroyCharacter(){
			yield return new WaitForSeconds (3);
			activeModel.transform.parent = null;
			Destroy (gameObject); 
		}
	}

}
