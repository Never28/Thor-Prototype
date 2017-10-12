﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI { 

    public class InventoryUI : MonoBehaviour {

        public EquipmentLeft eq_left;
        public CenterOverlay c_Overlay;
        public WeaponInfo weaponInfo;
        public PlayerStatus playerStatus;

        public GameObject gameMenu, inventory, centerMain, centerRight, centerOverlay;

        void Start() {
            CreateUIElements();
        }

        void CreateUIElements() {
            WeaponInfoInit();
            PlayerStatusInit();
        }

        void WeaponInfoInit() {
            for (int i = 0; i < 6; i++)
            {
                CreateAttDefUIElement(weaponInfo.ap_slots, weaponInfo.ap_grid, (AttackDefenseType)i);
            }
            for (int i = 0; i < 5; i++)
            {
                CreateAttDefUIElement(weaponInfo.g_absorb, weaponInfo.g_grid, (AttackDefenseType)i);
            }

            CreateAttDefUIElement(weaponInfo.g_absorb, weaponInfo.g_grid, AttackDefenseType.stability);

            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_grid, AttackDefenseType.bleed);
            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_grid, AttackDefenseType.curse);
            CreateAttDefUIElement_Mini(weaponInfo.a_effects, weaponInfo.a_effects_grid, AttackDefenseType.frost);

            CreateAttributeElement_Mini(weaponInfo.att_bonus, weaponInfo.att_b_grid, AttributeType.strenght);
            CreateAttributeElement_Mini(weaponInfo.att_bonus, weaponInfo.att_b_grid, AttributeType.dexterity);
            CreateAttributeElement_Mini(weaponInfo.att_bonus, weaponInfo.att_b_grid, AttributeType.intelligence);
            CreateAttributeElement_Mini(weaponInfo.att_bonus, weaponInfo.att_b_grid, AttributeType.faith);

            CreateAttributeElement_Mini(weaponInfo.att_req, weaponInfo.att_r_grid, AttributeType.strenght);
            CreateAttributeElement_Mini(weaponInfo.att_req, weaponInfo.att_r_grid, AttributeType.dexterity);
            CreateAttributeElement_Mini(weaponInfo.att_req, weaponInfo.att_r_grid, AttributeType.intelligence);
            CreateAttributeElement_Mini(weaponInfo.att_req, weaponInfo.att_r_grid, AttributeType.faith);
        }

        void PlayerStatusInit() {
            CreateAttributeElement(playerStatus.attSlot, playerStatus.grid, AttributeType.level);
            CreateEmptyElement(playerStatus.grid);
            for (int i = 1; i < 10; i++)
            {                
                CreateAttributeElement(playerStatus.attSlot, playerStatus.grid, (AttributeType)i);
            }
            CreateEmptyElement(playerStatus.grid);
            for (int i = 0; i < 3; i++)
            {
                int index = 1;
                index += 10;
                CreateAttributeElement(playerStatus.attSlot, playerStatus.grid, (AttributeType)i);
            }
            CreateEmptyElement(playerStatus.grid);
            for (int i = 0; i < 4; i++)
            {
                int index = 1;
                index += 13;
                CreateAttributeElement(playerStatus.attSlot, playerStatus.grid, (AttributeType)i);
            }
        }

        void CreateAttDefUIElement(List<AttDefType> l, Transform p, AttackDefenseType t) {
            AttDefType a = new AttDefType();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slot_template) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.text1.text = a.type.ToString();
            g.SetActive(true);
            g.transform.localScale = Vector3.one;
        }

        void CreateAttDefUIElement_Mini(List<AttDefType> l, Transform p, AttackDefenseType t)
        {
            AttDefType a = new AttDefType();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slot_mini) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.text1.text = "-";
            g.SetActive(true);
            g.transform.localScale = Vector3.one;
        }

        void CreateAttributeElement(List<AttributeSlot> l, Transform p, AttributeType t)
        {
            AttributeSlot a = new AttributeSlot();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(playerStatus.slotTemplate) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.text1.text = t.ToString();
            a.slot.text2.text = "30";
            g.SetActive(true);
            g.transform.localScale = Vector3.one;
        }

        void CreateAttributeElement_Mini(List<AttributeSlot> l, Transform p, AttributeType t)
        {
            AttributeSlot a = new AttributeSlot();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slot_mini) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.text1.text = "-";
            g.SetActive(true);
            g.transform.localScale = Vector3.one;
        }

        void CreateEmptyElement(Transform p)
        {
            GameObject g = Instantiate(playerStatus.emptySlot) as GameObject;
            g.transform.SetParent(p);
            g.SetActive(true);
            g.transform.localScale = Vector3.one;
        }


        public UIState curState;
    
        public void Tick() { 
        
        }

        public static InventoryUI singleton;
        void Awake() {
            singleton = this;
        }

    }

    public enum UIState { 
        equipment, inventory, attributes, messages, options
    }

    [System.Serializable]
    public class EquipmentLeft{
        public Text slotName;
        public Text curItem;
        public EquipmentSlots equipmentSlots;
        public Left_Inventory inventory;
    }

    [System.Serializable]
    public class PlayerStatus {
        public GameObject slotTemplate;
        public GameObject emptySlot;
        public Transform grid;
        public List<AttributeSlot> attSlot = new List<AttributeSlot>();
    }

    [System.Serializable]
    public class Left_Inventory {
        public Slider invSlider;
        public GameObject slotTemplate;
        public Transform slotGrid;
    }

    [System.Serializable]
    public class EquipmentSlots { 

    }

    [System.Serializable]
    public class CenterOverlay {
        public Image bigIcon;
        public Text itemName;
        public Text itemDescription;
        public Text skillName;
        public Text skillDescription;
    }

    #region WeaponInfo
    [System.Serializable]
    public class WeaponInfo {
        public Image smallIcon;
        public GameObject slot_template;
        public GameObject slot_mini;
        public GameObject breakSlot;
        public Text itemName;
        public Text weaponType;
        public Text damageType;
        public Text skillName;
        public Text fpCost;
        public Text weight;
        public Text durability_cur;
        public Text durability_max;
        public Transform ap_grid;
        public List<AttDefType> ap_slots = new List<AttDefType>();
        public Transform g_grid;
        public List<AttDefType> g_absorb = new List<AttDefType>();
        public Transform a_effects_grid;
        public List<AttDefType> a_effects = new List<AttDefType>();
        public Transform att_b_grid;
        public List<AttributeSlot> att_bonus = new List<AttributeSlot>();
        public Transform att_r_grid;
        public List<AttributeSlot> att_req = new List<AttributeSlot>();

        public AttributeSlot GetAttributeSlot(List<AttributeSlot> l, AttributeType type){
            for (int i = 0; i < l.Count; i++)
			{
			 if(l[i].type == type)
                 return l[i];
			}
            return null;
        }

        public AttDefType GetAttDefSlot(List<AttDefType> l, AttackDefenseType type)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].type == type)
                    return l[i];
            }
            return null;
        }
    }

    [System.Serializable]
    public class ItemDetails
    {

    }

    [System.Serializable]
    public class AttributeSlot {
        public bool isBreak;
        public AttributeType type;
        public InventoryUISlot slot;
    } 

    [System.Serializable]
    public class AttDefType {
        public bool isBreak;
        public AttackDefenseType type;
        public InventoryUISlot slot;
    } 

    #endregion

}
