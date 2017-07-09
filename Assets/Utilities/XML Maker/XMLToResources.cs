using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System;

namespace Utilities {
    [ExecuteInEditMode]
    public class XMLToResources : MonoBehaviour
    {
        public bool load;

        public ResourcesManager resourcesManager;
        public string weaponFileName = "items_database.xml";

        // Update is called once per frame
        void Update()
        {
            if (!load)
                return;
            load = false;
             
            LoadWeaponData(resourcesManager);
        }

        public void LoadWeaponData(ResourcesManager rm) {
            string filePath = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            filePath += weaponFileName;

            XmlDocument doc = new XmlDocument();
            doc.Load(filePath);

            foreach (XmlNode w in doc.DocumentElement.SelectNodes("//weapon"))
            {
                Weapon _w = new Weapon();
                _w.actions = new List<Action>();
                _w.twoHandedActions = new List<Action>();
                XmlNode weaponId = w.SelectSingleNode("weaponId");
                _w.weaponId = weaponId.InnerText;
                XmlNode oh_idle = w.SelectSingleNode("oh_idle");
                _w.oh_idle = oh_idle.InnerText;
                XmlNode th_idle = w.SelectSingleNode("th_idle");
                _w.th_idle = th_idle.InnerText;

                XMLToActions(doc, "actions", ref _w);
                XMLToActions(doc, "twoHandedActions", ref _w);

                XmlNode parryMultiplier = w.SelectSingleNode("parryMultiplier");
                float.TryParse(parryMultiplier.InnerText, out _w.parryMultiplier);
                XmlNode backstabMultiplier = w.SelectSingleNode("backstabMultiplier");
                float.TryParse(backstabMultiplier.InnerText, out _w.backstabMultiplier);
                XmlNode leftHandMirror = w.SelectSingleNode("leftHandMirror");
                _w.leftHandMirror = leftHandMirror.InnerText == "True";

                resourcesManager.weapons.Add(_w);
            }
        }

        void XMLToActions(XmlDocument doc, string nodeName, ref Weapon _w)
        {
            foreach (XmlNode a in doc.DocumentElement.SelectNodes("//" + nodeName))
            {
                Action _a = new Action();
                
                XmlNode actionInput = a.SelectSingleNode("ActionInput");
                _a.input = (ActionInput)Enum.Parse(typeof(ActionInput), actionInput.InnerText);
                XmlNode actionType = a.SelectSingleNode("ActionType");
                _a.type = (ActionType)Enum.Parse(typeof(ActionType), actionType.InnerText);
                XmlNode targetAnim = a.SelectSingleNode("targetAnim");
                _a.targetAnim = targetAnim.InnerText;
                XmlNode mirror = a.SelectSingleNode("mirror");
                _a.mirror = mirror.InnerText == "True";
                XmlNode canBeParried = a.SelectSingleNode("canBeParried");
                _a.canBeParried = canBeParried.InnerText == "True";
                XmlNode changeSpeed = a.SelectSingleNode("changeSpeed");
                _a.changeSpeed = changeSpeed.InnerText == "True";
                XmlNode animSpeed = a.SelectSingleNode("animSpeed");
                float.TryParse(animSpeed.InnerText, out _a.animSpeed);
                XmlNode canParry = a.SelectSingleNode("canParry");
                _a.canParry = canParry.InnerText == "True";
                XmlNode canBackstab = a.SelectSingleNode("canBackstab");
                _a.canBackstab = canBackstab.InnerText == "True";
                XmlNode overrideDamageAnim = a.SelectSingleNode("overrideDamageAnim");
                _a.overrideDamageAnim = overrideDamageAnim.InnerText == "True";
                XmlNode damageAnim = a.SelectSingleNode("damageAnim");
                _a.damageAnim = damageAnim.InnerText;

                _a.weaponStats = new WeaponStats();

                XmlNode physical = a.SelectSingleNode("physical");
                int.TryParse(physical.InnerText, out _a.weaponStats.physical);
                XmlNode strike = a.SelectSingleNode("strike");
                int.TryParse(strike.InnerText, out _a.weaponStats.strike);
                XmlNode slash = a.SelectSingleNode("slash");
                int.TryParse(slash.InnerText, out _a.weaponStats.slash);
                XmlNode thrust = a.SelectSingleNode("thrust");
                int.TryParse(thrust.InnerText, out _a.weaponStats.thrust);
                XmlNode magic = a.SelectSingleNode("magic");
                int.TryParse(magic.InnerText, out _a.weaponStats.magic);
                XmlNode fire = a.SelectSingleNode("fire");
                int.TryParse(fire.InnerText, out _a.weaponStats.fire);
                XmlNode lightning = a.SelectSingleNode("lightning");
                int.TryParse(lightning.InnerText, out _a.weaponStats.lightning);
                XmlNode dark = a.SelectSingleNode("dark");
                int.TryParse(dark.InnerText, out _a.weaponStats.dark);

                if (nodeName == "actions")
                    _w.actions.Add(_a);
                else
                    _w.twoHandedActions.Add(_a);
            }
        }

    }

}
