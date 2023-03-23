using System.Runtime.InteropServices;
using Console = System.Console;

namespace Sharpiler;

public class Parser
{
    private static Tokenizer? _tk;


    private static void RemoveComment(ref string input)
    {
        
        int startIndex = 0;
        while (true)
        {
            int hashIndex = input.IndexOf('#', startIndex);
            if (hashIndex == -1)
            {
                break;
            }
        
            int endIndex = input.IndexOfAny(new char[] { '\r', '\n' }, hashIndex);
            if (endIndex == -1)
            {
                endIndex = input.Length;
            }
        
            input = input.Remove(hashIndex, endIndex - hashIndex);
        
            startIndex = hashIndex;
        }
        
    }

    private static INode ParseBlock()
    {
        if (_tk == null) throw new Exception();
        
        INode currentNode;
        List<INode> children = new List<INode>();
        
        while (true)
        {
            if (_tk.Next.Type == "EOF") break;
            
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
                Identifier leftNode = new Identifier(_tk.Next.Value);
                _tk.SelectNext();
                if (_tk.Next.Type != "ASSIGN") throw new SyntaxException("Wrong token order");
                _tk.SelectNext();
                currentNode =  new Assignment(new List<INode>() { leftNode, ParseExpression() });
                break;
                
            case "PRINT":
                _tk.SelectNext();
                if (_tk.Next.Type != "LPAREN") throw new SyntaxException("Wrong token order");
                currentNode = new Print(new List<INode>(){ParseExpression(true)});
                break;
            
            case "NEWLINE":
                _tk.SelectNext();
                return new NoOp();
            
            default:
                throw new SyntaxException("Wrong token order");
        }
        
        return currentNode;
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
                
                case "NEWLINE":
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
        INode retVal;

        switch (_tk.Next.Type)
        {
            case "INT":
                retVal = new IntVal(_tk.Next.Value);
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