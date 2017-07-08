using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    public List<Action> actionSlots = new List<Action>();

    public ItemAction consumableItem;

    StateManager states;

    public void Init(StateManager st) {
        states = st;

        UpdateActionsOneHanded();
    }

    public void UpdateActionsOneHanded()
    {
        EmptyAllSlots();

        DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb);
        DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt);

        if (states.inventoryManager.hasLeftHandWeapon)
        {
            DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, true);
            DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, true);
        }
        else {
            DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb);
            DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt);
        }
    }

    public void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, bool isLeftHand = false)
    {
        Action a = GetAction(assign);
        Action w_a = w.GetAction(w.actions, input);
        if (w_a == null)
            return;
        a.targetAnim = w_a.targetAnim;
        a.type = w_a.type;
        a.canBeParried = w_a.canBeParried;
        a.changeSpeed = w_a.changeSpeed;
        a.animSpeed = w_a.animSpeed;
        a.canBackstab = w_a.canBackstab;
        a.overrideDamageAnim = w_a.overrideDamageAnim;
        a.damageAnim = w_a.damageAnim;
        a.parryMultiplier = w.parryMultiplier;
        a.backstabMultiplier = w.backstabMultiplier;

        if (isLeftHand)
        {
            a.mirror = true;
        }

        DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);
    }

    public void DeepCopyWeaponStats(WeaponStats from, WeaponStats to) {
        to.physical = from.physical;
        to.strike = from.strike;
        to.slash = from.slash;
        to.thrust = from.thrust;
        to.magic = from.magic;
        to.fire = from.fire;
        to.lightning = from.lightning;
        to.dark = from.dark;
    }

    public void UpdateActionsTwoHanded()
    {
        EmptyAllSlots();
        Weapon w = states.inventoryManager.rightHandWeapon.instance;

        for (int i = 0; i < w.twoHandedActions.Count; i++)
        {
            Action a = GetAction(w.twoHandedActions[i].input);
            a.targetAnim = w.twoHandedActions[i].targetAnim;
            a.type = w.twoHandedActions[i].type;
        }
    }

    void EmptyAllSlots() {
        for (int i = 0; i < 4; i++)
        {
            Action a = GetAction((ActionInput)i);
            a.targetAnim = null;
            a.mirror = false;
            a.type = ActionType.attack;
        }
    }

    ActionManager() {
        if (actionSlots.Count != 0)
            return;
        for (int i = 0; i < 4; i++)
        {
            Action a = new Action();
            a.input = (ActionInput)i;
            actionSlots.Add(a);
        }
    }

    public Action GetActionSlot(StateManager st) {
        ActionInput input = GetActionInput(st);
        return GetAction(input);
    }

    Action GetAction(ActionInput input) {
        for (int i = 0; i < actionSlots.Count; i++)
        {
            if (actionSlots[i].input == input) {
                return actionSlots[i];
            }
        }

        return null;
    }

    public ActionInput GetActionInput(StateManager st) { 

        if (st.rb)
            return ActionInput.rb;
        if (st.lb)
            return ActionInput.lb;
        if (st.rt)
            return ActionInput.rt;
        if (st.lt)
            return ActionInput.lt;

        return ActionInput.rb;
    }

    public bool IsLeftHandSlot(Action slot) {
        return (slot.input == ActionInput.lb || slot.input == ActionInput.lt);
    }
}

public enum ActionInput { 
    rb, lb, rt, lt
}

public enum ActionType { 
    attack, block, spell, parry
}

[System.Serializable]
public class Action {
    public ActionInput input;
    public ActionType type;
    public string targetAnim;
    public bool mirror = false;
    public bool canBeParried = true;
    public bool changeSpeed = false;
    public float animSpeed = 1.0f;
    public bool canBackstab = true;
    public bool canParry = true;
    [HideInInspector]
    public float parryMultiplier;
    [HideInInspector]
    public float backstabMultiplier;

    public bool overrideDamageAnim;
    public string damageAnim;

    public WeaponStats weaponStats;
}

[System.Serializable]
public class ItemAction {
    public string targetAnim;
    public string itemId;
}