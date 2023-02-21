using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit;

[HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
public static class NoStorageBlockersInPatch
{
    public static void Postfix(ref bool __result, IntVec3 c, Map map, Thing thing)
    {
        var targetThing = map.thingGrid.ThingAt(c, thing.def);
        if (targetThing == null)
        {
            return;
        }

        var limit = Limits.CalculateStackLimit(thing, out _);
        if (targetThing.stackCount >= limit)
        {
            __result = false;
        }
    }
}