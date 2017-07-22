using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponScriptableObject : ScriptableObject {

    public List<Weapon> weapons = new List<Weapon>();
    public List<WeaponStats> weaponStats = new List<WeaponStats>();
}
