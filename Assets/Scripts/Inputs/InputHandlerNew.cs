using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Thor.CameraScripts;

namespace Thor{
	
	public class InputHandlerNew : MonoBehaviour {

		[HideInInspector]
		StateManager stateManager;
		[HideInInspector]
		public CameraManagerOld cameraManager;
		MovementHandler movementHandler;


		// Use this for initialization
		void Start () {
			gameObject.AddComponent<MovementHandler> ();
			SetupCamera ();
			cameraManager = CameraManagerOld.singleton;
			cameraManager.target = this.transform;
			cameraManager.transform.position = this.transform.position;
			cameraManager.stateManager = stateManager;

			stateManager = GetComponent<StateManager> ();
			movementHandler = GetComponent<MovementHandler> ();
		}

		// Update is called once per frame
		void Update () {

		}
	}

}

