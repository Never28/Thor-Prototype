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

        DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.rb, ActionInput.rb);
        DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.rt, ActionInput.rt);

        if (states.inventoryManager.hasLeftHandWeapon)
        {
            DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.rb, ActionInput.lb, true);
            DeepCopyAction(states.inventoryManager.leftHandWeapon, ActionInput.rt, ActionInput.lt, true);
        }
        else {
            DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.lb, ActionInput.lb);
            DeepCopyAction(states.inventoryManager.rightHandWeapon, ActionInput.lt, ActionInput.lt);
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

        if (isLeftHand)
        {
            a.mirror = true;
        }
    }

    public void UpdateActionsTwoHanded()
    {
        EmptyAllSlots();
        Weapon w = states.inventoryManager.rightHandWeapon;

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
    public bool canBackstab = false;
}

[System.Serializable]
public class ItemAction {
    public string targetAnim;
    public string itemId;
}