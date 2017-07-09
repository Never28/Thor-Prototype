using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {

    public List<Weapon> weapons = new List<Weapon>();

    public static ResourcesManager singleton;
    void Awake() {
        singleton = this;
    }
}
