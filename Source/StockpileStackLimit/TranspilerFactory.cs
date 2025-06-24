using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

namespace StockpileStackLimit;

public class TranspilerFactory
{
    private readonly TranspilerDelegate getTranspiler;
    private readonly List<ITranspiler> transpiler;
    internal IEnumerator<CodeInstruction> CodeEnumerator;
    internal ILGenerator Generator;
    internal List<Label> Labels;
    internal List<LocalBuilder> Locals;

    public TranspilerFactory()
    {
        transpiler = [];
        getTranspiler = Transpiler;
    }

    public TranspilerFactory Search(string str)
    {
        transpiler.Add(new SearchTranspiler(CodeParser.ParseMultiple(str)));
        return this;
    }

    public TranspilerFactory Search(string str, int num)
    {
        for (var i = 0; i < num; i++)
        {
            transpiler.Add(new SearchTranspiler(CodeParser.ParseMultiple(str)));
        }

        return this;
    }

    public TranspilerFactory Replace(string from, string to)
    {
        transpiler.Add(new SearchDeleteTranspiler(CodeParser.ParseMultiple(from)));
        transpiler.Add(new InsertTranspiler(CodeParser.ParseMultiple(to)));
        return this;
    }

    public TranspilerFactory Replace(string from, string to, int num)
    {
        for (var i = 0; i < num; i++)
        {
            transpiler.Add(new SearchDeleteTranspiler(CodeParser.ParseMultiple(from)));
            transpiler.Add(new InsertTranspiler(CodeParser.ParseMultiple(to)));
        }

        return this;
    }

    public TranspilerFactory Insert(string str)
    {
        transpiler.Add(new InsertTranspiler(CodeParser.ParseMultiple(str)));
        return this;
    }

    public TranspilerFactory Delete(string str)
    {
        transpiler.Add(new SearchDeleteTranspiler(CodeParser.ParseMultiple(str)));
        return this;
    }

    public TranspilerFactory Delete(string str, int num)
    {
        for (var i = 0; i < num; i++)
        {
            transpiler.Add(new SearchDeleteTranspiler(CodeParser.ParseMultiple(str)));
        }

        return this;
    }

    public TranspilerFactory Delete(int num)
    {
        transpiler.Add(new SkipTranspiler(num));
        return this;
    }

    public HarmonyMethod GetTranspilerMethod()
    {
        return new HarmonyMethod(getTranspiler.Method);
    }

    private IEnumerable<CodeInstruction> Transpiler(ILGenerator generator, IEnumerable<CodeInstruction> instr)
    {
        if (transpiler.Count == 0 || transpiler[^1] is not EndingTranspiler)
        {
            transpiler.Add(new EndingTranspiler());
        }

        Generator = generator;
        CodeEnumerator = instr.GetEnumerator();
        Locals = [];
        Labels = [];
        foreach (var t in transpiler)
        {
            foreach (var code in t.TransMethod(this))
            {
                yield return code;
            }
        }
    }
}