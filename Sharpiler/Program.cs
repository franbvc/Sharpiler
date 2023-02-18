namespace Sharpiler;

public class Parser
{
    private static Tokenizer? _tk;

    private static int ParseExpression()
    {
        if (_tk == null) throw new Exception();
        if (!_tk.IsNextInt()) throw new SyntaxException("Expression doesn't start with a Number");
        int result = _tk.Next.Value;

        while (true)
        {
            _tk.SelectNext();
            switch (_tk.Next.Type)
            {
                case "PLUS":
                    _tk.SelectNext();
                    if (_tk.IsNextInt())
                        result += _tk.Next.Value;
                    else throw new SyntaxException("Wrong token order");

                    continue;

                case "MINUS":
                    _tk.SelectNext();
                    if (_tk.IsNextInt())
                        result -= _tk.Next.Value;
                    else throw new SyntaxException("Wrong token order");

                    continue;

                case "EOF":
                    return result;

                default:
                    throw new SyntaxException("Wrong token order");
            }
        }
    }

    public static int Run(string code)
    {
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

    