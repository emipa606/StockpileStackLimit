using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit;

[HarmonyPatch(typeof(StoreUtility), "TryFindBestBetterStoreCellFor")]
internal class FindBestBetterStoreCellPatcher
{
    public static void Prefix(Thing t, ref StoragePriority currentPriority)
    {
        if (t.stackCount > Limits.CalculateStackLimit(t))
        {
            currentPriority = StoragePriority.Unstored;
        }
    }
}