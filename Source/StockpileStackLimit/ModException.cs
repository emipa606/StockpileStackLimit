using System;

namespace StockpileStackLimit;

public class ModException(string s) : Exception($"[{Mod.Name}]{s}");