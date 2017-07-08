using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatsCalculations {

    public static int CalculateBaseDamage(WeaponStats wStats, CharacterStats cStats, float multiplier = 1) {

        float physical = (wStats.physical * multiplier) - cStats.physical;
        float slash = (wStats.slash * multiplier) - cStats.vs_slash;
        float strike = (wStats.strike * multiplier) - cStats.vs_strike;
        float thrust = (wStats.thrust * multiplier) - cStats.vs_thrust;

        float sum = physical + slash + strike + thrust;

        float magic = (wStats.magic * multiplier) - cStats.magic;
        float fire = (wStats.fire * multiplier) - cStats.fire;
        float lightning = (wStats.lightning * multiplier) - cStats.lightning;
        float dark = (wStats.dark * multiplier) - cStats.dark;

        sum += magic + fire + lightning + dark;

        if (sum < 0)
            sum = 0;

        return Mathf.RoundToInt(sum);
    }
}
