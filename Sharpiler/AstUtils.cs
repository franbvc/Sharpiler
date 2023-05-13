using System.Linq.Expressions;

namespace Sharpiler;

public partial class BinOp : INode
{
    private static void CompareBool(string op)
    {
        WriteAsm.Write("AND EAX, 1");
        WriteAsm.Write("AND EBX, 1");

        switch (op)
        {
            case "&&":
                WriteAsm.Write("AND EAX, EBX");
                return;
            case "||":
                WriteAsm.Write("OR EAX, EBX");
                return;
            default:
                throw new Exception($"Should never throw! Invalid op: {op}");
        }
    }

    private static void CompareInt(string op)
    {
        WriteAsm.Write("CMP EAX, EBX");

        switch (op)
        {
            case "==":
                WriteAsm.Write("SETE AL");
                break;
            case ">":
                WriteAsm.Write("SETG AL");
                break;
            case "<":
                WriteAsm.Write("SETL AL");
                break;

            default:
                throw new Exception($"Should never throw! Invalid op: {op}");
        }

        WriteAsm.Write("MOVZX EAX, AL");
    }

    private readonly Dictionary<string, Action> _operatorAction = new Dictionary<string, Action>()
    {
        { "+", () => WriteAsm.Write("ADD EAX, EBX") },
        { "-", () => WriteAsm.Write("SUB EAX, EBX") },
        { "*", () => WriteAsm.Write("IMUL EAX, EBX") },
        { "/", () => WriteAsm.Write("IDIV EBX") },
        { "&&", () => CompareBool("&&") },
        { "||", () => CompareBool("||")},
        { ">", () => CompareInt(">") },
        { "<", () => CompareInt("<") },
        { "==", () => CompareInt("==") },
    };

   
}