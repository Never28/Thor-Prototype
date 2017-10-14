using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CharacterStats {

    [Header("Current")]
    public float _health;
    public float _focus;
    public float _stamina;
    public int _souls;

    public float _healthRecoverValue = 60;
    public float _focusRecoverValue = 80;

    [Header("Base Power")]
    public int health = 100;
    public int focus = 50;
    public int stamina = 30;
    public float equip = 20;
    public float poise = 20;
    public float itemDiscovery = 111;

    [Header("Attack Power")]
    public int r_weapon_1 = 51;
    public int r_weapon_2 = 51;
    public int r_weapon_3 = 51;
    public int l_weapon_1 = 51;
    public int l_weapon_2 = 51;
    public int l_weapon_3 = 51;

    [Header("Defence")]
    public int physical = 10;
    public int vs_strike = 10;
    public int vs_slash = 10;
    public int vs_thrust = 10;
    public int magic = 30;
    public int fire = 30;
    public int lightning = 30;
    public int dark = 30;

    [Header("Resistance")]
    public int bleed = 10;
    public int poison = 10;
    public int frost = 10;
    public int curse = 10;

    public int attunementSlots = 10;

    public void InitCurrent() {

        if (statEffects != null)
            statEffects();

        _health = health;
        _focus = focus;
        _stamina = stamina; 

    }

    public delegate void StatEffects();
    public StatEffects statEffects;

    public void AddHealth() {
        health += 5;
    }

    public void RemoveHealth() {
        health -= 5;
    }
}


public enum AttributeType
{
    level, vigor, attunement, endurance, vitality, strenght, dexterity, intelligence, faith, luck, hp, fp, stamina, equip_load, poise, item_discovery, attunement_slots
}

public enum AttackDefenseType { 
    physical, magic, fire, lightning, dark, critical, stability, bleed, curse, frost, magicBuff, strike, slash, thrust, poison 
}

public enum WeaponDamageType { 
    sum, vs_strike, vs_slash, vs_thrust
}

[System.Serializable]
public class Attributes {
    public int level = 1;
    public int souls = 0;
    public int vigor = 11;
    public int attunement = 11;
    public int endurance = 11;
    public int vitality = 11;
    public int strenght = 11;
    public int dexterity = 11;
    public int intelligence = 11;
    public int faith = 11;
    public int luck = 11;
}

[System.Serializable]
public class WeaponStats{
    public string weaponId;
    public int physical;
    public int strike;
    public int slash;
    public int thrust;
    public int magic = 0;
    public int fire = 0;
    public int lightning = 0;
    public int dark = 0;
}