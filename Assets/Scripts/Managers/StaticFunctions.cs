﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StaticFunctions {

    public static void DeepCopyWeapon(Weapon from, Weapon to) {
        to.icon = from.icon;
        to.oh_idle = from.oh_idle;
        to.th_idle = from.th_idle;

        to.actions = new List<Action>();
        for (int i = 0; i < from.actions.Count; i++)
        {
            Action a = new Action();
            a.weaponStats = new WeaponStats();
            DeepCopyActionToAction(a, from.actions[i]);
            to.actions.Add(a);
        }
        to.twoHandedActions = new List<Action>();
        for (int i = 0; i < from.twoHandedActions.Count; i++)
        {
            Action a = new Action();
            a.weaponStats = new WeaponStats();
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

    public static void DeepCopyActionToAction(Action a, Action w_a) {
        a.input = w_a.input;
        a.targetAnim = w_a.targetAnim;
        a.type = w_a.type;
        a.spellClass = w_a.spellClass;
        a.canParry = w_a.canParry;
        a.canBeParried = w_a.canBeParried;
        a.changeSpeed = w_a.changeSpeed;
        a.animSpeed = w_a.animSpeed;
        a.canBackstab = w_a.canBackstab;
        a.overrideDamageAnim = w_a.overrideDamageAnim;
        a.damageAnim = w_a.damageAnim;

        DeepCopyWeaponStats(w_a.weaponStats, a.weaponStats);
    }

    public static void DeepCopyAction(Weapon w, ActionInput input, ActionInput assign, List<Action> actionList, bool isLeftHand = false)
    {
        Action a = GetAction(assign, actionList);
        Action w_a = w.GetAction(w.actions, input);
        if (w_a == null)
            return;
        a.targetAnim = w_a.targetAnim;
        a.type = w_a.type;
        a.spellClass = w_a.spellClass;
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

    public static void DeepCopyWeaponStats(WeaponStats from, WeaponStats to)
    {
        to.physical = from.physical;
        to.strike = from.strike;
        to.slash = from.slash;
        to.thrust = from.thrust;
        to.magic = from.magic;
        to.fire = from.fire;
        to.lightning = from.lightning;
        to.dark = from.dark;
    }

    public static void DeepCopySpell(Spell from, Spell to) {
        to.itemName = from.itemName;
        to.itemDescription = from.itemDescription;
        to.icon = from.icon;
        to.spellType = from.spellType;
        to.spellClass = from.spellClass;
        to.projectile = from.projectile;
        to.spell_effect = from.spell_effect;
        to.particlePrefab = from.particlePrefab;

        to.spellActions = new List<SpellAction>();
        for (int i = 0; i < from.spellActions.Count; i++)
        {
            SpellAction a = new SpellAction();
            DeepCopySpellActionToSpellAction(a, from.spellActions[i]);
            to.spellActions.Add(a);
        }
    }

    public static void DeepCopySpellActionToSpellAction(SpellAction to, SpellAction from) {
        to.input = from.input;
        to.targetAnim = from.targetAnim;
        to.throwAnim = from.throwAnim;
        to.castTime = from.castTime;
    }

    public static Action GetAction(ActionInput input, List<Action> actions)
    {
        for (int i = 0; i < actions.Count; i++)
        {
            if (actions[i].input == input)
            {
                return actions[i];
            }
        }

        return null;
    }
	
}
