using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(Thing), nameof(Thing.TryAbsorbStack))]
internal class Thing_TryAbsorbStack
{
    public static bool Prefix(Thing __instance, Thing other, bool respectStackLimit, ref bool __result)
    {
        Mod.Debug("Thing.TryAbsorbStack begin");
        if (!Limits.HasStackLimit(__instance))
        {
            Mod.Debug($"{__instance} has no stacklimit");
            return true;
        }

        if (!__instance.CanStackWith(other))
        {
            Mod.Debug($"{__instance} cannot stack with {other}");
            __result = false;
            return false;
        }

        int num;
        if (respectStackLimit)
        {
            var t = Limits.CalculateStackLimit(__instance) - __instance.stackCount;
            Mod.Debug($"stackcount is {__instance.stackCount}, t is {t}");
            if (t < 0)
            {
                t = 0;
            }

            num = Mathf.Min(other.stackCount, t);
            Mod.Debug($"num is {num}");
        }
        else
        {
            num = other.stackCount;
            Mod.Debug($"{num} is set to other.stackCount");
        }

        if (num <= 0)
        {
            Mod.Debug($"{num} is under 0");
            __result = false;
            return true;
        }

        if (__instance.def.useHitPoints)
        {
            Mod.Debug($"{__instance} should set hitpoints");
            __instance.HitPoints =
                Mathf.CeilToInt(((__instance.HitPoints * __instance.stackCount) + (other.HitPoints * num)) /
                                (float)(__instance.stackCount + num));
        }

        __instance.stackCount += num;
        other.stackCount -= num;
        StealAIDebugDrawer.Notify_ThingChanged(__instance);
        if (__instance.Spawned)
        {
            Mod.Debug($"{__instance} Notify_ThingStackChanged");
            __instance.Map.listerMergeables.Notify_ThingStackChanged(__instance);
        }

        if (other.stackCount <= 0)
        {
            Mod.Debug($"{other} stackcount under 0");
            other.Destroy();
            __result = true;
        }
        else
        {
            __result = false;
        }

        return false;
    }
}