using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using HarmonyLib;

namespace StockpileStackLimit;

public static class CodeParser
{
    private static readonly OpCode anyOpcode =
        (OpCode)typeof(OpCode).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
            .Invoke([256, -257283419]);

    private static readonly object anyOperand = "*";

    public static readonly OpCode LocalvarOpcode =
        (OpCode)typeof(OpCode).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
            .Invoke([257, 279317166]);

    public static readonly OpCode LabelOpcode =
        (OpCode)typeof(OpCode).GetConstructors(BindingFlags.NonPublic | BindingFlags.Instance)[0]
            .Invoke([258, 279317166]);

    private static readonly Regex matchMethod = new(@"^(.*?)::?(.*?)(?:\((.*?)\))?$");

    private static readonly Dictionary<string, Type> keywordTypes = new()
    {
        { "bool", typeof(bool) }, { "byte", typeof(byte) }, { "char", typeof(char) },
        {
            "decimal", typeof(decimal)
        },
        { "double", typeof(double) }, { "float", typeof(float) }, { "int", typeof(int) }, { "long", typeof(long) },
        { "sbyte", typeof(sbyte) }, { "short", typeof(short) }, { "string", typeof(string) }, { "uint", typeof(uint) },
        { "ulong", typeof(ulong) }, { "ushort", typeof(ushort) }
    };

    private static Type String2Type(string str)
    {
        _ = keywordTypes.TryGetValue(str, out var t);
        t ??= AccessTools.TypeByName(str);

        return t;
    }

    private static CodeInstruction parse(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            throw new Exception("String to Parse is null or empty");
        }

        var parts = str.Split([' '], 2);
        var opCodeStr = parts[0];
        var opcode = anyOpcode;
        if (opCodeStr != "*")
        {
            opCodeStr = opCodeStr.ToLower();
            switch (opCodeStr)
            {
                case "localvar":
                    return new CodeInstruction(LocalvarOpcode, String2Type(parts[1]));
                case "label":
                    return new CodeInstruction(LabelOpcode, Convert.ToInt32(parts[1]));
                default:
                    opCodeStr = opCodeStr.Replace('.', '_');
                    opcode = (OpCode)typeof(OpCodes)
                        .GetField(opCodeStr, BindingFlags.IgnoreCase | BindingFlags.Static | BindingFlags.Public)
                        ?.GetValue(null)!;
                    break;
            }
        }

        if (parts.Length == 1 || opcode.OperandType == OperandType.InlineNone)
        {
            return new CodeInstruction(opcode);
        }

        var opRandStr = parts[1];
        if (opRandStr == "*")
        {
            return new CodeInstruction(opcode, anyOperand);
        }

        object obj = null;
        switch (opcode.OperandType)
        {
            case OperandType.InlineMethod:
                var result = matchMethod.Match(opRandStr);
                if (!result.Success)
                {
                    break;
                }

                var type = String2Type(result.Groups[1].Value);
                var method = result.Groups[2].Value;
                var argStr = result.Groups[3].Value;
                Type[] args = null;
                if (argStr != "")
                {
                    var t = argStr.Split(',');
                    args = new Type[t.Length];
                    for (var i = 0; i < t.Length; i++)
                    {
                        var s = t[i].Trim();
                        _ = keywordTypes.TryGetValue(s, out args[i]);
                        args[i] ??= String2Type(s);
                    }
                }

                obj = opcode == OpCodes.Newobj
                    ? AccessTools.Constructor(type, args)
                    : AccessTools.Method(type, method, args);

                break;
            case OperandType.InlineField:
                parts = opRandStr.Split([':'], StringSplitOptions.RemoveEmptyEntries);
                obj = AccessTools.Field(String2Type(parts[0]), parts[1]);
                break;
            case OperandType.InlineString:
                obj = opRandStr;
                break;
            case OperandType.InlineType:
                obj = String2Type(opRandStr);
                break;
            case OperandType.InlineI:
            case OperandType.InlineBrTarget:
            case OperandType.ShortInlineBrTarget:
                obj = Convert.ToInt32(opRandStr);
                break;
            case OperandType.InlineVar:
            case OperandType.ShortInlineI:
                obj = Convert.ToInt16(opRandStr);
                break;
            case OperandType.ShortInlineVar:
            case OperandType.InlineI8:
                obj = Convert.ToByte(opRandStr);
                break;
            case OperandType.InlineR:
                obj = Convert.ToDouble(opRandStr);
                break;
            case OperandType.ShortInlineR:
                obj = Convert.ToSingle(opRandStr);
                break;
        }

        return obj == null
            ? throw new Exception("Unknown OperandType or Wrong operand")
            : new CodeInstruction(opcode, obj);
    }

    public static List<CodeInstruction> ParseMultiple(string str)
    {
        var codes = str.Split([';'], StringSplitOptions.RemoveEmptyEntries);
        var result = new List<CodeInstruction>(codes.Length);
        foreach (var s in codes)
        {
            result.Add(parse(s));
        }

        return result;
    }

    public static bool IsMatchWith(this CodeInstruction codeWithMatchOption, CodeInstruction instr)
    {
        var result = anyOpcode == codeWithMatchOption.opcode || codeWithMatchOption.opcode == instr.opcode;
        result &= anyOperand == codeWithMatchOption.operand ||
                  (codeWithMatchOption.operand?.Equals(instr.operand) ?? null == instr.operand);
        return result;
    }
}