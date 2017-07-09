using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {

    Dictionary<string, int> itemIds = new Dictionary<string, int>();

    public static ResourcesManager singleton;
    void Awake() {
        singleton = this;
        LoadItemIds();
    }

    void LoadItemIds() {
        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        for (int i = 0; i < obj.weapons.Count; i++)
        {
            if (itemIds.ContainsKey(obj.weapons[i].itemName))
            {
                Debug.Log("Item is a duplicate");
            }
            else {
                itemIds.Add(obj.weapons[i].itemName, i);
            }
        }
    }

    int GetItemIdFromString(string id) {
        int index = -1;
        if (itemIds.TryGetValue(id, out index)) {
            return index;
        }
        return -1;
    }

    public Weapon GetWeapon(string id) {

        WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

        int index = GetItemIdFromString(id);

        if (index == -1)
            return null;

        return obj.weapons[index];

    }
}
