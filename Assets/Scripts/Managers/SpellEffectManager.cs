using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellEffectManager : MonoBehaviour {

    Dictionary<string, int> spellEffects = new Dictionary<string, int>();

    public void UseSpellEffect(string id, StateManager c, EnemyStates e = null) {
        int index = GetEffect(id);

        if (index == -1) {
            Debug.Log("Spell effect doesn't exit");
            return;
        }

        switch (index)
        {
            case 0:
                FireBreath(c);
                break;
            case 1:
                DarkShield(c);
                break;
            case 2:
                HealingSmall(c);
                break;
            case 3:
                FireBall(c);
                break;
            case 4:
                OnFire(c, e);
                break;
        }
    }

    int GetEffect(string id) {
        int index = -1;
        if (spellEffects.TryGetValue(id, out index)) { 
        }
        return index;
    }

    void FireBreath(StateManager c) {
        c.spellCast_Start = c.inventoryManager.OpenBreathCollider;
        c.spellCast_Loop = c.inventoryManager.EmitSpellParticle;
        c.spellCast_Stop = c.inventoryManager.CloseBreathCollider;
    }

    void DarkShield(StateManager c)
    {
        c.spellCast_Start = c.inventoryManager.OpenBlockCollider;
        c.spellCast_Loop = c.inventoryManager.EmitSpellParticle;
        c.spellCast_Stop = c.inventoryManager.CloseBlockCollider;
    }

    void HealingSmall(StateManager c) {
        c.spellCast_Stop = c.AddHealth;
    }

    void FireBall(StateManager c) {
        c.spellCast_Loop = c.inventoryManager.EmitSpellParticle;
    }

    void OnFire(StateManager c, EnemyStates e) {
        if (c != null) { 
        }
        if (e != null) {
            e.spellEffect_Loop = e.OnFire;
        }
    }

    public static SpellEffectManager singleton;
    void Awake() {
        singleton = this;

        spellEffects.Add("firebreath", 0);
        spellEffects.Add("darkshield", 1);
        spellEffects.Add("healingsmall", 2);
        spellEffects.Add("fireball", 3);
        spellEffects.Add("onfire", 4);
    }
}
