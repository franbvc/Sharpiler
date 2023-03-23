using System.Diagnostics.CodeAnalysis;

namespace Sharpiler;

public static class SymbolTable
{
    private static readonly IDictionary<string, int> SymbolDictionary = new Dictionary<string, int>();

    public static void Set(string key, int value)
    {
        SymbolDictionary[key] = value;
    }
    
    public static int Get(string key)
    {
        return SymbolDictionary[key];
    }
}

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

class Identifier : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Identifier(string value, List<INode> children = null!)
    {
        Value = value;
        Children = children ?? new List<INode>();
    }

    public int Evaluate()
    {
        // retorna o valor do ID no dict
        return SymbolTable.Get(Value);
    }
}

class Assignment : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Assignment(List<INode> children, char value = ' ')
    {
        if (children.Count != 2) throw new SemanticException("Assignment: Wrong children amount");
        Value = value;
        Children = children;
    }

    public int Evaluate()
    {
        // Atribui o valor da direita ao dict do valor da esquerda
        SymbolTable.Set(Children[0].Value, Children[1].Evaluate());
        return 0;
    }
}

class Print : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Print(List<INode> children, char value = ' ')
    {
        if (children.Count != 1) throw new SemanticException("Print: Wrong children amount");
        Value = value;
        Children = children;
    }

    public int Evaluate()
    {
        // TODO: deve passar dict como parametro se filho for id?
        Console.WriteLine(Children[0].Evaluate());
        return 0;
    }
}

class Block : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Block(List<INode> children, char value = ' ')
    {
        if (children.Count == 0) throw new SemanticException("Block: No Statements");
        Value = value;
        Children = children;
    }

    public int Evaluate()
    {
        // TODO: confirmar isso!
        foreach (INode child in Children)
        {
            child.Evaluate();
        }
        
        return 0;
    }
}