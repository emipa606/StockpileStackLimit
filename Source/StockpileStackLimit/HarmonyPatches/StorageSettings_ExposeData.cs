using HarmonyLib;
using RimWorld;
using Verse;

namespace StockpileStackLimit.HarmonyPatches;

[HarmonyPatch(typeof(StorageSettings), nameof(StorageSettings.ExposeData))]
internal class StorageSettings_ExposeData
{
    public static void Postfix(StorageSettings __instance)
    {
        switch (Scribe.mode)
        {
            case LoadSaveMode.Saving:
                Scribe.saver.WriteElement("stacklimit", Limits.GetLimit(__instance).ToString());
                break;
            case LoadSaveMode.LoadingVars:
                Limits.SetLimit(__instance,
                    ScribeExtractor.ValueFromNode(Scribe.loader.curXmlParent["stacklimit"], -1));
                break;
        }
    }
}