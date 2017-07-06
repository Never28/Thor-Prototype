using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public Weapon curWeapon;

    public void Init() {
        curWeapon.w_hook.CloseDamageColliders();
    }
}

[System.Serializable]
public class Weapon
{
    public List<Action> actions;
    public List<Action> twoHandedActions;
    public GameObject weaponModel;
    public WeaponHook w_hook;
}