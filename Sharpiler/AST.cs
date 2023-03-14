namespace Sharpiler;

public interface INode
{
    dynamic Value { get; set; }
    List<INode> Children { get; set; }
    int Evaluate();
}

class UnOp : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public UnOp(char value, List<INode> children)
    {
        if (children.Count != 1) throw new SemanticException("UnOp: Wrong children amount");
        Value = value;
        Children = children;
    }

    public int Evaluate()
    {
        if (Value == '-') return -Children[0].Evaluate();

        return Children[0].Evaluate();
    }
}

class IntVal : INode
{
    public dynamic Value { get; set; }

    public List<INode> Children { get; set; }

    public IntVal(int value, List<INode> children = null!)
    {
        Value = value;
        Children = children ?? new List<INode>();
    }

    public int Evaluate()
    {
        return Value;
    }
}

class BinOp : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public BinOp(char value, List<INode> children)
    {
        if (children.Count != 2) throw new SemanticException("BinOp: Wrong children amount");
        Value = value;
        Children = children;
    }

    public int Evaluate()
    {
        switch (Value)
        {
            case '+':
                return Children[0].Evaluate() + Children[1].Evaluate();
            case '-':
                return Children[0].Evaluate() - Children[1].Evaluate();
            case '*':
                return Children[0].Evaluate() * Children[1].Evaluate();
            case '/':
                return Children[0].Evaluate() / Children[1].Evaluate();
            default:
                throw new SemanticException("Invalid Binary Operation");
        }
    }
}

class NoOp : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public NoOp(char value = ' ', List<INode> children = null!)
    {
        Value = value;
        Children = children ?? new List<INode>();
    }

    public int Evaluate()
    {
        return 0;
    }
}