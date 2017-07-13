using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKHandler : MonoBehaviour {

    Animator anim;

    Transform handHelper;
    Transform bodyHelper;
    Transform headHelper;
    Transform shoulderHelper;
    Transform animShoulder;
    Transform headTrans;

    public float weight;

    public IKSnapshot[] ikSnapshots;
    public Vector3 defaultHeadPos;

    IKSnapshot GetIKSnapshot(IKSnapshotType type){
        for (int i = 0; i < ikSnapshots.Length; i++)
		{
			if(ikSnapshots[i].type == type)
                return ikSnapshots[i];
		}

        return null;
    }

    public void Init(Animator a) {
        anim = a;

        headHelper = new GameObject().transform;
        headHelper.name = "IK Head Helper";
        handHelper = new GameObject().transform;
        handHelper.name = "IK Hand Helper";
        bodyHelper = new GameObject().transform;
        bodyHelper.name = "IK Body Helper";
        shoulderHelper = new GameObject().transform;
        shoulderHelper.name = "IK Shoulder Helper";

        shoulderHelper.parent = transform.parent;
        shoulderHelper.localPosition = Vector3.zero;
        shoulderHelper.localRotation = Quaternion.identity;
        headHelper.parent = shoulderHelper;
        bodyHelper.parent = shoulderHelper;
        handHelper.parent = shoulderHelper; 

        headTrans = anim.GetBoneTransform(HumanBodyBones.Head);
    }

    public void UpdateIKTargets(IKSnapshotType type, bool isLeft) {
        IKSnapshot snap = GetIKSnapshot(type);

        Vector3 targetBodyPos = snap.bodyPos;
        if(isLeft)
            targetBodyPos.x = -targetBodyPos.x;

        bodyHelper.localPosition = targetBodyPos;
        
        if(snap.overrideHeadPosition)
            headHelper.localPosition = snap.headPos;
        else
            headHelper.localPosition = defaultHeadPos;

        handHelper.localPosition = snap.handPos;
        handHelper.localEulerAngles = snap.handEulers;
    }

    public void OnAnimatorMoveTick(bool isLeft) {
        Transform shoulder = anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftShoulder : HumanBodyBones.RightShoulder);

        shoulderHelper.transform.position = shoulder.position;
    }

    public void Tick(AvatarIKGoal goal, float w) {

        weight = Mathf.Lerp(weight, w, Time.deltaTime * 5);

        anim.SetIKPositionWeight(goal, weight);
        anim.SetIKRotationWeight(goal, weight);
        
        anim.SetIKPosition(goal, handHelper.position);
        anim.SetIKRotation(goal, handHelper.rotation);

        anim.SetLookAtWeight(weight, 0.8f, 1, 1, 1);
        anim.SetLookAtPosition(bodyHelper.position);
    }

    public void LateTick() {
        if (headTrans == null || headHelper == null)
            return;

        Vector3 dir = headHelper.position - headTrans.position;
        if (dir == Vector3.zero)
            dir = headTrans.forward;

        Quaternion targetRot = Quaternion.LookRotation(dir);
        Quaternion curRot = Quaternion.Slerp(headTrans.rotation, targetRot, weight);

        headTrans.rotation = curRot;
    }
    
}

public enum IKSnapshotType { breath, shield_l, shield_r }

[System.Serializable]
public class IKSnapshot
{
    public IKSnapshotType type;
    public Vector3 handPos;
    public Vector3 handEulers;
    public Vector3 bodyPos;
    public bool overrideHeadPosition;
    public Vector3 headPos;
}