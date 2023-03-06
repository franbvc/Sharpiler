using System.Runtime.InteropServices;
using Console = System.Console;

namespace Sharpiler;

public class Parser
{
    private static Tokenizer? _tk;

    private static void RemoveComment(ref string inputCode)
    {
        int index = inputCode.IndexOf('#');
        if (index >= 0) inputCode = inputCode.Substring(0, index);
    }

    private static int ParseExpression()
    {
        if (_tk == null) throw new Exception();
        int result = ParseTerm();

        while (true)
        {
            switch (_tk.Next.Type)
            {
                case "PLUS":
                    _tk.SelectNext();
                    result += ParseTerm();
                    continue;
                case "MINUS":
                    _tk.SelectNext();
                    result -= ParseTerm();
                    continue;
                case "EOF":
                    goto End;
                case "RPAREN":
                    goto End;
                default:
                    throw new SyntaxException("Wrong token order");
            }
        }

        End:
        return result;
    }

    private static int ParseTerm()
    {
        if (_tk == null) throw new Exception();
        int result = ParseFactor();

        while (true)
        {
            switch (_tk.Next.Type)
            {
                case "MULT":
                    _tk.SelectNext();
                    result *= ParseFactor();
                    continue;
                case "DIV":
                    _tk.SelectNext();
                    result /= ParseFactor();
                    continue;
                default:
                    return result;
            }
        }
    }

    private static int ParseFactor()
    {
        if (_tk == null) throw new Exception();
        if (!_tk.IsNextFactorSymbol()) throw new SyntaxException("Wrong token order");

        switch (_tk.Next.Type)
        {
            case "INT":
                int returnInt = _tk.Next.Value;
                _tk.SelectNext();
                return returnInt;
            case "MINUS":
                _tk.SelectNext();
                return -ParseFactor();
            case "PLUS":
                _tk.SelectNext();
                return ParseFactor();
            case "LPAREN":
                _tk.SelectNext();
                int result = ParseExpression();
                if (_tk.Next.Type != "RPAREN") throw new SyntaxException("Wrong token order");
                _tk.SelectNext();
                return result;
            default:
                throw new SyntaxException("Wrong token order");
        }
    }

    public static int Run(string code)
    {
        RemoveComment(ref code);
        _tk = new Tokenizer(code);
        _tk.SelectNext();
        return ParseExpression();
    }
}

class Test
{
    static void Main(string[] args)
    {
        Console.WriteLine(Parser.Run(args[0]));
    }
}