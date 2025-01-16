using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(StorageSettingsClipboard), nameof(StorageSettingsClipboard.Copy))]
public static class StorageSettingsClipboard_Copy
{
    public static int clipboardLimit = -1;

    public static void Postfix(StorageSettings s)
    {
        clipboardLimit = Limits.GetLimit(s);
    }
}