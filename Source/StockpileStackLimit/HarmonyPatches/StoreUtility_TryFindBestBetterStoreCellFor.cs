using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(StoreUtility), nameof(StoreUtility.TryFindBestBetterStoreCellFor))]
internal class StoreUtility_TryFindBestBetterStoreCellFor
{
    public static void Prefix(Thing t, ref StoragePriority currentPriority)
    {
        if (t.stackCount > Limits.CalculateStackLimit(t))
        {
            currentPriority = StoragePriority.Unstored;
        }
    }
}