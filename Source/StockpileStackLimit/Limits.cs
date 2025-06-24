using System.Collections.Generic;
using RimWorld;
using Verse;

namespace StockpileStackLimit;

public static class Limits
{
    private static readonly Dictionary<StorageSettings, int> limits = new();

    public static int CalculateStackLimit(Thing t)
    {
        return CalculateStackLimit(t, out _);
    }

    public static int CalculateStackLimit(Thing t, out SlotGroup slotGroup)
    {
        slotGroup = null;
        if (!t.Spawned)
        {
            return t.def.stackLimit;
        }

        ISlotGroup slotGroupCell = t.Map.haulDestinationManager.SlotGroupAt(t.Position);
        ISlotGroup storageGroup = slotGroupCell?.StorageGroup;
        var slotgroup = storageGroup ?? slotGroupCell;

        //var slotgroup = t.Map.haulDestinationManager.SlotGroupAt(t.Position); //t.GetSlotGroup();
        if (slotgroup == null)
        {
            return t.def.stackLimit;
        }

        var limit = GetLimit(slotgroup.Settings);
        return limit > 0 ? limit : t.def.stackLimit;
    }

    public static int CalculateStackLimit(SlotGroup slotGroup)
    {
        if (slotGroup == null)
        {
            return 99999;
        }

        var setting = slotGroup.Settings;
        var limit = GetLimit(setting);
        return limit > 0 ? limit : 99999;
    }

    public static int CalculateStackLimit(Map map, IntVec3 cell)
    {
        var slotGroup = map.haulDestinationManager.SlotGroupAt(cell);
        if (slotGroup == null)
        {
            return 99999;
        }

        var limit = GetLimit(slotGroup.Settings);
        if (limit <= 0)
        {
            return 99999;
        }

        if (!cell.TryGetFirstThing<Thing>(map, out var thing))
        {
            return limit;
        }

        return limit - thing.stackCount;
    }

    public static bool HasStackLimit(Thing t)
    {
        if (!t.Spawned)
        {
            return false;
        }

        var slotGroup = t.Map.haulDestinationManager.SlotGroupAt(t.Position); //t.GetSlotGroup();
        if (slotGroup == null)
        {
            return false;
        }

        var setting = slotGroup.Settings;
        return hasLimit(setting);
    }

    public static int GetLimit(StorageSettings settings)
    {
        return limits.GetValueOrDefault(settings, -1);
    }

    public static void SetLimit(StorageSettings settings, int limit)
    {
        _ = settings.owner as ISlotGroupParent;
        //t.Map.listerHaulables.RecalcAllInCells(t.AllSlotCells());
        limits[settings] = limit;
    }

    private static bool hasLimit(StorageSettings settings)
    {
        return limits.ContainsKey(settings) && limits[settings] > 0;
    }
}