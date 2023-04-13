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
        INode firstBlock = ParseBlock();
        
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
        INode elseBlock = ParseBlock();
        
        if (_tk.Next.Type != "END") throw new SyntaxException("Wrong token order");
        _tk.SelectNext();

        return new If(new List<INode>() { condition, firstBlock, elseBlock });
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