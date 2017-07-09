using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {

    Dictionary<string, int> weaponIds = new Dictionary<string, int>();
    Dictionary<string, int> spellIds = new Dictionary<string, int>();

    public static ResourcesManager singleton;
    void Awake() {
        singleton = this;
        LoadWeaponIds();
        LoadSpellIds();
    }

    void LoadSpellIds() {
        SpellItemScriptableObject obj = Resources.Load("SpellItemScriptableObject") as SpellItemScriptableObject;

        if (obj == null) {
            Debug.Log("SpellItemScriptableObject could not be loaded");
            return;
        }

        for (int i = 0; i < obj.spells.Count; i++)
        {
            if (spellIds.ContainsKey(obj.spells[i].itemName))
            {
                Debug.Log(obj.spells[i].itemName + " item is a duplicate");
            }
            else
            {
                spellIds.Add(obj.spells[i].itemName, i);
            }
        }
    }

    void LoadWeaponIds() {
        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
        {
            Debug.Log("WeaponScriptableObject could not be loaded");
            return;
        }

        for (int i = 0; i < obj.weapons.Count; i++)
        {
            if (weaponIds.ContainsKey(obj.weapons[i].itemName))
            {
                Debug.Log(obj.weapons[i].itemName + " item is a duplicate");
            }
            else {
                weaponIds.Add(obj.weapons[i].itemName, i);
            }
        }
    }

    int GetSpellIdFromString(string id)
    {
        int index = -1;
        if (spellIds.TryGetValue(id, out index))
        {
            return index;
        }
        return -1;
    }

    int GetWeaponIdFromString(string id) {
        int index = -1;
        if (weaponIds.TryGetValue(id, out index))
        {
            return index;
        }
        return -1;
    }

    public Spell GetSpell(string id)
    {

        SpellItemScriptableObject obj = Resources.Load("SpellItemScriptableObject") as SpellItemScriptableObject;

        if (obj == null)
        {
            Debug.Log("SpellItemScriptableObject could not be loaded");
            return null;
        }

        int index = GetSpellIdFromString(id);

        if (index == -1)
            return null;

        return obj.spells[index];

    }

    public Weapon GetWeapon(string id) {

        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
        {
            Debug.Log("WeaponScriptableObject could not be loaded");
            return null;
        }

        int index = GetWeaponIdFromString(id);

        if (index == -1)
            return null;

        return obj.weapons[index];

    }
}
