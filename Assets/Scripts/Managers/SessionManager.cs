using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour {

    public List<string> weapon_items = new List<string>();
    public List<string> cons_items = new List<string>();
    public List<string> spell_items = new List<string>();

    public List<Item> GetItems(ItemType t) {
        switch (t)
        {
            case ItemType.weapon:
                return ResourcesManager.singleton.GetAllItemsFromList(weapon_items, t);
            case ItemType.spell:
                return ResourcesManager.singleton.GetAllItemsFromList(spell_items, t);
            case ItemType.consum:
                return ResourcesManager.singleton.GetAllItemsFromList(cons_items, t);
            case ItemType.equipment:
            default:
                return null;
        }
    }

    public static SessionManager singleton;
    void Awake() {
        singleton = this;
    }
}
