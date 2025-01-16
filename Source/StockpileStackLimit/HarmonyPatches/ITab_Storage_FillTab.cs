using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(ITab_Storage), "FillTab")]
internal class ITab_Storage_FillTab
{
    public static ITab_Storage currentTab;

    public static void Prefix(ITab_Storage __instance)
    {
        currentTab = __instance;
    }

    public static void Postfix()
    {
        currentTab = null;
    }
}