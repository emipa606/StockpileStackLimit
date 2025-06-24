using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(ITab_Storage), "FillTab")]
internal class ITab_Storage_FillTab
{
    public static ITab_Storage CurrentTab;

    public static void Prefix(ITab_Storage __instance)
    {
        CurrentTab = __instance;
    }

    public static void Postfix()
    {
        CurrentTab = null;
    }
}