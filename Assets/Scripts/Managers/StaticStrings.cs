using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StaticStrings
{
    //Input
    public static string Vertical = "Vertical";
    public static string Horizontal = "Horizontal";
    public static string B = "B";
    public static string A = "A";
    public static string X = "X";
    public static string Y = "Y";
    public static string RT = "RT";
    public static string LT = "LT";
    public static string RB = "RB";
    public static string LB = "LB";
    public static string L = "L";
    public static string Pad_X = "Pad X";
    public static string Pad_Y = "Pad Y";

    //Animator Parameters
    public static string vertical = "vertical";
    public static string horizontal = "horizontal";
    public static string mirror = "mirror";
    public static string parry_attack = "parry_attack";
    public static string animSpeed = "animSpeed";
    public static string onGround = "onGround";
    public static string run = "run";
    public static string interacting = "interacting";
    public static string blocking = "blocking";
    public static string isLeft = "isLeft";
    public static string canMove = "canMove";
    public static string lockon = "lockon";
    public static string spellCasting = "spellCasting";

    //Animator States
    public static string Rolls = "Rolls";
    public static string attack_interrupt = "attack_interrupt";
    public static string parry_received = "parry_received";
    public static string backstabbed = "backstabbed";
    public static string damage_1 = "damage_1";
    public static string damage_2 = "damage_2";
    public static string damage_3 = "damage_3";
    public static string changeWeapon = "changeWeapon";
    public static string EmptyBoth = "Empty Both";
    public static string EmptyLeft = "Empty Left";
    public static string EmptyRight = "Empty Right";
    public static string equip_weapon_oh = "equip_weapon_oh";

    //Other
    public static string _r = "_r";
    public static string _l = "_l";

    //Data
    public static string itemFolder = "/Items/";

    public static string SaveLocation() {
        string saveLocation = Application.streamingAssetsPath;

        if (!Directory.Exists(saveLocation))
        {
            Directory.CreateDirectory(saveLocation);
        }
        return saveLocation;
    }
}
