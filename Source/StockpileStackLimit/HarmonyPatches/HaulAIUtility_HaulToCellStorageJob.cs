using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(HaulAIUtility), nameof(HaulAIUtility.HaulToCellStorageJob))]
public static class HaulAIUtility_HaulToCellStorageJob
{
    public static bool Prefix(Pawn p, Thing t, IntVec3 storeCell, ref Job __result)
    {
        var targetLimit = Limits.CalculateStackLimit(p.Map, storeCell);
        if (targetLimit == 0)
        {
            __result = null;
            return false;
        }

        var limit = Limits.CalculateStackLimit(t, out var slotGroup);
        Job job;
        var movable = Mathf.Clamp(t.stackCount - limit, 0, targetLimit);
        if (t.stackCount > limit)
        {
            job = new Job(JobDefOf.HaulToCell, t, storeCell)
            {
                count = movable,
                haulOpportunisticDuplicates = true,
                haulMode = HaulMode.ToCellStorage
            };
            __result = job;
            Mod.Debug($"dispatch job1, thing={t},cell={storeCell}");
            return false;
        }

        //limit = Limits.CalculateStackLimit(p.Map.haulDestinationManager.SlotGroupAt(storeCell));
        limit = Limits.CalculateStackLimit(slotGroup);
        if (limit >= 99999)
        {
            Mod.Debug($"dispatch job3, thing={t},cell={storeCell}");
            return true;
        }

        job = new Job(JobDefOf.HaulToCell, t, storeCell);
        var thing = p.Map.thingGrid.ThingAt(storeCell, t.def);
        job.count = limit - thing?.stackCount ?? limit;

        job.haulOpportunisticDuplicates = false;
        job.haulMode = HaulMode.ToCellStorage;
        __result = job;
        Mod.Debug($"dispatch job2, thing={t},cell={storeCell}");
        return false;
    }
}