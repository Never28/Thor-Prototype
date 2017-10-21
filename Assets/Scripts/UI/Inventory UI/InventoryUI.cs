using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{

    public class InventoryUI : MonoBehaviour
    {

        public EquipmentLeft eq_left;
        public CenterOverlay c_overlay;
        public WeaponInfo weaponInfo;
        public PlayerStatus playerStatus;

        public GameObject gameMenu, inventory, centerMain, centerRight, centerOverlay;

        List<IconBase> iconSlotsCreated = new List<IconBase>();
        public EquipmentSlotsUI equipSlotsUI;

        public Transform eqSlotParent;
        //public List<EquipmentUISlot> equipSlots = new List<EquipmentUISlot>();
        EquipmentUISlot[,] equipSlots;

        public Vector3 curSlotPos;

        float inputT;
        bool dontMove;

        public Color unselected;
        public Color selected;
        public EquipmentUISlot curEqSlot;
        public EquipmentUISlot prevEqSlot;

        float inputTimer;
        public float inputDelay = 0.4f;

        void HandleSlotMovement() {
            int x = Mathf.RoundToInt(curSlotPos.x);
            int y = Mathf.RoundToInt(curSlotPos.y);

            bool up = (Input.GetAxis(StaticStrings.Vertical) > 0);
            bool down = (Input.GetAxis(StaticStrings.Vertical) < 0);
            bool left = (Input.GetAxis(StaticStrings.Horizontal) < 0);
            bool right = (Input.GetAxis(StaticStrings.Horizontal) > 0);

            if (!up && !down && !left && !right)
            {
                inputTimer = 0;
            }
            else {
                inputTimer -= Time.deltaTime;
            }

            if (inputTimer < 0)
                inputTimer = 0;
            if (inputTimer > 0)
                return;


            if (up) {
                inputTimer = inputDelay;
                y--;
            }
            if (down) {
                inputTimer = inputDelay;
                y++;
            }
            if (left) {
                inputTimer = inputDelay;
                x--;
            }
            if (right) {
                inputTimer = inputDelay;
                x++;
            }
            if (x > 4)
                x = 0;
            if(x < 0)
                x = 4;
            if (y > 5)
                y = 0;
            if (y < 0)
                y = 5;

            if (curEqSlot)
                curEqSlot.icon.background.color = unselected;

            if (x == 4 && y == 3) {
                x = 4;
                y = 2;
            }

            curEqSlot = equipSlots[x, y];
            curSlotPos.x = x;
            curSlotPos.y = y;
            if(curEqSlot)
                curEqSlot.icon.background.color = selected;
        }

        #region Init

        void Start()
        {
            CreateUIElements();
            InitEqSlots();
        }

        void InitEqSlots()
        {
            EquipmentUISlot[] eq = eqSlotParent.GetComponentsInChildren<EquipmentUISlot>();
            equipSlots = new EquipmentUISlot[5, 6];

            for (int i = 0; i < eq.Length; i++)
            {
                eq[i].Init(this);
                int x = Mathf.RoundToInt(eq[i].slotPos.x);
                int y = Mathf.RoundToInt(eq[i].slotPos.y);
                equipSlots[x, y] = eq[i];
            }
        }

        void CreateUIElements()
        {
            WeaponInfoInit();
            PlayerStatusInit();
            WeaponStatusInit();
        }

        void WeaponInfoInit()
        {
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

        void PlayerStatusInit()
        {
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.level, "Level");

            CreateEmptyElement(playerStatus.attGrid);

            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.vigor, "Vigor");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.attunement, "Attunement");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.endurance, "Endurance");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.vitality, "Vitality");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.strenght, "Strenght");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.dexterity, "Dexterity");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.intelligence, "Intelligence");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.faith, "Faith");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.luck, "Luck");

            CreateEmptyElement(playerStatus.attGrid);

            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.hp, "HP");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.fp, "FP");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.stamina, "Stamina");

            CreateEmptyElement(playerStatus.attGrid);

            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.equip_load, "Equip Load");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.poise, "Stamina");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.item_discovery, "Item Discovery");
            CreateAttributeElement(playerStatus.attSlot, playerStatus.attGrid, AttributeType.attunement_slots, "Attunement Slots");
        }

        void WeaponStatusInit()
        {
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.physical, "Physical");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.strike, "Vs Strike");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.slash, "Vs Slash");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.thrust, "Vs Thrust");

            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.magic, "Magic");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.fire, "Fire");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.lightning, "Lightning");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.defGrid, AttackDefenseType.dark, "Dark");

            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.resGrid, AttackDefenseType.bleed, "Bleed");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.resGrid, AttackDefenseType.poison, "Poison");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.resGrid, AttackDefenseType.frost, "Frost");
            CreateWeaponStatusSlot(playerStatus.defSlots, playerStatus.resGrid, AttackDefenseType.curse, "Curse");

            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "R Weapon 1");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "R Weapon 2");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "R Weapon 3");

            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "L Weapon 1");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "L Weapon 2");
            CreateAttackPowerSlot(playerStatus.apSlots, playerStatus.apGrid, "L Weapon 3");
        }

        void CreateWeaponStatusSlot(List<PlayerStatusDef> l, Transform p, AttackDefenseType t, string txt1Text = null)
        {
            PlayerStatusDef w = new PlayerStatusDef();

            GameObject g = Instantiate(playerStatus.doubleSlot_template) as GameObject;
            g.SetActive(true);
            g.transform.SetParent(p);
            w.type = t;
            w.slot = g.GetComponent<InventoryUIDoubleSlot>();
            if (string.IsNullOrEmpty(txt1Text))
                w.slot.text1.text = t.ToString();
            else
                w.slot.text1.text = txt1Text;
            w.slot.text2.text = "30";
            w.slot.text3.text = "30";
            g.transform.localScale = Vector3.one;
        }

        void CreateAttackPowerSlot(List<AttackPowerSlot> l, Transform p, string id)
        {
            AttackPowerSlot a = new AttackPowerSlot();
            l.Add(a);

            GameObject g = Instantiate(weaponInfo.slot_template) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            a.slot.text1.text = id;
            a.slot.text2.text = "30";
            g.SetActive(true);
            g.transform.localScale = Vector3.one;
        }

        void CreateAttDefUIElement(List<AttDefType> l, Transform p, AttackDefenseType t)
        {
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

        void CreateAttributeElement(List<AttributeSlot> l, Transform p, AttributeType t, string txt1Text = null)
        {
            AttributeSlot a = new AttributeSlot();
            a.type = t;
            l.Add(a);

            GameObject g = Instantiate(playerStatus.slotTemplate) as GameObject;
            g.transform.SetParent(p);
            a.slot = g.GetComponent<InventoryUISlot>();
            if (string.IsNullOrEmpty(txt1Text))
                a.slot.text1.text = t.ToString();
            else
                a.slot.text1.text = txt1Text;
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
        #endregion

        public UIState curState;

        public void LoadCurrentItems(ItemType t)
        {
            List<Item> itemList = SessionManager.singleton.GetItems(t);

            if (itemList == null)
                return;
            if (itemList.Count == 0)
                return;

            GameObject prefab = eq_left.inventory.slotTemplate;
            Transform p = eq_left.inventory.slotGrid;

            int dif = iconSlotsCreated.Count - itemList.Count;
            int extra = (dif > 0) ? dif : 0;
            for (int i = 0; i < itemList.Count + extra; i++)
            {
                if (i > itemList.Count - 1)
                {
                    iconSlotsCreated[i].gameObject.SetActive(false);
                    continue;
                }
                IconBase icon = null;
                if (iconSlotsCreated.Count - 1 < i)
                {
                    GameObject g = Instantiate(prefab) as GameObject;
                    g.SetActive(true);
                    g.transform.SetParent(p);
                    g.transform.localScale = Vector3.one;
                    icon = g.GetComponent<IconBase>();
                    iconSlotsCreated.Add(icon);
                }
                else
                {
                    icon = iconSlotsCreated[i];
                }
                icon.icon.enabled = true;
                icon.icon.sprite = itemList[i].icon;
                icon.id = itemList[i].item_id;
            }
        }

        public void Tick()
        {

        }

        public void LoadEquipment(InventoryManager inv)
        {
            for (int i = 0; i < inv.rh_weapons.Count; i++)
            {
                if (i > 2)
                    break;

                EquipmentUISlot slot = equipSlotsUI.weapon[i];
                equipSlotsUI.UpdateEqSlot(inv.rh_weapons[i], slot, ItemType.weapon);
            }
            for (int i = 0; i < inv.lh_weapons.Count; i++)
            {
                if (i > 2)
                    break;

                EquipmentUISlot slot = equipSlotsUI.weapon[i + 3];
                equipSlotsUI.UpdateEqSlot(inv.lh_weapons[i], slot, ItemType.weapon);
            }

            for (int i = 0; i < inv.consumable_items.Count; i++)
            {
                if (i > 9)
                    break;

                EquipmentUISlot slot = equipSlotsUI.cons[i];
                equipSlotsUI.UpdateEqSlot(inv.consumable_items[i], slot, ItemType.consum); 
            }
        }

        public InventoryManager invManager;
        public bool load;

        void Update()
        {
            if (load)
            {
                LoadEquipment(invManager);
                prevEqSlot = null;
                load = false;
            }

            HandleSlotMovement();
            if (prevEqSlot != curEqSlot) {
                LoadItemFromSlot();
            }
            prevEqSlot = curEqSlot;
        }

        void LoadItemFromSlot() {
            if (curEqSlot == null)
                return;

            if (string.IsNullOrEmpty(curEqSlot.icon.id)) {
                curEqSlot.icon.id = "unarmed";
            }

            ResourcesManager rm = ResourcesManager.singleton;

            eq_left.slotName.text = curEqSlot.slotName;

            switch (curEqSlot.slotType)
            {
                case EqSlotType.weapons:
                    LoadWeaponItem(rm);
                    break;
                case EqSlotType.arrows:
                    break;
                case EqSlotType.bolts:
                    break;
                case EqSlotType.equipment:
                    break;
                case EqSlotType.rings:
                    break;
                case EqSlotType.covenant:
                    break;
                case EqSlotType.consumables:
                    LoadConsumableItem(rm);
                    break;
                default:
                    break;
            }
        }

        void LoadWeaponItem(ResourcesManager rm) {
            string weaponId = curEqSlot.icon.id;
            WeaponStats stats = rm.GetWeaponStats(weaponId);
            Item item = rm.GetItem(curEqSlot.icon.id, ItemType.weapon);

            eq_left.curItem.text = item.name_item;

            UpdateCenterOverlay(item);

            //Center
            weaponInfo.smallIcon.sprite = item.icon;
            weaponInfo.itemName.text = item.name_item;
            weaponInfo.weaponType.text = stats.weaponType;
            weaponInfo.damageType.text = stats.damageType;
            weaponInfo.skillName.text = stats.skillName;
            weaponInfo.weight.text = stats.weightCost.ToString();
            weaponInfo.durability_cur.text = stats.maxDurability.ToString();
            weaponInfo.durability_max.text = stats.maxDurability.ToString();

            c_overlay.skillName.text = stats.skillName;

            UpdateUIAttackDefenseElement(AttackDefenseType.physical, weaponInfo.ap_slots, stats.a_physical.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.magic, weaponInfo.ap_slots, stats.a_magic.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.fire, weaponInfo.ap_slots, stats.a_fire.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.lightning, weaponInfo.ap_slots, stats.a_lightning.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.dark, weaponInfo.ap_slots, stats.a_dark.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.critical, weaponInfo.ap_slots, stats.critical.ToString());

            UpdateUIAttackDefenseElement(AttackDefenseType.frost, weaponInfo.a_effects, stats.a_frost.ToString(), true);
            UpdateUIAttackDefenseElement(AttackDefenseType.curse, weaponInfo.a_effects, stats.a_curse.ToString(), true);
            //UpdateUIAttackDefenseElement(AttackDefenseType.poison, weaponInfo.ap_slots, stats.poison.ToString());

            UpdateUIAttackDefenseElement(AttackDefenseType.physical, weaponInfo.g_absorb, stats.d_physical.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.magic, weaponInfo.g_absorb, stats.d_magic.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.fire, weaponInfo.g_absorb, stats.d_fire.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.lightning, weaponInfo.g_absorb, stats.d_lightning.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.dark, weaponInfo.g_absorb, stats.d_dark.ToString());
            UpdateUIAttackDefenseElement(AttackDefenseType.stability, weaponInfo.g_absorb, stats.stability.ToString());

        }

        void UpdateUIAttackDefenseElement(AttackDefenseType t, List<AttDefType> l, string value, bool onText1 = false) {
            AttDefType s1 = weaponInfo.GetAttDefSlot(l, t);
            if (!onText1)
                s1.slot.text2.text = value;
            else
                s1.slot.text1.text = value;
        }

        void UpdateCenterOverlay(Item item) {
            c_overlay.bigIcon.sprite = item.icon;
            c_overlay.itemName.text = item.name_item;
            c_overlay.itemDescription.text = item.itemDescription;
            c_overlay.skillDescription.text = item.skillDescription;
        }

        void LoadConsumableItem(ResourcesManager rm) {
            string weaponId = curEqSlot.icon.id;
            Item item = rm.GetItem(curEqSlot.icon.id, ItemType.consum);

            UpdateCenterOverlay(item);
        }

        public static InventoryUI singleton;
        void Awake()
        {
            singleton = this;
        }

    }

    public enum EqSlotType
    {
        weapons, arrows, bolts, equipment, rings, covenant, consumables
    }

    public enum UIState
    {
        equipment, inventory, attributes, messages, options
    }

    [System.Serializable]
    public class EquipmentSlotsUI
    {
        public List<EquipmentUISlot> weapon = new List<EquipmentUISlot>();
        public List<EquipmentUISlot> arrow = new List<EquipmentUISlot>();
        public List<EquipmentUISlot> bolt = new List<EquipmentUISlot>();
        public List<EquipmentUISlot> equipment = new List<EquipmentUISlot>();
        public List<EquipmentUISlot> ring = new List<EquipmentUISlot>();
        public List<EquipmentUISlot> cons = new List<EquipmentUISlot>();
        public EquipmentUISlot covenant = new EquipmentUISlot();

        public void UpdateEqSlot(string itemId, EquipmentUISlot s, ItemType itemType) {
            Item item = ResourcesManager.singleton.GetItem(itemId, itemType);
            s.icon.icon.sprite = item.icon;
            s.icon.icon.enabled = true; 
            s.icon.id = item.item_id;
        }

        public void AddSlotOnList(EquipmentUISlot eq)
        {
            switch (eq.slotType)
            {
                case EqSlotType.weapons:
                    weapon.Add(eq);
                    break;
                case EqSlotType.arrows:
                    arrow.Add(eq);
                    break;
                case EqSlotType.bolts:
                    bolt.Add(eq);
                    break;
                case EqSlotType.equipment:
                    equipment.Add(eq);
                    break;
                case EqSlotType.rings:
                    ring.Add(eq);
                    break;
                case EqSlotType.covenant:
                    covenant = eq;
                    break;
                case EqSlotType.consumables:
                    cons.Add(eq);
                    break;
                default:
                    break;
            }
        }
    }

    [System.Serializable]
    public class EquipmentLeft
    {
        public Text slotName;
        public Text curItem;
        public Left_Inventory inventory;
    }

    [System.Serializable]
    public class PlayerStatus
    {
        public GameObject slotTemplate;
        public GameObject doubleSlot_template;
        public GameObject emptySlot;
        public Transform attGrid;
        public Transform apGrid;
        public Transform defGrid;
        public Transform resGrid;
        public List<AttributeSlot> attSlot = new List<AttributeSlot>();
        public List<AttackPowerSlot> apSlots = new List<AttackPowerSlot>();
        public List<PlayerStatusDef> defSlots = new List<PlayerStatusDef>();
        public List<PlayerStatusDef> resSlots = new List<PlayerStatusDef>();
    }

    [System.Serializable]
    public class Left_Inventory
    {
        public Slider invSlider;
        public GameObject slotTemplate;
        public Transform slotGrid;
    }

    [System.Serializable]
    public class CenterOverlay
    {
        public Image bigIcon;
        public Text itemName;
        public Text itemDescription;
        public Text skillName;
        public Text skillDescription;
    }

    #region WeaponInfo
    [System.Serializable]
    public class WeaponInfo
    {
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

        public AttributeSlot GetAttributeSlot(List<AttributeSlot> l, AttributeType type)
        {
            for (int i = 0; i < l.Count; i++)
            {
                if (l[i].type == type)
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
    public class AttributeSlot
    {
        public bool isBreak;
        public AttributeType type;
        public InventoryUISlot slot;
    }

    [System.Serializable]
    public class AttDefType
    {
        public bool isBreak;
        public AttackDefenseType type;
        public InventoryUISlot slot;
    }

    #endregion

    [System.Serializable]
    public class AttackPowerSlot
    {
        public InventoryUISlot slot;
    }

    [System.Serializable]
    public class PlayerStatusDef
    {
        public AttackDefenseType type;
        public InventoryUIDoubleSlot slot;
    }
}
