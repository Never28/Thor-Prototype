using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsScriptablesObject : ScriptableObject {
    public List<Item> cons_items = new List<Item>();
    public List<Item> weapon_items = new List<Item>();
    public List<Item> spell_items = new List<Item>();
}
