using Verse;

namespace StockpileStackLimit;

internal static class PlaceSpotQualityAtPatch
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