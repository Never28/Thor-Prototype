﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFunctions {

    public static void DeepCopyWeapon(Weapon from, Weapon to) {        
        to.item_id = from.item_id;
        to.oh_idle = from.oh_idle;
        to.th_idle = from.th_idle;

        to.actions = new List<Action>();
        for (int i = 0; i < from.actions.Count; i++)
        {
            Action a = new Action();
            DeepCopyActionToAction(a, from.actions[i]);
            to.actions.Add(a);
        }
        to.twoHandedActions = new List<Action>();
        for (int i = 0; i < from.twoHandedActions.Count; i++)
        {
            Action a = new Action();
            DeepCopyActionToAction(a, from.twoHandedActions[i]);
            to.actions.Add(a);
        }

        to.parryMultiplier = from.parryMultiplier;
        to.backstabMultiplier = from.backstabMultiplier;
        to.leftHandMirror = from.leftHandMirror;
        to.modelPrefab = from.modelPrefab;
        to.r_model_pos = from.r_model_pos;
        to.l_model_pos = from.l_model_pos;
        to.r_model_eulers = from.r_model_eulers;
        to.l_model_eulers = from.l_model_eulers;
        to.model_scale = from.model_scale;
    }

    public static void DeepCopyActionToAction(Action to, Action from) {
        to.firstStep = new ActionAnim();
        to.firstStep.input = from.firstStep.input;
        to.firstStep.targetAnim = from.firstStep.targetAnim;

        to.comboSteps = new List<ActionAnim>();

        to.type = from.type;
        to.spellClass = from.spellClass;
        to.canParry = from.canParry;
        to.canBeParried = from.canBeParried;
        to.changeSpeed = from.changeSpeed;
        to.animSpeed = from.animSpeed;
        to.canBackstab = from.canBackstab;
        to.overrideDamageAnim = from.overrideDamageAnim;
        to.damageAnim = from.damageAnim;
        to.overrideKick = from.overrideKick;
        to.kickAnim = from.kickAnim;

        DeepCopySteps(from, to);
    }

    public static void DeepCopySteps(Action from, Action to) {

        to.comboSteps = new List<ActionAnim>();
        for (int i = 0; i < from.comboSteps.Count; i++)
        {
            ActionAnim a = new ActionAnim();
            a.input = from.comboSteps[i].input;
            a.targetAnim = from.comboSteps[i].targetAnim;
            to.comboSteps.Add(a);
        }
    }

    public static void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, List<Action> actionList, bool isLeftHand = false)
    {
        Action a = GetAction(assign, actionList);
        Action from = w.GetAction(w.actions, input);
        if (from == null)
            return;

        a.firstStep = new ActionAnim();
        a.firstStep.targetAnim = from.firstStep.targetAnim;
        a.comboSteps = new List<ActionAnim>();

        DeepCopySteps(from, a);
        a.type = from.type;
        a.spellClass = from.spellClass;
        a.canBeParried = from.canBeParried;
        a.changeSpeed = from.changeSpeed;
        a.animSpeed = from.animSpeed;
        a.canBackstab = from.canBackstab;
        a.overrideDamageAnim = from.overrideDamageAnim;
        a.damageAnim = from.damageAnim;
        a.parryMultiplier = w.parryMultiplier;
        a.backstabMultiplier = w.backstabMultiplier;
        a.overrideKick = from.overrideKick;
        a.kickAnim = from.kickAnim;
        if (isLeftHand)
        {
            a.mirror = true;
        }
    }

    public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
    {
        if (from == null) {
            Debug.Log(to.weaponId + " weapon stats weren't found, assigning everything as zero");
            return;
        }
        
        to.a_physical = from.a_physical;
        to.a_strike = from.a_strike;
        to.a_slash = from.a_slash;
        to.a_thrust = from.a_thrust;
        to.a_magic = from.a_magic;
        to.a_fire = from.a_fire;
        to.a_lightning = from.a_lightning;
        to.a_dark = from.a_dark;
    }

    public static void DeepCopySpell(Spell from, Spell to) {
        to.item_id = from.item_id;
        to.spellType = from.spellType;
        to.spellClass = from.spellClass;
        to.projectile = from.projectile;
        to.spell_effect = from.spell_effect;
        to.particlePrefab = from.particlePrefab;

        to.spellActions = new List<SpellAction>();
        for (int i = 0; i < from.spellActions.Count; i++)
        {
            SpellAction a = new SpellAction();
            DeepCopySpellAction(a, from.spellActions[i]);
            to.spellActions.Add(a);
        }
    }

    public static void DeepCopySpellAction(SpellAction to, SpellAction from) {
        to.input = from.input;
        to.targetAnim = from.targetAnim;
        to.throwAnim = from.throwAnim;
        to.castTime = from.castTime;
        to.focusCost = from.focusCost;
        to.staminaCost = from.staminaCost;
    }

    public static Action GetAction(ActionInput input, List<Action> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].GetFirstInput() == input)
            {
                return actions[i];
            }
        }

        return null;
    }

    public static void DeepCopyConsumable(Consumable to, Consumable from) {
        to.consumableEffect = from.consumableEffect;
        to.targetAnim = from.targetAnim;
        to.item_id = from.item_id;
        to.itemPrefab = from.itemPrefab;
        to.model_scale = from.model_scale;
        to.r_model_eulers = from.r_model_eulers;
        to.r_model_pos = from.r_model_pos;
    }
}
