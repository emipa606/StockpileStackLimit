using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(MassUtility), nameof(MassUtility.CountToPickUpUntilOverEncumbered))]
internal class MassUtility_CountToPickUpUntilOverEncumbered
{
    private static void Postfix(Thing thing, ref int __result)
    {
        if (!Limits.HasStackLimit(thing))
        {
            return;
        }

        var t = thing.stackCount - Limits.CalculateStackLimit(thing);
        if (t < 0)
        {
            t = 0;
        }

        __result = Mathf.Min(t, __result);
    }
}