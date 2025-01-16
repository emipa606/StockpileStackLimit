using Verse;

namespace StockpileStackLimit.HarmonyPatches;

internal static class GenPlace_PlaceSpotQualityAt
{
    public static bool Prefix(IntVec3 c, Map map, Thing thing, ref byte __result)
    {
        Mod.Debug("GenPlace.PlaceSpotQualityAt begin");
        if (thing.stackCount < Limits.CalculateStackLimit(map, c))
        {
            return true;
        }

        __result = 0;
        return false;
    }
}