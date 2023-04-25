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
        if (_tk.Next.Type != "NEWLINE") throw new SyntaxException("Wrong token order");
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
        
        if (_tk.Next.Type != "ELSE" ) throw new SyntaxException("Wrong token order");
        
        _tk.SelectNext();
        if (_tk.Next.Type != "NEWLINE") throw new SyntaxException("Wrong token order");
        INode elseBlock = ParseBlock(true);
        
        if (_tk.Next.Type != "END") throw new SyntaxException("Wrong token order");
        _tk.SelectNext();

        return new If(new List<INode>() { condition, firstBlock, elseBlock });
    }

    private static INode ParseStatementIdentifier()
    {
        if (_tk == null) throw new Exception();

        Identifier leftNode = new Identifier(_tk.Next.Value);
        _tk.SelectNext();

        if (_tk.Next.Type == "ASSIGN")
        {
            _tk.SelectNext();
            return new Assignment(new List<INode>() { leftNode, ParseRelativeExpression() });
        }

        if (_tk.Next.Type != "SCOPE") 
            throw new SyntaxException("Identifier wrong token order: found " + _tk.Next.Type);
        
        _tk.SelectNext();
        if (_tk.Next.Type != "TYPE")
            throw new SyntaxException("Wrong token order: expected TYPE got " + _tk.Next.Type);
        string varType = _tk.Next.Value;
            
        _tk.SelectNext();
        if (_tk.Next.Type != "ASSIGN") return new VariableDeclaration(new List<INode>() { leftNode }, varType);
        
        _tk.SelectNext();
        return new VariableDeclaration(
            new List<INode>() { leftNode, ParseRelativeExpression() },
            varType);

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