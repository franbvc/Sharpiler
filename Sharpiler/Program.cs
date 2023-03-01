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
        if (!_tk.IsNextInt()) throw new SyntaxException("Wrong token order");
        int result = _tk.Next.Value;

        while (true)
        {
            _tk.SelectNext();
            switch (_tk.Next.Type)
            {
                case "MULT":
                    _tk.SelectNext();
                    if (_tk.IsNextInt())
                        result *= _tk.Next.Value;
                    else throw new SyntaxException("Wrong token order");

                    continue;

                case "DIV":
                    _tk.SelectNext();
                    if (_tk.IsNextInt())
                        result /= _tk.Next.Value;
                    else throw new SyntaxException("Wrong token order");

                    continue;

                default:
                    return result;
            }
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