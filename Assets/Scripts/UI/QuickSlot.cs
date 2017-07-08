using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class QuickSlot : MonoBehaviour
    {
        public List<QSlot> slots;

        public void Init() {
            ClearIcons();
        }

        public void ClearIcons() {
            for (int i = 0; i < slots.Count; i++)
            {
                slots[i].icon.gameObject.SetActive(false);
            }
        }

        public void UpdateSlot(QSlotType type, Sprite i) {
            QSlot q = GetSlot(type);
            q.icon.sprite = i;
            q.icon.gameObject.SetActive(true);
        }

        public QSlot GetSlot(QSlotType t){
            for (int i = 0; i < slots.Count; i++)
			{
                if (slots[i].type == t)
                    return slots[i];
			}
            return null;
        }

        public static QuickSlot singleton;
        void Awake() {
            singleton = this; 
        }
    }

    public enum QSlotType { 
        rh, lh, item, spell
    }

    [System.Serializable]
    public class QSlot {
        public Image icon;
        public QSlotType type; 
    }
}
