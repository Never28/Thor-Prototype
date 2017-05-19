using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thor.CameraScripts;

namespace Thor{
	public class InputHandler : MonoBehaviour {

		[HideInInspector]
		StateManager states;
		[HideInInspector]
		public CameraManagerOld camManager;
		HandleMovement_Player hMove;

		float horizontal;
		float vertical;
		bool runInput;
		bool aimInput;
		float shootAxis;
		float aimAxis;
		bool shootInput;
		bool reloadInput;
		bool action1Input;
		bool action2Input;
		bool switchInput;
		bool firstPerson;
		bool pivotInput;
		bool vaultInput;
		bool coverInput;
		bool crouchInput;
		bool pickupInput;

		public Vector3 aimPosition;
		[HideInInspector]
		public Vector3 coverNormal;

		Renderer[] modelRenderers;

		UIManager uiM;
		//UI.CanvasOverlay lvlCanvas;

		bool canPickup;

		bool initForMenu;

		// Use this for initialization
		void Start () {
			uiM = UIManager.singleton;
			//lvlCanvas = UI.CanvasOverlay.singleton;
			//Add References
			gameObject.AddComponent<MovementHandler> ();

			//Get References
			camManager = CameraManagerOld.singleton;
			camManager.target = this.transform;
			camManager.transform.position = transform.position;
			camManager.states = states;

			states = GetComponent<StateManager> ();
			hMove = GetComponent<MovementHandler> ();

			//Init in order	
			states.isPlayer = true;
			/*if (!SessionMaster.singleton.debugMode) {
				PlayerProfile p = Manager.SessionMaster.singleton.GetProfile ();
				CharContainer charContainer = ResourcesManager.singleton.GetChar (p.charId);
				GameObject model = charContainer.prefab;
				states.modelPrefab = model;
				states.modelRig = charContainer.rig;
				states.Init ();
				states.weaponManager.weapons = new System.Collections.Generic.List<string> ();
				states.weaponManager.weapons.Add (p.mainWeapon);
				states.weaponManager.weapons.Add (p.secWeapon);
			} else {
				states.Init ();
			}*/

			states.Init ();

			//states.weaponManager.Init (states); //the weapon manager needs to initialize after we set our weapon

			hMove.Init (states, this);

			FixPlayerMeshes ();
		}

		void FixPlayerMeshes(){
			modelRenderers = states.activeModel.GetComponentsInChildren<Renderer> ();

			SkinnedMeshRenderer[] smr = GetComponentsInChildren<SkinnedMeshRenderer> ();
			foreach (SkinnedMeshRenderer r in smr) {
				r.updateWhenOffscreen = true;
			}
		}
			
		void FixedUpdate () {
			if (states.isDead) {
				return;
			}

			states.FixedTick ();

			if (!states.inCover) {
				hMove.Tick ();
			}

			//states.ikHandler.Tick ();
		}

		void HandleInGameMenu(){
			/*if (uiM.GameMenu_UI.activeInHierarchy) {
				if (!initForMenu) {
					camManager.enabled = false;
					lvlCanvas.gameObject.SetActive (false);
					Cursor.lockState = CursorLockMode.None;
					Cursor.visible = true;

					if (!SessionMaster.singleton.isMultiplayer) {
						Time.timeScale = 0;
					}
					initForMenu = true;
				}
			} else {
				if (initForMenu) {
					lvlCanvas.gameObject.SetActive (true);
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
					camManager.enabled = true;

					if (!SessionMaster.singleton.isMultiplayer) {
						Time.timeScale = 1;
					}
					initForMenu = false;
				}
			}*/
		}

		void Update(){
			if (Input.GetKeyDown (KeyCode.Escape)) {
				//uiM.gameMenu_UI.SetActive (!uiM.gameMenu_UI.activeInHierarchy);
			}

			HandleInGameMenu ();

			if (initForMenu) {
				return;
			}
			if (states.isDead) {
				return;
			}

			GetInput ();
			UpdateStatesFromInput ();
			states.RegularTick ();
			HandleAim ();

			if (coverInput) {
				if (states.canCover && !states.inCover) {
					states.curStates = StateManager.CharStates.cover;
					states.inCover = true;
					return;
				} else {
					states.curStates = StateManager.CharStates.idle;
					states.inCover = false;
					states.rBody.isKinematic = false;
				}
			}
		}

		void LateUpdate(){
			if (states.isDead) {
				return;
			}

			states.LateTick ();
			//states.ikHandler.LateTick ();
		}

