using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class ScriptableObjectManager {

    public static void CreateAsset<T>() where T : ScriptableObject {
        T asset = ScriptableObject.CreateInstance<T>();

        if (Resources.Load(typeof(T).ToString()) == null)
        {
            string assetPath = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/" + typeof(T).ToString() + ".asset");
            AssetDatabase.CreateAsset(asset, assetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }
        else {
            Debug.Log(typeof(T).ToString() + " already created.");
        }
    }
    
    [MenuItem("Assets/Inventory/CreateItems")]
    public static void CreateItems()
    {
        ScriptableObjectManager.CreateAsset<ItemsScriptablesObject>();
    }

    [MenuItem("Assets/Inventory/CreateConsumables")]
    public static void CreateConsumables() {
        ScriptableObjectManager.CreateAsset<ConsumableScriptableObject>();
    }

    [MenuItem("Assets/Inventory/CreateWeapons")]
    public static void CreateWeapon()
    {
        ScriptableObjectManager.CreateAsset<WeaponScriptableObject>();
    }

    [MenuItem("Assets/Inventory/CreateSpellItems")]
    public static void CreateSpellItems()
    {
        ScriptableObjectManager.CreateAsset<SpellItemScriptableObject>(); 
    }
}
