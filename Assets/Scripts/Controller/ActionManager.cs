using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionManager : MonoBehaviour {

    public int actionIndex;
    public List<Action> actionSlots = new List<Action>();

    StateManager states;

    public void Init(StateManager st) {
        states = st;

        UpdateActionsOneHanded();
    }

    public void UpdateActionsOneHanded()
    {
        EmptyAllSlots();
        if (states.inventoryManager.rightHandWeapon != null)
        {
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rb, ActionInput.rb, actionSlots);
            StaticFunctions.DeepCopyAction(states.inventoryManager.rightHandWeapon.instance, ActionInput.rt, ActionInput.rt, actionSlots);
        }
        if (states.inventoryManager.leftHandWeapon == null)
            return;

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
        if (states.inventoryManager.rightHandWeapon == null)
            return;

        Weapon w = states.inventoryManager.rightHandWeapon.instance;

        for (int i = 0; i < w.twoHandedActions.Count; i++)
        {
            Action a = StaticFunctions.GetAction(w.twoHandedActions[i].GetFirstInput(), actionSlots);
            a.firstStep.targetAnim = w.twoHandedActions[i].firstStep.targetAnim;
            StaticFunctions.DeepCopySteps(w.twoHandedActions[i], a);
            a.type = w.twoHandedActions[i].type;
        }
    }

    void EmptyAllSlots()
    {
        for (int i = 0; i < 4; i++)
        {
            Action a = StaticFunctions.GetAction((ActionInput)i, actionSlots);

            if (a == null)
                return;

            //a.firstStep = null;
            a.comboSteps = null;
            a.mirror = false;
            a.type = ActionType.attack;
        }
        StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rb, ActionInput.rb, actionSlots);
        StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rt, ActionInput.rt, actionSlots);
        StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rb, ActionInput.lb, actionSlots, true);
        StaticFunctions.DeepCopyAction(states.inventoryManager.unarmedRuntime.instance, ActionInput.rt, ActionInput.lt, actionSlots, true);
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
        return (slot.GetFirstInput() == ActionInput.lb || slot.GetFirstInput() == ActionInput.lt);
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
    public ActionType type;
    public SpellClass spellClass;
    public ActionAnim firstStep;
    public List<ActionAnim> comboSteps;
    public bool mirror = false;
    public bool canBeParried = true;
    public bool changeSpeed = false;
    public float animSpeed = 1.0f;
    public bool canBackstab = true;
    public bool canParry = true;
    public float staminaCost = 5;
    public int focusCost = 0;

    public bool overrideKick;
    public string kickAnim;

    [HideInInspector]
    public float parryMultiplier;
    [HideInInspector]
    public float backstabMultiplier;

    public bool overrideDamageAnim;
    public string damageAnim;

    public ActionInput GetFirstInput()
    {
        if (firstStep == null)
            firstStep = new ActionAnim();
        return firstStep.input;
    }

    public ActionAnim GetActionStep(ref int index) {

        if (index == 0)
        {
            if(comboSteps.Count == 0){
                index = 0;
            }
            else {
                index++;
            }
            return firstStep;
        }

        if (index > comboSteps.Count - 1)
            index = 0;

        ActionAnim returnValue = comboSteps[index - 1];
        index++;
        if (index > comboSteps.Count - 1)
            index = 0;
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
