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

        StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlots);
        StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlots);

        if (states.inventoryManager.hasLeftHandWeapon)
        {
            StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rb, ActionInput.lb, actionSlots, true);
            StaticFunctions.DeepCopyAction(states.inventoryManager.leftHandWeapon.instance, ActionInput.rt, ActionInput.lt, actionSlots, true);
        }
        else {
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lb, ActionInput.lb, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.lt, ActionInput.lt, actionSlots);
        }
    }

    public void UpdateActionsTwoHanded()
    {
        EmptyAllSlots();
        Weapon w = states.inventoryManager.rightHandWeapon.instance;

        for (int i = 0; i < w.twoHandedActions.Count; i++)
        {
            Action a = StaticFunctions.GetAction(w.twoHandedActions[i].input, actionSlots);
            a.targetAnim = w.twoHandedActions[i].targetAnim;
            a.type = w.twoHandedActions[i].type;
        }
    }

    void EmptyAllSlots() {
        for (int i = 0; i < 4; i++)
        {
            Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);
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
        return StaticFunctions.GetAction(input, actionSlots);
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

public enum SpellClass { 
    pyromancy, miracle, sorcery 
}

public enum SpellType { 
    projectile, buff, looping
}

[System.Serializable]
public class Action {
    public ActionInput input;
    public ActionType type;
    public SpellClass spellClass;
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