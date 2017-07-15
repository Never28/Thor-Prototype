using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour {

    public List<string> rh_weapons;
    public List<string> lh_weapons;
    public List<string> spell_items;

    public int r_index;
    public int l_index;
    public int s_index;
    List<RuntimeWeapon> r_r_weapons = new List<RuntimeWeapon>();
    List<RuntimeWeapon> r_l_weapons = new List<RuntimeWeapon>();
    List<RuntimeSpellItem> r_spells = new List<RuntimeSpellItem>();

    public RuntimeWeapon rightHandWeapon;
    public RuntimeWeapon leftHandWeapon;
    public RuntimeSpellItem currentSpell;
    public bool hasLeftHandWeapon = true;

    public GameObject parryCollider;
    public GameObject breathCollider;
    public GameObject blockCollider;

    StateManager states;

    public void Init(StateManager st) {
        states = st;

        LoadInventory();

        ParryCollider pr = parryCollider.GetComponent < ParryCollider>();
        pr.Init(st);
        CloseParryCollider();
        CloseBreathCollider();
        CloseBlockCollider();
    }

    public void LoadInventory() { 
        for (int i = 0; i < rh_weapons.Count; i++)
        {
            WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(rh_weapons[i]));
        }
        for (int i = 0; i < lh_weapons.Count; i++)
        {
            WeaponToRuntimeWeapon(ResourcesManager.singleton.GetWeapon(lh_weapons[i]),true);
        }

        if (r_r_weapons.Count > 0) {
            if (r_index > r_r_weapons.Count - 1)
                r_index = 0;
            rightHandWeapon = r_r_weapons[r_index];
        }
        if (r_l_weapons.Count > 0)
        {
            if (l_index > r_l_weapons.Count - 1)
                l_index = 0;
            leftHandWeapon = r_l_weapons[l_index];
        }

        if(rightHandWeapon)
            EquipWeapon(rightHandWeapon, false);
        if (leftHandWeapon) {
            EquipWeapon(leftHandWeapon, true);
            hasLeftHandWeapon = true;        
        }

        for (int i = 0; i < spell_items.Count; i++)
        {
            SpellToRuntimeSpell(ResourcesManager.singleton.GetSpell(spell_items[i]));
        }

        if (r_spells.Count > 0) {
            if (s_index > r_spells.Count - 1)
                s_index = 0;
            EquipSpell(r_spells[s_index]);
        }
        
        hasLeftHandWeapon = (leftHandWeapon != null);

        InitAllDamageColliders(states);
        CloseAllDamageColliders();
    }

    public void EquipWeapon(RuntimeWeapon w, bool isLeft = false) {

        if (isLeft)
        {
            if (leftHandWeapon != null)
            {
                leftHandWeapon.weaponModel.SetActive(false);
            }

            leftHandWeapon = w;
        }
        else {
            if (rightHandWeapon != null) {
                rightHandWeapon.weaponModel.SetActive(false);
            }

            rightHandWeapon = w;
        }

        string targetIdle = w.instance.oh_idle;
        targetIdle += (isLeft) ? StaticStrings._l : StaticStrings._r;

        states.anim.SetBool(StaticStrings.mirror, isLeft);
        states.anim.Play(StaticStrings.changeWeapon);
        states.anim.Play(targetIdle);

        UI.QuickSlot uiSlot = UI.QuickSlot.singleton;

        uiSlot.UpdateSlot((isLeft) ? UI.QSlotType.lh : UI.QSlotType.rh, w.instance.icon);
        w.weaponModel.SetActive(true);
    }

    public void EquipSpell(RuntimeSpellItem s) {

        currentSpell = s;

        UI.QuickSlot uiSlot = UI.QuickSlot.singleton;
        uiSlot.UpdateSlot(UI.QSlotType.spell, s.instance.icon);
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

    public RuntimeWeapon WeaponToRuntimeWeapon(Weapon w, bool isLeft = false) {
        GameObject go = new GameObject();
        RuntimeWeapon inst = go.AddComponent<RuntimeWeapon>();

        inst.instance = new Weapon();
        StaticFunctions.DeepCopyWeapon(w, inst.instance);
        go.name = w.itemName;

        inst.weaponModel = Instantiate(inst.instance.modelPrefab) as GameObject;
        Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
        inst.weaponModel.transform.parent = p;
        if (isLeft)
        {
            inst.weaponModel.transform.localPosition = inst.instance.l_model_pos;
            inst.weaponModel.transform.localEulerAngles = inst.instance.l_model_eulers;
        }
        else
        {
            inst.weaponModel.transform.localPosition = inst.instance.r_model_pos;
            inst.weaponModel.transform.localEulerAngles = inst.instance.r_model_eulers;        
        }

        inst.weaponModel.transform.localScale = inst.instance.model_scale;

        inst.w_hook = inst.weaponModel.GetComponentInChildren<WeaponHook>();
        inst.w_hook.InitDamageColliders(states);

        if (isLeft) {
            r_l_weapons.Add(inst);
        }else{
            r_r_weapons.Add(inst);
        }

        inst.weaponModel.SetActive(false);

        return inst;
    }

    public RuntimeSpellItem SpellToRuntimeSpell(Spell s, bool isLeft = false) {
        GameObject go = new GameObject();
        RuntimeSpellItem inst = go.AddComponent<RuntimeSpellItem>();

        inst.instance = new Spell();
        StaticFunctions.DeepCopySpell(s, inst.instance);
        go.name = s.itemName;


        r_spells.Add(inst);
        return inst;
    }

    public void CreateSpellParticle(RuntimeSpellItem inst, bool isLeft = false, bool parentUnderRoot = false) {
        if (inst.currentParticle == null) {
            inst.currentParticle = Instantiate(inst.instance.particlePrefab) as GameObject;
            inst.p_hook = inst.currentParticle.GetComponentInChildren<ParticleHook>();
            inst.p_hook.Init();
        }

        if (parentUnderRoot)
        {
            Transform p = states.anim.GetBoneTransform((isLeft) ? HumanBodyBones.LeftHand : HumanBodyBones.RightHand);
            inst.currentParticle.transform.parent = p;
            inst.currentParticle.transform.localRotation = Quaternion.identity;
            inst.currentParticle.transform.localPosition = Vector3.zero;
        }
        else {
            inst.currentParticle.transform.parent = transform;
            inst.currentParticle.transform.localRotation = Quaternion.identity;
            inst.currentParticle.transform.localPosition = new Vector3(0, 1.5f, .9f);
        }
    }

    public void ChangeToNextWeapon(bool isLeft) {
        if (isLeft)
        {
            if (l_index < r_l_weapons.Count - 1)
            {
                l_index++;
            }
            else
            {
                l_index = 0;
            }

            EquipWeapon(r_l_weapons[l_index], true);
        }
        else { 
            if (r_index < r_r_weapons.Count - 1)
            {
                r_index++;
            }
            else
            {
                r_index = 0;
            }

            EquipWeapon(r_r_weapons[r_index]);
        }

        states.actionManager.UpdateActionsOneHanded();
    }

    public void ChangeToNextSpell() {
        if (s_index < r_spells.Count - 1)
        {
            s_index++;
        }
        else
        {
            l_index = 0;
        }

        EquipSpell(r_spells[s_index]);
    }
    
    #region Delegate Calls
    
    public void OpenBreathCollider() {
        breathCollider.SetActive(true);
    }

    public void CloseBreathCollider() {
        breathCollider.SetActive(false);
    }

    public void OpenBlockCollider() {
        breathCollider.SetActive(true);
    }

    public void CloseBlockCollider() {
        breathCollider.SetActive(false);
    }

    public void EmitSpellParticle() {
        currentSpell.p_hook.Emit(1); 
    }
    #endregion
}

