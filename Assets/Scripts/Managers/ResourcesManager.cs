using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {

    public List<Weapon> weapons = new List<Weapon>();
    Dictionary<string, int> weaponDictionary = new Dictionary<string, int>();

    public static ResourcesManager singleton;
    void Awake() {
        singleton = this;

        for (int i = 0; i < weapons.Count; i++)
        {
            if (string.IsNullOrEmpty(weapons[i].weaponId))
                continue;
            if (!weaponDictionary.ContainsKey(weapons[i].weaponId))
                weaponDictionary.Add(weapons[i].weaponId, i);
            else
                Debug.Log(weapons[i].weaponId + " is a duplicated id");
        }
    }

    public Weapon GetWeapon(string id) {
        int index = -1;
        if (weaponDictionary.TryGetValue(id, out index)) {
            return weapons[index];
        }
        return null;
    }
}
