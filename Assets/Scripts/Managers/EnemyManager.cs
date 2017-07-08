﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour {

    public List<EnemyTarget> enemyTargets = new List<EnemyTarget>();

    public EnemyTarget GetEnemy(Vector3 from) {
        EnemyTarget r = null;
        float minDis = float.MaxValue;
        for (int i = 0; i < enemyTargets.Count; i++)
        {
            float tDis = Vector3.Distance(from, enemyTargets[i].GetTarget().position);
            if (tDis < minDis) {
                minDis = tDis;
                r = enemyTargets[i];
            }
        }
        return r;
    }

    public static EnemyManager singleton;
    void Awake() {
        singleton = this;
    }
}