		void GetInput(){
			/*if (Input.GetButtonDown (Statics.firstPersonInput)) {
				firstPerson = !firstPerson;
			}*/

			//vertical = Input.GetAxis (Statics.Vertical);
			horizontal = Input.GetAxis (Statics.Horizontal);
			aimAxis = Input.GetAxis (Statics.aimInput);
			aimInput = (aimAxis != 0);
			shootAxis = Input.GetAxis (Statics.shootInput);
			runInput = Input.GetButton (Statics.runInput);
			reloadInput = Input.GetButton (Statics.reloadInput);
			switchInput = Input.GetButton (Statics.switchInput);
			vaultInput = Input.GetButton (Statics.vaultInput);
			coverInput = Input.GetButton (Statics.coverInput);
			crouchInput = Input.GetButton (Statics.crouchInput);
			pickupInput = Input.GetKeyUp (KeyCode.Z);

			if (!states.onLocomotion) {
				aimInput = false;
			}
			if (states.inCover) {
				runInput = false;
			}
		}
		/*
		void UpdateStatesFromInput(){
			vertical = Input.GetAxis (Statics.Vertical);
			horizontal = Input.GetAxis (Statics.Horizontal);

			Vector3 v = camManager.transform.forward * vertical;
			Vector3 h = camManager.transform.right * horizontal;

			v.y = 0;
			h.y = 0;

			states.horizontal = horizontal;
			states.vertical = vertical;

			Vector3 moveDir = (h + v).normalized;
			states.moveDirection = moveDir;
			states.inAngle_MoveDir = InAngle (states.moveDirection, 25);
			if (states.walk && horizontal != 0 || states.walk && vertical != 0) {
				states.inAngle_MoveDir = true;
			}

			states.onLocomotion = states.anim.GetBool (Statics.onLocomotion);
			HandleRun ();

			states.jumpInput = Input.GetButton (Statics.Jump);
		}*/

		void MeshesStatus(bool status){
			foreach (Renderer r in modelRenderers) {
				r.enabled = status;
			}
		}

		void UpdateStatesFromInput(){
			Vector3 v = camManager.transform.forward * vertical;
			Vector3 h = camManager.transform.right * horizontal;

			v.y = 0;
			h.y = 0;

			states.horizontal = horizontal;
			states.vertical = vertical;
			Vector3 moveDir = (h + v).normalized;
			states.moveDirection = moveDir;
			states.reload = reloadInput;
			states.switchWeapon = switchInput;
			states.onLocomotion = states.anim.GetBool (Statics.onLocomotion);

			if (!states.shooting) {
				states.aiming = aimInput;
			}
			if (states.aiming) {
				if (states.inCover && !states.inCoverCanAim) {
					states.aiming = false;
				}
			}
			if (states.reload) {
				states.aiming = false;
			}
			if (!states.aiming) {
				states.inAngle_MoveDir = InAngle (states.moveDirection, 25);
				if (states.walk && horizontal != 0 || states.walk && vertical != 0) {
					states.inAngle_MoveDir = InAngle (states.moveDirection, 60);
				}

				HandleRun ();
			} else {
				states.canRun_b = false;
				states.walk = true;
				states.inAngle_MoveDir = true;
			}

			if (crouchInput && !states.run) {
				states.crouching = !states.crouching; 
			}
		}

		bool InAngle(Vector3 targetDir, float angleThreshold){
			bool r = false;
			float angle = Vector3.Angle (transform.forward, targetDir);

			if (angle < angleThreshold) {
				r = true;
			}
			return r;
		}

		void HandleRun(){
			bool runInput = Input.GetButton (Statics.runInput);

			if (runInput) {
				states.walk = false;
				states.run = true;
				states.crouching = false;
			} else {
				states.walk = true;
				states.run = false;
			}

			if (horizontal != 0 || vertical != 0) {
				states.run = runInput;
				states.anim.SetInteger (Statics.specialType, Statics.GetAnimSpecialType(AnimSpecials.run));
			} else {
				if (states.run) {
					states.run = false;
				}
			}

			if (!states.inAngle_MoveDir && hMove.doAngleCheck) {
				states.run = false;
			}
			if (states.obstacleForward) {
				states.run = false;
			}
			if (!states.run) {
				states.anim.SetInteger(Statics.specialType, Statics.GetAnimSpecialType(AnimSpecials.runToStop));
			}
		}

		void HandleAim(){
			if (!states.vaulting) {
				states.anim.SetBool (Statics.aim, states.aiming);
			} else {
				states.anim.SetBool (Statics.aim, false);
				return;
			}

			Ray ray = new Ray (camManager.camTrans.position, camManager.camTrans.forward);
			Debug.DrawRay (ray.origin, ray.direction * 5);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 50, states.ignoreLayers)) {
				aimPosition = hit.point;
			} else {
				aimPosition = ray.GetPoint (25);
			}

			states.aimPosition = aimPosition;

			if (states.aiming && !states.inAction) {
				camManager.ChangeState (Statics.aim);
				/*Weapons.Weapon w = states.weaponManager.GetActive ().wReference;
				camManager.SetSpeed (w.weaponStats.turnSpeed, w.weaponStats.turnSpeedController);*/

				Vector3 dir = aimPosition - transform.position;
				float angle = Vector3.Angle (transform.forward, dir);
				states.inAngle_Aim = (angle < 30);

				shootInput = (shootAxis != 0) && !states.inAction;
				states.shooting = shootInput;
			} else {
				camManager.SetDefault ();
				camManager.ChangeState (Statics.normal);
				states.inAngle_Aim = false;
				shootInput = false;
				states.shooting = false;
			}

			if (states.actualShooting) {
				/*Weapons.WeaponStats activeStats = states.weaponManager.GetActive ().activeStats;
				camManager.SetOffsets (activeStats.cameraRecoilX, activeStats.cameraRecoilY);*/
			}
		}

		public void EnableRootMovement(){
			//hMove.EnableRootMovement ();
		}
	}
}


