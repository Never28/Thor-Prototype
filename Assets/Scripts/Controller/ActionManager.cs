using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    public int actionIndex;
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
            a.steps = w.twoHandedActions[i].steps;
            a.type = w.twoHandedActions[i].type;
        }
    }

    void EmptyAllSlots() {
        for (int i = 0; i < 4; i++)
        {
            Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);
            a.steps = null;
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

    public Action GetActionFromInput(ActionInput a_input) {
        return StaticFunctions.GetAction(a_input, actionSlots);
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
    public List<ActionSteps> steps;
    public bool mirror = false;
    public bool canBeParried = true;
    public bool changeSpeed = false;
    public float animSpeed = 1.0f;
    public bool canBackstab = true;
    public bool canParry = true;
    public float staminaCost = 5;
    public int focusCost = 0;

    [HideInInspector]
    public float parryMultiplier;
    [HideInInspector]
    public float backstabMultiplier;

    public bool overrideDamageAnim;
    public string damageAnim;

    ActionSteps defaultStep;

    public ActionSteps GetActionStep(ref int index) {
        if(steps == null || steps.Count == 0){
            if (defaultStep == null) {
                defaultStep = new ActionSteps();
                defaultStep.branches = new List<ActionAnim>();
                ActionAnim aa = new ActionAnim();
                aa.input = input;
                aa.targetAnim = targetAnim;
                defaultStep.branches.Add(aa);
            }
            return defaultStep;
        }

        if (index > steps.Count - 1)
            index = 0;
        ActionSteps returnValue = steps[index];
        if (index > steps.Count - 1)
        {
            index = 0;
        }
        else {
            index++;
        }
        return returnValue;
    }
}

[System.Serializable]
public class ActionSteps {
    public List<ActionAnim> branches = new List<ActionAnim>();

    public ActionAnim GetBranch(ActionInput inp) {
        for (int i = 0; i < branches.Count; i++)
        {
            if (branches[i].input == inp)
                return branches[i];
        }
        return branches[0];
    }
}

[System.Serializable]
public class ActionAnim {
    public ActionInput input;
    public string targetAnim;
}

[System.Serializable]
public class SpellAction {
    public ActionInput input;
    public string targetAnim;
    public string throwAnim;
    public float castTime;
    public float focusCost;
    public float staminaCost;
}

[System.Serializable]
public class ItemAction {
    public string targetAnim;
    public string itemId;
}