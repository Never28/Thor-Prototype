using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorHook : MonoBehaviour
{

    Animator anim;
    StateManager states;
    EnemyStates eStates;
    Rigidbody rigid;

    public float rootMotionMultiplier;
    bool rolling;
    float roll_t;
    float delta;
    AnimationCurve rollCurve;

    public void Init(StateManager st, EnemyStates eSt)
    {
        states = st;
        eStates = eSt;
        if (st != null) {
            anim = st.anim;
            rigid = st.rigid;
            rollCurve = st.rollCurve;
            delta = st.delta;
        }
        if (eSt != null) {
            anim = eSt.anim;
            rigid = eSt.rigid;
            delta = eSt.delta;
        }

    }

    public void InitForRoll()
    {
        //rolling = true;
        roll_t = 0;
    }

    public void CloseRoll()
    {
        if (!rolling)
            return;
        rootMotionMultiplier = 1;
        roll_t = 0;
        rolling = false;
    }

    void OnAnimatorMove()
    {
        if (states == null && eStates == null)
            return;

        if (rigid == null)
            return;

        if (states != null) {
            if (states.canMove)
                return;

            delta = states.delta;
        }
        if (eStates != null) {
            if (eStates.canMove)
                return;

            delta = eStates.delta;
        }


        rigid.drag = 0;

        if (rootMotionMultiplier == 0)
            rootMotionMultiplier = 1;

        if (!rolling)
        {
            Vector3 delta2 = anim.deltaPosition;
            delta2.y = 0;
            Vector3 v = (delta2 * rootMotionMultiplier) / delta;
            rigid.velocity = v;
        }
        else
        {
            roll_t += delta/.6f;
            if (roll_t > 1)
                roll_t = 1;

            if (states == null)
                return;

            float zValue = rollCurve.Evaluate(roll_t);
            Vector3 v1 = Vector3.forward * zValue;
            Vector3 relative = transform.TransformDirection(v1);
            Vector3 v2 = (relative * rootMotionMultiplier) / delta;
            rigid.velocity = v2;
        }

    }

    public void OpenDamageColliders() {
        if (states == null)
            return;

        states.inventoryManager.curWeapon.w_hook.OpenDamageColliders();
    }

    public void CloseDamageColliders() {
        if (states == null)
            return;

        states.inventoryManager.curWeapon.w_hook.CloseDamageColliders();
    }
}