[System.Serializable]
public class Item {
    public string itemName;
    public string itemDescription;
    public Sprite icon;


}

[System.Serializable]
public class Weapon : Item
{
    public string oh_idle;
    public string th_idle;

    public List<Action> actions;
    public List<Action> twoHandedActions;
    public bool leftHandMirror;
    public float parryMultiplier;
    public float backstabMultiplier;

    public GameObject modelPrefab;
    public WeaponStats weaponStats;

    public Vector3 r_model_pos;
    public Vector3 l_model_pos;
    public Vector3 r_model_eulers;
    public Vector3 l_model_eulers;
    public Vector3 model_scale;

    public Action GetAction(List<Action> l, ActionInput input)
    {
        if (l == null)
            return null;

        for (int i = 0; i < l.Count; i++)
        {
            if (l[i].input == input)
                return l[i];
        }
        return null;
    }
}

[System.Serializable]
public class Spell : Item {
    public SpellType spellType;
    public SpellClass spellClass;
    public List<SpellAction> spellActions = new List<SpellAction>(); 
    public GameObject projectile;
    public GameObject particlePrefab;
    public string spell_effect;


    public SpellAction GetAction(List<SpellAction> l, ActionInput input)
    {
        if (l == null)
            return null;

        for (int i = 0; i < l.Count; i++)
        {
            if (l[i].input == input)
                return l[i];
        }
        return null;
    }
}

