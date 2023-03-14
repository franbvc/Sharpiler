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

    private static INode ParseExpression(bool isSubExpression = false)
    {
        if (_tk == null) throw new Exception();
        INode currentNode;
        INode rootNode = ParseTerm();

        while (true)
        {
            switch (_tk.Next.Type)
            {
                case "PLUS":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp('+', new List<INode>() { currentNode, ParseTerm() });
                    continue;

                case "MINUS":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp('-', new List<INode>() { currentNode, ParseTerm() });
                    continue;
                case "EOF":
                    goto End;
                case "RPAREN":
                    if (isSubExpression) goto End;
                    throw new SyntaxException("Wrong token order");
                default:
                    throw new SyntaxException("Wrong token order");
            }
        }

        End:
        return rootNode;
    }

    private static INode ParseTerm()
    {
        if (_tk == null) throw new Exception();
        INode currentNode;
        INode rootNode = ParseFactor();

        while (true)
        {
            switch (_tk.Next.Type)
            {
                case "MULT":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp('*', new List<INode>() { currentNode, ParseFactor() });
                    continue;
                case "DIV":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp('/', new List<INode>() { currentNode, ParseFactor() });
                    continue;
                default:
                    return rootNode;
            }
        }
    }

    private static INode ParseFactor()
    {
        if (_tk == null) throw new Exception();
        if (!_tk.IsNextFactorSymbol()) throw new SyntaxException("Wrong token order");

        switch (_tk.Next.Type)
        {
            case "INT":
                INode retVal = new IntVal(_tk.Next.Value);
                _tk.SelectNext();
                return retVal;

            case "MINUS":
                _tk.SelectNext();
                return new UnOp('-', new List<INode>() { ParseFactor() });

            case "PLUS":
                _tk.SelectNext();
                return new UnOp('+', new List<INode>() { ParseFactor() });

            case "LPAREN":
                _tk.SelectNext();
                INode result = ParseExpression(true);
                if (_tk.Next.Type != "RPAREN") throw new SyntaxException("Wrong token order");
                _tk.SelectNext();
                return result;
            default:
                throw new SyntaxException("Wrong token order");
        }
    }

    public static INode Run(string code)
    {
        RemoveComment(ref code);
        _tk = new Tokenizer(code);
        _tk.SelectNext();
        return ParseExpression();
    }
}

class Test
{
    // static void Main(string[] args)
    // {
    //     INode astRoot = Parser.Run(args[0]);
    //     Console.WriteLine(astRoot.Evaluate());
    // }
    static void Main(string[] args)
    {
        string filename = args[0];

        try
        {
            string fileContents = File.ReadAllText(filename);
            INode astRoot = Parser.Run(fileContents);
            Console.WriteLine(astRoot.Evaluate());
        }
        catch (IOException e)
        {
            Console.WriteLine("Error reading file: " + e.Message);
            throw;
        }
    }
}