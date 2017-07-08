﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public Weapon rightHandWeapon;
    public Weapon leftHandWeapon;
    public bool hasLeftHandWeapon = true;

    public GameObject parryCollider;

    StateManager states;

    public void Init(StateManager st) {
        states = st;
        EquipWeapon(rightHandWeapon, false);
        if(hasLeftHandWeapon)
            EquipWeapon(leftHandWeapon, true);
        InitAllDamageColliders(st);
        CloseAllDamageColliders();

        ParryCollider pr = parryCollider.GetComponent < ParryCollider>();
        pr.Init(st);
        CloseParryCollider();
    }

    public void EquipWeapon(Weapon w, bool isLeft = false) {
        string targetIdle = w.oh_idle;
        targetIdle += (isLeft) ? "_l" : "_r";

        states.anim.SetBool(StaticStrings.mirror, isLeft);
        states.anim.Play(StaticStrings.changeWeapon);
        states.anim.Play(targetIdle);

        UI.QuickSlot uiSlot = UI.QuickSlot.singleton;

        uiSlot.UpdateSlot((isLeft) ? UI.QSlotType.lh : UI.QSlotType.rh, w.icon);

    }

    public Weapon GetCurrentWeapon(bool isLeft) {
        if (isLeft)
            return leftHandWeapon;
        else
            return rightHandWeapon;
    }

    public void OpenAllDamageColliders()
    {
        if (rightHandWeapon.w_hook != null)
            rightHandWeapon.w_hook.OpenDamageColliders();
        if (leftHandWeapon.w_hook != null)
            leftHandWeapon.w_hook.OpenDamageColliders();
    }

    public void CloseAllDamageColliders() {
        if (rightHandWeapon.w_hook != null)
            rightHandWeapon.w_hook.CloseDamageColliders();
        if (leftHandWeapon.w_hook != null)
            leftHandWeapon.w_hook.CloseDamageColliders();
    }

    public void InitAllDamageColliders(StateManager states) {
        if (rightHandWeapon.w_hook != null)
            rightHandWeapon.w_hook.InitDamageColliders(states);
        if (leftHandWeapon.w_hook != null)
            leftHandWeapon.w_hook.InitDamageColliders(states);
    }

    public void OpenParryCollider() {
        parryCollider.SetActive(true);
    }

    public void CloseParryCollider() {
        parryCollider.SetActive(false);
    }
}

[System.Serializable]
public class Weapon
{
    public string weaponId;
    public Sprite icon;
    public string oh_idle;
    public string th_idle;

    public List<Action> actions;
    public List<Action> twoHandedActions;
    public WeaponStats parryStats;
    public WeaponStats backstabStats;
    public bool leftHandMirror;
    public GameObject weaponModel;
    public WeaponHook w_hook;

    public Action GetAction(List<Action> l, ActionInput input) {
        for (int i = 0; i < l.Count; i++)
        {
            if (l[i].input == input)
                return l[i];
        }
        return null;
    }
}