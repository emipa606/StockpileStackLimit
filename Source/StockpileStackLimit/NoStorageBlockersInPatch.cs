using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit;

[HarmonyPatch(typeof(StoreUtility), "NoStorageBlockersIn")]
public static class NoStorageBlockersInPatch
{
    //public static IEnumerable<CodeInstruction> Transpiler(ILGenerator generator,
    //    IEnumerable<CodeInstruction> instructions)
    //{
    //    return new TranspilerFactory()
    //        .Replace("ldarg.2;ldfld *;ldfld *", "ldloc.2;call Limits::CalculateStackLimit(Verse.Thing)")
    //        .Transpiler(generator, instructions);
    //}

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