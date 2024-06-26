using System;
using System.Collections.Generic;
using HarmonyLib;

namespace StockpileStackLimit;

internal class SearchTranspiler : ITranspiler
{
    private readonly List<CodeInstruction> search;

    public SearchTranspiler(List<CodeInstruction> codes)
    {
        if (codes is not { Count: > 0 })
        {
            throw new Exception();
        }

        search = codes;
    }

    public IEnumerable<CodeInstruction> TransMethod(TranspilerFactory factory)
    {
        var queue = new Queue<CodeInstruction>();
        var instructions = factory.CodeEnumerator;
        while (instructions.MoveNext())
        {
            var current = instructions.Current;
            queue.Enqueue(current);
            if (queue.Count > search.Count)
            {
                yield return queue.Dequeue();
            }

            if (queue.Count != search.Count)
            {
                continue;
            }

            var count = 0;
            foreach (var item in queue)
            {
                if (search[count].IsMatchWith(item))
                {
                    count++;
                }
                else
                {
                    break;
                }
            }

            if (count < search.Count)
            {
                continue;
            }

            while (queue.Count > 0)
            {
                yield return queue.Dequeue();
            }

            yield break;
        }

        while (queue.Count > 0)
        {
            yield return queue.Dequeue();
        }
    }
}