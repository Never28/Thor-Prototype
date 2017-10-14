using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utilities {
    [ExecuteInEditMode]
    public class WeaponPlacer : MonoBehaviour
    {
        public string itemId;
        public GameObject itemModel;

        public bool leftHand;
        public bool save;

        public SaveType saveType;

        public enum SaveType { weapon, consumable}

        void Update()
        {
            if (!save)
                return;
            save = false;

            switch (saveType)
            {
                case SaveType.weapon:
                    SaveWeapon();
                    break;
                case SaveType.consumable:
                    SaveConsumable();
                    break;
                default:
                    break;
            }

        }

        void SaveWeapon() {
            if (itemModel == null)
                return;
            if (string.IsNullOrEmpty(itemId))
                return;


            WeaponScriptableObject obj = Resources.Load("WeaponScriptableObject") as WeaponScriptableObject;

            if (obj == null)
                return;

            for (int i = 0; i < obj.weapons.Count; i++)
            {
                if (obj.weapons[i].item_id == itemId)
                {
                    Weapon w = obj.weapons[i];
                    if (leftHand)
                    {
                        w.l_model_eulers = itemModel.transform.localEulerAngles;
                        w.l_model_pos = itemModel.transform.localPosition;
                    }
                    else
                    {
                        w.r_model_eulers = itemModel.transform.localEulerAngles;
                        w.r_model_pos = itemModel.transform.localPosition;
                    }

                    w.model_scale = itemModel.transform.localScale;

                    return;
                }
            }

            Debug.Log(itemId + " wasn't found in inventory."); 
        }

        void SaveConsumable()
        {
            if (itemModel == null)
                return;
            if (string.IsNullOrEmpty(itemId))
                return;


            ConsumableScriptableObject obj = Resources.Load("ConsumableScriptableObject") as ConsumableScriptableObject;

            if (obj == null)
                return;

            for (int i = 0; i < obj.consumables.Count; i++) 
            {
                if (obj.consumables[i].item_id == itemId)
                {
                    Consumable c = obj.consumables[i];
                    c.r_model_eulers = itemModel.transform.localEulerAngles;
                    c.r_model_pos = itemModel.transform.localPosition;
                    c.model_scale = itemModel.transform.localScale;

                    return;
                }
            }

            Debug.Log(itemId + " wasn't found in inventory.");
        }

    }

}
