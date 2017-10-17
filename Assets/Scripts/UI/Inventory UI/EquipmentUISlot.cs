using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    public class EquipmentUISlot : MonoBehaviour
    {
        public IconBase icon;
        public EqSlotType slotType;

        public Vector2 slotPos;

        public void Init(InventoryUI ui)
        {
            icon = GetComponent<IconBase>();
            ui.equipSlotsUI.AddSlotOnList(this);
        }

    }

}
