using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatsCalculations {

    public static int CalculateBaseDamage(WeaponStats wStats, CharacterStats cStats) {

        int physical = wStats.physical - cStats.physical;
        int slash = wStats.slash - cStats.vs_slash;
        int strike = wStats.strike - cStats.vs_strike;
        int thrust = wStats.thrust - cStats.vs_thrust;

        int sum = physical + slash + strike + thrust;

        int magic = wStats.magic - cStats.magic;
        int fire = wStats.fire - cStats.fire;
        int lightning = wStats.lightning - cStats.lightning;
        int dark = wStats.dark - cStats.dark;

        sum += magic + fire + lightning + dark;

        if (sum < 0)
            sum = 0;

        return sum;
    }
}
