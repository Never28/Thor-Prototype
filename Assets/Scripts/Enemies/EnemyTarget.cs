﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTarget : MonoBehaviour {

    public int index;
    public List<Transform> targets = new List<Transform>();
    public List<HumanBodyBones> humanoidBones = new List<HumanBodyBones>();

    public EnemyStates eStates;

    Animator anim;

    public void Init(EnemyStates eSt)
    {
        eStates = eSt;
        anim = eSt.anim;
        if (!anim.isHuman)
            return;
        for (int i = 0; i < humanoidBones.Count; i++) {
            targets.Add(anim.GetBoneTransform(humanoidBones[i]));
        }

        EnemyManager.singleton.enemyTargets.Add(this);
    }

    public Transform GetTarget(bool negative = false)
    {
        if (targets.Count == 0)
            return transform;

        if (!negative)
        {
            if (index < targets.Count - 1)
            {
                index++;
            }
            else
            {
                index = 0;
            }
        }
        else {
            if (index <= 0)
            {
                index = targets.Count - 1;
            }
            else {
                index--; 
            }
        }

        index = Mathf.Clamp(index, 0, targets.Count);

        return targets[index];
    }

}
