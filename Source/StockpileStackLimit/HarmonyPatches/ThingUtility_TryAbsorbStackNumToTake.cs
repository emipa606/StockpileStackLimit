using HarmonyLib;
using UnityEngine;
using Verse;

namespace StockpileStackLimit.HarmonyPatches;

public static class ThingUtility_TryAbsorbStackNumToTake
{
    [HarmonyPriority(Priority.Low)]
    public static bool Prefix(ref int __result, Thing thing, Thing other, bool respectStackLimit)
    {
        if (respectStackLimit)
        {
            var t = Limits.CalculateStackLimit(thing) - thing.stackCount;
            if (t < 0)
            {
                t = 0;
            }

            __result = Mathf.Min(other.stackCount, t);
        }
        else
        {
            __result = other.stackCount;
        }

        return false;
    }
}