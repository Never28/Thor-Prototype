﻿using System.Collections;
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

    public void UpdateActionsOneHanded() {
        EmptyAllSlots();
        Weapon w = states.inventoryManager.curWeapon;

        for (int i = 0; i < w.actions.Count; i++)
        {
            Action a = GetAction(w.actions[i].input);
            a.targetAnim = w.actions[i].targetAnim;
        }
    }

    public void UpdateActionsTwoHanded()
    {
        EmptyAllSlots();
        Weapon w = states.inventoryManager.curWeapon;

        for (int i = 0; i < w.twoHandedActions.Count; i++)
        {
            Action a = GetAction(w.twoHandedActions[i].input);
            a.targetAnim = w.twoHandedActions[i].targetAnim;
        }
    }

    void EmptyAllSlots() {
        for (int i = 0; i < 4; i++)
        {
            Action a = GetAction((ActionInput)i);
            a.targetAnim = null;
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

[System.Serializable]
public class Action {
    public ActionInput input;
    public string targetAnim;
}

[System.Serializable]
public class ItemAction {
    public string targetAnim;
    public string itemId;
}