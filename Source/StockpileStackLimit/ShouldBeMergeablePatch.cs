using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit;

[HarmonyPatch(typeof(ListerMergeables), "ShouldBeMergeable")]
public class ShouldBeMergeablePatch
{
    private static void Postfix(Thing t, ref bool __result)
    {
        if (__result)
        {
            __result = !Limits.HasStackLimit(t) && t.stackCount != Limits.CalculateStackLimit(t);
        }
    }
}