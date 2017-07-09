using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utilities {
    [ExecuteInEditMode]
    public class ItemToXML : MonoBehaviour
    {
        public bool make;
        public List<ItemInstance> candidates = new List<ItemInstance>();
        public string xml_version = "<?xml version = \"1.0\" encoding = \"UTF-8\"?>";
        public string targetName;

        void Update() {
            if (!make)
                return;
            make = false;

            string xml = xml_version;
            xml += "\n";
            xml += "<root>";

            foreach (ItemInstance item in candidates)
	        {
                Weapon w = item.instance;
                xml += "<weapon>" + "\n";
                xml += "<weaponId>" + w.weaponId + "</weaponId>" + "\n";
                xml += "<oh_idle>" + w.oh_idle + "</oh_idle>" + "\n";
                xml += "<th_idle>" + w.th_idle + "</th_idle>" + "\n";

                xml += ActionsToString(w.actions, "actions");
                xml += ActionsToString(w.twoHandedActions, "twoHandedActions");

                xml += "<parryMultiplier>" + w.parryMultiplier + "</parryMultiplier>" + "\n";
                xml += "<backstabMultiplier>" + w.backstabMultiplier + "</backstabMultiplier>" + "\n";
                xml += "<leftHandMirror>" + w.leftHandMirror + "</leftHandMirror>" + "\n";
                xml += "<model_pos_x>" + w.model_pos.x + "</model_pos_x>";
                xml += "<model_pos_y>" + w.model_pos.y + "</model_pos_y>";
                xml += "<model_pos_z>" + w.model_pos.z + "</model_pos_z>" + "\n";
                xml += "<model_eulers_x>" + w.model_eulers.x + "</model_eulers_x>";
                xml += "<model_eulers_y>" + w.model_eulers.y + "</model_eulers_y>";
                xml += "<model_eulers_z>" + w.model_eulers.z + "</model_eulers_z>" + "\n";
                xml += "<model_scale_x>" + w.model_scale.x + "</model_scale_x>";
                xml += "<model_scale_y>" + w.model_scale.y + "</model_scale_y>";
                xml += "<model_scale_z>" + w.model_scale.z + "</model_scale_z>" + "\n";
                xml += "</weapon>" + "\n";
	        }

            xml += "</root>";
            
            string path = StaticStrings.SaveLocation() + StaticStrings.itemFolder;
            if (string.IsNullOrEmpty(targetName))
                targetName = "items_database.xml";
            File.WriteAllText(path + targetName, xml);
        }

        string ActionsToString(List<Action> l, string nodeName) {
            string xml = null;
            foreach (Action a in l)
            {
                xml += "<" + nodeName + ">" + "\n";
                xml += "<ActionInput>" + a.input.ToString() + "</ActionInput>" + "\n";
                xml += "<ActionType>" + a.type.ToString() + "</ActionType>" + "\n";
                xml += "<targetAnim>" + a.targetAnim + "</targetAnim>" + "\n";
                xml += "<mirror>" + a.mirror + "</mirror>" + "\n";
                xml += "<canBeParried>" + a.canBeParried + "</canBeParried>" + "\n";
                xml += "<changeSpeed>" + a.changeSpeed + "</changeSpeed>" + "\n";
                xml += "<animSpeed>" + a.animSpeed.ToString() + "</animSpeed>" + "\n";
                xml += "<canParry>" + a.canParry + "</canParry>" + "\n";
                xml += "<canBackstab>" + a.canBackstab + "</canBackstab>" + "\n";
                xml += "<overrideDamageAnim>" + a.overrideDamageAnim + "</overrideDamageAnim>" + "\n";
                xml += "<damageAnim>" + a.damageAnim + "</damageAnim>" + "\n";

                WeaponStats s = a.weaponStats;
                xml += "<physical>" + s.physical + "</physical>" + "\n";
                xml += "<strike>" + s.strike + "</strike>" + "\n";
                xml += "<slash>" + s.slash + "</slash>" + "\n";
                xml += "<thrust>" + s.thrust + "</thrust>" + "\n";
                xml += "<magic>" + s.magic + "</magic>" + "\n";
                xml += "<fire>" + s.fire + "</fire>" + "\n";
                xml += "<lightning>" + s.lightning + "</lightning>" + "\n";
                xml += "<dark>" + s.dark + "</dark>" + "\n";

                xml += "</"+ nodeName + ">" + "\n";
            }
            return xml;
        }

    }
}

