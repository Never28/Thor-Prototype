using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public ItemInstance rightHandWeapon;
    public ItemInstance leftHandWeapon;
    public bool hasLeftHandWeapon = true;

    public GameObject parryCollider;

    StateManager states;

    public void Init(StateManager st) {
        states = st;
        if(rightHandWeapon)
            EquipWeapon(rightHandWeapon.instance, false);
        if(leftHandWeapon)
            EquipWeapon(leftHandWeapon.instance, true);

        hasLeftHandWeapon = (leftHandWeapon != null);

        InitAllDamageColliders(st);
        CloseAllDamageColliders();

        ParryCollider pr = parryCollider.GetComponent < ParryCollider>();
        pr.Init(st);
        CloseParryCollider();
    }

    public void EquipWeapon(Weapon w, bool isLeft = false) {
        string targetIdle = w.oh_idle;
        targetIdle += (isLeft) ? StaticStrings._l : StaticStrings._r;

        states.anim.SetBool(StaticStrings.mirror, isLeft);
        states.anim.Play(StaticStrings.changeWeapon);
        states.anim.Play(targetIdle);

        UI.QuickSlot uiSlot = UI.QuickSlot.singleton;

        uiSlot.UpdateSlot((isLeft) ? UI.QSlotType.lh : UI.QSlotType.rh, w.icon);
        w.weaponModel.SetActive(true);
    }

    public Weapon GetCurrentWeapon(bool isLeft) {
        if (isLeft)
            return leftHandWeapon.instance;
        else
            return rightHandWeapon.instance;
    }

    public void OpenAllDamageColliders()
    {
        if (rightHandWeapon.instance.w_hook != null)
            rightHandWeapon.instance.w_hook.OpenDamageColliders();
        if (leftHandWeapon.instance.w_hook != null)
            leftHandWeapon.instance.w_hook.OpenDamageColliders();
    }

    public void CloseAllDamageColliders() {
        if (rightHandWeapon.instance.w_hook != null)
            rightHandWeapon.instance.w_hook.CloseDamageColliders();
        if (leftHandWeapon.instance.w_hook != null)
            leftHandWeapon.instance.w_hook.CloseDamageColliders();
    }

    public void InitAllDamageColliders(StateManager states) {
        if (rightHandWeapon.instance.w_hook != null)
            rightHandWeapon.instance.w_hook.InitDamageColliders(states);
        if (leftHandWeapon.instance.w_hook != null)
            leftHandWeapon.instance.w_hook.InitDamageColliders(states);
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
    public bool leftHandMirror;
    public float parryMultiplier;
    public float backstabMultiplier;
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

    public Vector3 model_pos;
    public Vector3 model_eulers;
    public Vector3 model_scale;
}