using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace StockpileStackLimit;

internal class InsertTranspiler : ITranspiler
{
    private readonly List<CodeInstruction> list;

    public InsertTranspiler(List<CodeInstruction> codes)
    {
        if (codes is not { Count: > 0 })
        {
            throw new Exception();
        }

        list = codes;
    }

    public IEnumerable<CodeInstruction> TransMethod(TranspilerFactory factory)
    {
        _ = factory.CodeEnumerator;
        var generator = factory.Generator;
        var locals = factory.Locals;
        var labels = factory.Labels;
        foreach (var code in list)
        {
            var no = code.opcode.Value;
            if (code.opcode == CodeParser.LocalvarOpcode)
            {
                locals.Add(generator.DeclareLocal((Type)code.operand));
                continue;
            }

            if (code.opcode == CodeParser.LabelOpcode)
            {
                var index = (int)code.operand;
                for (var i = labels.Count - 1; i < index; i++)
                {
                    labels.Add(generator.DefineLabel());
                }

                var t = new List<Label> { labels[index] };
                yield return new CodeInstruction(OpCodes.Nop) { labels = t };
                continue;
            }

            if (no is 17 or 18 or 19 or -500 or -499 or -498)
            {
                code.operand = locals[Convert.ToInt32(code.operand)];
                yield return code;
                continue;
            }

            if (code.opcode.OperandType == OperandType.InlineBrTarget ||
                code.opcode.OperandType == OperandType.ShortInlineBrTarget)
            {
                var index = (int)code.operand;
                for (var i = labels.Count - 1; i < index; i++)
                {
                    labels.Add(generator.DefineLabel());
                }

                code.operand = labels[index];
            }

            yield return code;
        }
    }
}