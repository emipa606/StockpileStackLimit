using HarmonyLib;
using RimWorld;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(StorageSettingsClipboard), nameof(StorageSettingsClipboard.PasteInto))]
public static class StorageSettingsClipboard_PasteInto
{
    public static void Postfix(StorageSettings s)
    {
        Limits.SetLimit(s, StorageSettingsClipboard_Copy.clipboardLimit);
    }
}