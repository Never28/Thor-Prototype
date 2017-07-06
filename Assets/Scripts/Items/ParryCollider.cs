using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryCollider : MonoBehaviour {

    StateManager states;
    EnemyStates eStates;

    public float maxTimer = 0.6f;
    float timer; 

    public void Init(StateManager st) {
        states = st;
    }

    void Update() {
        if (states) {
            timer += states.delta;
            if (timer > maxTimer) {
                timer = 0;
                gameObject.SetActive(false);
            }
        }
        if (eStates)
        {
            timer += eStates.delta;
            if (timer > maxTimer)
            {
                timer = 0;
                gameObject.SetActive(false);
            }
        }     

    }

    public void InitEnemy(EnemyStates eSt) {
        eStates = eSt;
    }

    void OnTriggerEnter(Collider other) {
        //DamageCollider dc = other.GetComponent<DamageCollider>();
        //if (dc == null)
        //    return;

        if (states) {
            EnemyStates eSt = other.transform.GetComponentInParent<EnemyStates>();

            if (eSt)
                eSt.CheckForParry(transform.root, states);
        }
        if (eStates) { 
            //check for player
        }
    }
}
