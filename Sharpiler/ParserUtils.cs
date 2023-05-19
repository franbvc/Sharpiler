namespace Sharpiler;

public partial class Parser

{
    public enum ConditionalType
    {
        If,
        While
    }

    private static INode BuildConditionalNode(ConditionalType type)
    {
        if (_tk == null) throw new Exception();

        INode condition = ParseRelativeExpression();
        CheckToken("NEWLINE");
        INode firstBlock = ParseBlock(true);

        if (_tk.Next.Type == "END")
        {
            _tk.SelectNext();
            return type switch
            {
                ConditionalType.If => new If(new List<INode>() { condition, firstBlock, new NoOp() }),
                ConditionalType.While => new While(new List<INode>() { condition, firstBlock }),
                _ => throw new SyntaxException("Wrong token order")
            };
        }

        CheckToken("ELSE");
        _tk.SelectNext();
        
        CheckToken("NEWLINE");
        INode elseBlock = ParseBlock(true);

        CheckToken("END");
        _tk.SelectNext();

        return new If(new List<INode>() { condition, firstBlock, elseBlock });
    }

    private static void CheckToken(string expected)
    {
        if (_tk == null) throw new Exception();
        if (_tk.Next.Type != expected)
            throw new SyntaxException(
                "Wrong token order: expected " + expected + ", but got " + _tk.Next.Type);
    }

    private static void CheckMultipleTokens(List<string> expected)
    {
        if (_tk == null) throw new Exception();
        if (!expected.Contains(_tk.Next.Type))
            throw new SyntaxException(
                "Wrong token order: expected " + string.Join(" or ", expected) + ", but got " + _tk.Next.Type);
    }

    private static List<INode> ParseCallArgs()
    {
        if (_tk == null) throw new Exception();
        List<INode> args = new List<INode>();
        if (_tk.Next.Type == "RPAREN") return args;

        while (true)
        {
            args.Add(ParseRelativeExpression(true));
            if (_tk.Next.Type == "RPAREN") break;
            CheckToken("COMMA");
            _tk.SelectNext();
        }

        _tk.SelectNext();
        return args;
    }

    private static INode ParseStatementIdentifier()
    {
        if (_tk == null) throw new Exception();

        Identifier leftNode = new Identifier(_tk.Next.Value);
        string leftValue = _tk.Next.Value;

        _tk.SelectNext();

        switch (_tk.Next.Type)
        {
            case "ASSIGN":
                _tk.SelectNext();
                return new Assignment(new List<INode>() { leftNode, ParseRelativeExpression() });
            case "SCOPE":
            {
                _tk.SelectNext();
                CheckToken("TYPE");

                string varType = _tk.Next.Value;
                _tk.SelectNext();

                if (_tk.Next.Type != "ASSIGN") return new VariableDeclaration(new List<INode>() { leftNode }, varType);
                _tk.SelectNext();

                return new VariableDeclaration(
                    new List<INode>() { leftNode, ParseRelativeExpression() },
                    varType);
            }
            case "LPAREN":
            {
                _tk.SelectNext();
                FunctionCall call = new FunctionCall(ParseCallArgs(), leftValue);
                _tk.SelectNext();
                return call;
            }
        }

        throw new SyntaxException(
            "Wrong token order: expected '=', '::' or '(', but got " + _tk.Next.Type);
    }

    private static void ParseParams(List<INode> paramList)
    {
        if (_tk == null) throw new Exception();

        while (_tk.Next.Type is not "RPAREN")
        {
            CheckToken("IDENTIFIER");
            var varName = new Identifier(_tk.Next.Value);
            _tk.SelectNext();

            CheckToken("SCOPE");
            _tk.SelectNext();

            CheckToken("TYPE");
            string varType = _tk.Next.Value;

            paramList.Add(new VariableDeclaration(new List<INode>() { varName }, varType));
            _tk.SelectNext();

            if (_tk.Next.Type is "RPAREN") break;
            if (_tk.Next.Type is not "COMMA")
                throw new SyntaxException("Wrong token order: expected ',' or ')', but got " + _tk.Next.Type);
            _tk.SelectNext();
        }
    }

    private static INode ParseFunction()
    {
        if (_tk == null) throw new Exception();

        CheckToken("IDENTIFIER");
        Identifier functionName = new Identifier(_tk.Next.Value);
        _tk.SelectNext();

        CheckToken("LPAREN");
        _tk.SelectNext();

        List<INode> paramList = new List<INode>() { functionName };
        ParseParams(paramList);
        _tk.SelectNext();

        CheckToken("SCOPE");
        _tk.SelectNext();

        CheckToken("TYPE");
        string returnType = _tk.Next.Value;
        _tk.SelectNext();

        CheckToken("NEWLINE");
        _tk.SelectNext();

        INode functionBody = ParseBlock(true);
        paramList.Add(functionBody);
        
        CheckToken("END");
        _tk.SelectNext();

        return new FunctionDeclaration(paramList, returnType);
    }


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
}