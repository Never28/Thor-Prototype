using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatsCalculations {

    public static int CalculateBaseDamage(WeaponStats wStats, CharacterStats cStats, float multiplier = 1) {

        float physical = (wStats.a_physical * multiplier) - cStats.physical;
        float slash = (wStats.a_slash * multiplier) - cStats.vs_slash;
        float strike = (wStats.a_strike * multiplier) - cStats.vs_strike;
        float thrust = (wStats.a_thrust * multiplier) - cStats.vs_thrust;

        float sum = physical + slash + strike + thrust;

        float magic = (wStats.a_magic * multiplier) - cStats.magic;
        float fire = (wStats.a_fire * multiplier) - cStats.fire;
        float lightning = (wStats.a_lightning * multiplier) - cStats.lightning;
        float dark = (wStats.a_dark * multiplier) - cStats.dark;

        sum += magic + fire + lightning + dark;

        if (sum < 0)
            sum = 0;

        return Mathf.RoundToInt(sum);
    }
}
