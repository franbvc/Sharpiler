using Console = System.Console;

namespace Sharpiler;

public partial class Parser
{
    private static Tokenizer? _tk;

    private static INode ParseBlock(bool isSubBlock = false)
    {
        if (_tk == null) throw new Exception();

        INode currentNode;
        List<INode> children = new List<INode>();

        while (true)
        {
            if (!isSubBlock && _tk.Next.Type == "END") throw new SyntaxException("Unmatched END");
            if (!isSubBlock && _tk.Next.Type == "ELSE") throw new SyntaxException("Unmatched ELSE");
            if (_tk.Next.Type is "EOF" or "END" or "ELSE") break;

            currentNode = ParseStatement();
            if (currentNode is not NoOp) children.Add(currentNode);
        }

        return new Block(children);
    }

    private static INode ParseStatement()
    {
        if (_tk == null) throw new Exception();
        INode currentNode;

        switch (_tk.Next.Type)
        {
            case "IDENTIFIER":
                currentNode = ParseStatementIdentifier();
                break;

            case "PRINT":
                _tk.SelectNext();
                if (_tk.Next.Type != "LPAREN") throw new SyntaxException("Wrong token order");
                currentNode = new Print(new List<INode>() { ParseRelativeExpression(true) });
                break;

            case "WHILE":
                _tk.SelectNext();
                currentNode = BuildConditionalNode(ConditionalType.While);
                break;

            case "IF":
                _tk.SelectNext();
                currentNode = BuildConditionalNode(ConditionalType.If);
                break;

            case "NEWLINE":
                _tk.SelectNext();
                return new NoOp();

            default:
                throw new SyntaxException("Wrong token order");
        }

        return currentNode;
    }

    private static INode ParseRelativeExpression(bool isSubExpression = false)
    {
        if (_tk == null) throw new Exception();
        INode currentNode;
        INode rootNode = ParseExpression();

        while (true)
        {
            switch (_tk.Next.Type)
            {
                case "GT":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp(">", new List<INode>() { currentNode, ParseExpression() });
                    continue;

                case "LT":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp("<", new List<INode>() { currentNode, ParseExpression() });
                    continue;

                case "EQ":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp("==", new List<INode>() { currentNode, ParseExpression() });
                    continue;
                
                case "DOT":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp(".", new List<INode>() { currentNode, ParseExpression() });
                    continue;

                case "NEWLINE":
                case "EOF":
                    goto End;

                case "RPAREN":
                    if (isSubExpression) goto End;
                    throw new SyntaxException("Wrong token order: missing ')'");

                default:
                    throw new SyntaxException("Wrong token order: got " + _tk.Next.Type +
                                              " instead of (GT or LT or EQ)");
            }
        }

        End:
        return rootNode;
    }

    private static INode ParseExpression()
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
                    rootNode = new BinOp("+", new List<INode>() { currentNode, ParseTerm() });
                    continue;

                case "MINUS":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp("-", new List<INode>() { currentNode, ParseTerm() });
                    continue;

                case "OR":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp("||", new List<INode>() { currentNode, ParseTerm() });
                    continue;

                default:
                    return rootNode;
            }
        }
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
                    rootNode = new BinOp("*", new List<INode>() { currentNode, ParseFactor() });
                    continue;
                case "DIV":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp("/", new List<INode>() { currentNode, ParseFactor() });
                    continue;
                case "AND":
                    _tk.SelectNext();
                    currentNode = rootNode;
                    rootNode = new BinOp("&&", new List<INode>() { currentNode, ParseFactor() });
                    continue;
                default:
                    return rootNode;
            }
        }
    }

    private static INode ParseFactor()
    {
        if (_tk == null) throw new Exception();
        if (!_tk.IsNextFactorSymbol()) throw new SyntaxException("ParseFactor: Wrong token order");
        INode retVal;

        switch (_tk.Next.Type)
        {
            case "INT":
                retVal = new IntVal(_tk.Next.Value);
                _tk.SelectNext();
                return retVal;
            
            case "STRING":
                retVal = new StrVal(_tk.Next.Value);
                _tk.SelectNext();
                return retVal;

            case "IDENTIFIER":
                retVal = new Identifier(_tk.Next.Value);
                _tk.SelectNext();
                return retVal;

            case "MINUS":
                _tk.SelectNext();
                return new UnOp('-', new List<INode>() { ParseFactor() });

            case "PLUS":
                _tk.SelectNext();
                return new UnOp('+', new List<INode>() { ParseFactor() });

            case "NOT":
                _tk.SelectNext();
                return new UnOp('!', new List<INode>() { ParseFactor() });

            case "LPAREN":
                _tk.SelectNext();
                INode result = ParseRelativeExpression(true);
                if (_tk.Next.Type != "RPAREN") throw new SyntaxException("Wrong token order");
                _tk.SelectNext();
                return result;

            case "READ":
                _tk.SelectNext();
                if (_tk.Next.Type != "LPAREN") throw new SyntaxException("Wrong token order");
                _tk.SelectNext();
                INode readNode = new Read();
                if (_tk.Next.Type != "RPAREN") throw new SyntaxException("Wrong token order");
                _tk.SelectNext();
                return readNode;

            default:
                throw new SyntaxException("Wrong token order");
        }
    }

    public static INode Run(string code)
    {
        RemoveComment(ref code);
        _tk = new Tokenizer(code);
        _tk.SelectNext();
        return ParseBlock();
    }
}

class Test
{
    static void Main(string[] args)
    {
        string filename = args[0];

        try
        {
            string fileContents = File.ReadAllText(filename);
            INode astRoot = Parser.Run(fileContents);
            astRoot.Evaluate();
        }
        catch (IOException e)
        {
            Console.WriteLine("Error reading file: " + e.Message);
            throw;
        }
    }
}