using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public List<string> rh_weapons;
    public List<string> lh_weapons;

    public ItemInstance rightHandWeapon;
    public ItemInstance leftHandWeapon;
    public bool hasLeftHandWeapon = true;

    public GameObject parryCollider;

    StateManager states;

    public void Init(StateManager st) {
        states = st;

        if(rh_weapons.Count > 0)
            rightHandWeapon = WeaponToItemInstance(ResourcesManager.singleton.GetWeapon(rh_weapons[0]));
        if (lh_weapons.Count > 0)
        {
            leftHandWeapon = WeaponToItemInstance(ResourcesManager.singleton.GetWeapon(lh_weapons[0]),true);
            hasLeftHandWeapon = true;        
        }

        if(rightHandWeapon)
            EquipWeapon(rightHandWeapon, false);
        if(leftHandWeapon)
            EquipWeapon(leftHandWeapon, true);

        hasLeftHandWeapon = (leftHandWeapon != null);

        InitAllDamageColliders(st);
        CloseAllDamageColliders();

        ParryCollider pr = parryCollider.GetComponent < ParryCollider>();
        pr.Init(st);
        CloseParryCollider();
    }

    public void EquipWeapon(ItemInstance w, bool isLeft = false) {
        string targetIdle = w.instance.oh_idle;
        targetIdle += (isLeft) ? StaticStrings._l : StaticStrings._r;

        states.anim.SetBool(StaticStrings.mirror, isLeft);
        states.anim.Play(StaticStrings.changeWeapon);
        states.anim.Play(targetIdle);

        UI.QuickSlot uiSlot = UI.QuickSlot.singleton;

        uiSlot.UpdateSlot((isLeft) ? UI.QSlotType.lh : UI.QSlotType.rh, w.instance.icon);
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

    public ItemInstance WeaponToItemInstance(Weapon w, bool isLeft = false) {
        GameObject go = new GameObject();
        ItemInstance inst = go.AddComponent<ItemInstance>();

        inst.instance = new Weapon();
        StaticFunctions.DeepCopyWeapon(w, inst.instance);

        inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
        Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
        inst.weaponModel.transform.parent = p;
        inst.weaponModel.transform.localPosition = inst.instance.model_pos;
        inst.weaponModel.transform.localEulerAngles = inst.instance.model_eulers;
        inst.weaponModel.transform.localScale = inst.instance.model_scale;

        inst.w_hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
        inst.w_hook.InitDamageColliders(states);    

        return inst;
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

    public GameObject modelPrefab;

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