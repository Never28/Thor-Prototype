using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {

    Dictionary<string, int> i_spells = new Dictionary<string, int>();
    Dictionary<string, int> i_weapons = new Dictionary<string, int>();
    Dictionary<string, int> i_cons = new Dictionary<string, int>();
    Dictionary<string, int> weaponIds = new Dictionary<string, int>();
    Dictionary<string, int> spellIds = new Dictionary<string, int>();
    Dictionary<string, int> weaponStatsIds = new Dictionary<string, int>();
    Dictionary<string, int> consumableIds = new Dictionary<string, int>();

    public static ResourcesManager singleton;

    //Init
    void Awake() {
        singleton = this;
        LoadItemIds();
        LoadWeaponIds();
        LoadSpellIds();
        LoadConsumableIds();
    }

    void LoadItemIds() {
        ItemsScriptablesObject obj = Resources.Load("ItemsScriptablesObject") as ItemsScriptablesObject;

        if (obj == null)
        {
            Debug.Log("ItemsScriptablesObject could not be loaded");
            return;
        }

        for (int i = 0; i < obj.cons_items.Count; i++)
        {
            if (i_cons.ContainsKey(obj.cons_items[i].item_id))
            {
                Debug.Log(obj.cons_items[i].item_id + " item is a duplicate");
            }
            else
            {
                i_cons.Add(obj.cons_items[i].item_id, i);
            }
        }
        for (int i = 0; i < obj.spell_items.Count; i++)
        {
            if (i_spells.ContainsKey(obj.spell_items[i].item_id))
            {
                Debug.Log(obj.spell_items[i].item_id + " item is a duplicate");
            }
            else
            {
                i_spells.Add(obj.spell_items[i].item_id, i);
            }
        }
        for (int i = 0; i < obj.weapon_items.Count; i++)
        {
            if (i_weapons.ContainsKey(obj.weapon_items[i].item_id))
            {
                Debug.Log(obj.weapon_items[i].item_id + " item is a duplicate");
            }
            else
            {
                i_weapons.Add(obj.weapon_items[i].item_id, i);
            }
        }
    }

    void LoadSpellIds() {
        SpellItemScriptableObject obj = Resources.Load("SpellItemScriptableObject") as SpellItemScriptableObject;

        if (obj == null) {
            Debug.Log("SpellItemScriptableObject could not be loaded");
            return;
        }

        for (int i = 0; i < obj.spells.Count; i++)
        {
            if (spellIds.ContainsKey(obj.spells[i].item_id))
            {
                Debug.Log(obj.spells[i].item_id + " item is a duplicate");
            }
            else
            {
                spellIds.Add(obj.spells[i].item_id, i);
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
            if (weaponIds.ContainsKey(obj.weapons[i].item_id))
            {
                Debug.Log(obj.weapons[i].item_id + " item is a duplicate");
            }
            else {
                weaponIds.Add(obj.weapons[i].item_id, i);
            }
        }

        for (int i = 0; i < obj.weaponStats.Count; i++)
        {
            if (weaponStatsIds.ContainsKey(obj.weaponStats[i].weaponId))
            {
                Debug.Log(obj.weaponStats[i].weaponId + " item is a duplicate");
            }
            else {
                weaponStatsIds.Add(obj.weaponStats[i].weaponId, i);
            }
        }
    }

    void LoadConsumableIds()
    {
        ConsumableScriptableObject obj = Resources.Load("ConsumableScriptableObject") as ConsumableScriptableObject;

        if (obj == null)
        {
            Debug.Log("ConsumableScriptableObject could not be loaded");
            return;
        }

        for (int i = 0; i < obj.consumables.Count; i++)
        {
            if (consumableIds.ContainsKey(obj.consumables[i].item_id))
            {
                Debug.Log(obj.consumables[i].item_id + " item is a duplicate");
            }
            else
            {
                consumableIds.Add(obj.consumables[i].item_id, i);
            }
        }

    }

    int GetIndexFromString(Dictionary<string, int> d, string id)
    {
        int index = -1;
        d.TryGetValue(id, out index);
        return index;
    }

    public enum ItemType { 
        weapon, spell, consum,equipment
    }

    public Item GetItem(string id, ItemType type) {
        ItemsScriptablesObject obj = Resources.Load("ItemsScriptablesObject") as ItemsScriptablesObject;

        if (obj == null)
        {
            Debug.Log("ItemsScriptablesObject could not be loaded");
            return null;
        }

        Dictionary<string, int> d = null;
        List<Item> l = null;

        switch (type)
        {
            case ItemType.weapon:
                d = i_weapons;
                l = obj.weapon_items;
                break;
            case ItemType.spell:
                d = i_spells;
                l = obj.spell_items;
                break;
            case ItemType.consum: 
                d = i_cons;
                l = obj.cons_items;
                break;
            case ItemType.equipment:
                break;
            default:
                return null;
        }

        if (d == null || l == null)
            return null;

        int index = GetIndexFromString(d, id);
        if (index == -1)
            return null;

        return l[index];

    }

    //Weapons

    public Weapon GetWeapon(string id) {

        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
        {
            Debug.Log("WeaponScriptableObject could not be loaded");
            return null;
        }
        int index = GetIndexFromString(weaponIds, id);
        if (index == -1)
            return null;

        return obj.weapons[index];

    }

    public WeaponStats GetWeaponStats(string id)
    {

        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        if (obj == null)
        {
            Debug.Log("WeaponScriptableObject could not be loaded");
            return null;
        }

        int index = GetIndexFromString(weaponStatsIds, id);

        if (index == -1)
            return null;

        return obj.weaponStats[index];

    }

    //Spells

    public Spell GetSpell(string id)
    {

        SpellItemScriptableObject obj = Resources.Load("SpellItemScriptableObject") as SpellItemScriptableObject;

        if (obj == null)
        {
            Debug.Log("SpellItemScriptableObject could not be loaded");
            return null;
        }

        int index = GetIndexFromString(spellIds, id);

        if (index == -1)
            return null;

        return obj.spells[index];

    }

    //Consumables

    public Consumable GetConsumable(string id)
    {

        ConsumableScriptableObject obj = Resources.Load("ConsumableScriptableObject") as ConsumableScriptableObject;

        if (obj == null)
        {
            Debug.Log("ConsumableScriptableObject could not be loaded");
            return null;
        }

        int index = GetIndexFromString(consumableIds, id);

        if (index == -1)
            return null;

        return obj.consumables[index];

    }

}
