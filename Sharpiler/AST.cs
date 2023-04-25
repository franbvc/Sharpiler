namespace Sharpiler;

public interface INode
{
    dynamic Value { get; set; }
    List<INode> Children { get; set; }
    dynamic Evaluate();
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

    public dynamic Evaluate()
    {
        if (Value == '-') return -Children[0].Evaluate();
        if (Value == '!') return (Children[0].Evaluate() == 1) ? 0 : 1;

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

    public dynamic Evaluate()
    {
        return Value;
    }
}

class StrVal : INode
{
    public dynamic Value { get; set; }

    public List<INode> Children { get; set; }

    public StrVal(string value, List<INode> children = null!)
    {
        Value = value;
        Children = children ?? new List<INode>();
    }

    public dynamic Evaluate()
    {
        return Value;
    }
}

class BinOp : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public BinOp(string value, List<INode> children)
    {
        if (children.Count != 2) throw new SemanticException("BinOp: Wrong children amount");
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        dynamic leftNode = Children[0].Evaluate();
        dynamic rightNode = Children[1].Evaluate();

        if (leftNode is int && rightNode is string)
            throw new SemanticException("Invalid Binary Operation (int + string)");

        if (leftNode is string)
            return Value switch
            {
                "." => leftNode + rightNode,
                _ => throw new SemanticException($"Invalid Binary Operation (string {Value} ...) ")
            };

        if (leftNode is int && rightNode is int)
            return Value switch
            {
                "+" => Children[0].Evaluate() + Children[1].Evaluate(),
                "-" => Children[0].Evaluate() - Children[1].Evaluate(),
                "*" => Children[0].Evaluate() * Children[1].Evaluate(),
                "/" => Children[0].Evaluate() / Children[1].Evaluate(),
                "&&" => (Children[0].Evaluate() == 1) && (Children[1].Evaluate() == 1) ? 1 : 0,
                "||" => (Children[0].Evaluate() == 1) || (Children[1].Evaluate() == 1) ? 1 : 0,
                "==" => (Children[0].Evaluate() == Children[1].Evaluate()) ? 1 : 0,
                ">" => (Children[0].Evaluate() > Children[1].Evaluate()) ? 1 : 0,
                "<" => (Children[0].Evaluate() < Children[1].Evaluate()) ? 1 : 0,
                _ => throw new SemanticException("Invalid Binary Operation")
            };

        throw new SemanticException("Invalid Binary Operation");
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

    public dynamic Evaluate()
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

    public dynamic Evaluate()
    {
        // retorna o valor do ID no dict
        (string, string) retTuple = SymbolTable.Get(Value);
        string retType = retTuple.Item1;
        string retVal = retTuple.Item2;

        return retType switch
        {
            "Int" => int.Parse(retVal),
            "String" => retVal,
            _ => throw new SemanticException("Identifier (ST): Invalid type")
        };
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

    public dynamic Evaluate()
    {
        // Atribui o valor da direita ao dict do valor da esquerda
        string key = Children[0].Value;
        var val = Children[1].Evaluate();
        string type = SymbolTable.GetType(key);

        if (val is string && type != "String" ||
            val is int && type != "Int")
            throw new SemanticException($"Assignment: Invalid type '{type}' for '{key}'");

        SymbolTable.Set(key, val.ToString(), type);
        return 0;
    }
}

class VariableDeclaration : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public VariableDeclaration(List<INode> children, string value)
    {
        if (value != "Int" && value != "String")
            throw new SemanticException($"VariableDeclaration: Invalid type '{value}'");
        if (children.Count < 1) throw new SemanticException("VariableDeclaration: Wrong children amount");
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        // Adiciona o valor da esquerda ao dict
        if (Children.Count == 1)
        {
            SymbolTable.Set(Children[0].Value, "_", Value);
            return 0;
        }

        var val = Children[1].Evaluate();

        switch (val)
        {
            case int:
                SymbolTable.Set(Children[0].Value, val.ToString(), "Int");
                return 0;
            case string:
                SymbolTable.Set(Children[0].Value, val, "String");
                return 0;
            default:
                throw new SemanticException($"VarDec: type '{val.GetType()} can't exist in ST'");
        }
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

    public dynamic Evaluate()
    {
        Console.WriteLine(Children[0].Evaluate());
        return 0;
    }
}

class Read : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Read(List<INode> children = null!, char value = ' ')
    {
        Value = value;
        Children = children ?? new List<INode>();
    }

    public dynamic Evaluate()
    {
        string input = Console.ReadLine() ?? throw new InvalidOperationException();
        return int.Parse(input);
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

    public dynamic Evaluate()
    {
        foreach (INode child in Children)
        {
            child.Evaluate();
        }

        return 0;
    }
}

class If : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public If(List<INode> children, char value = ' ')
    {
        if (children.Count != 3) throw new SemanticException("If: Wrong children amount");
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        if (Children[0].Evaluate() == 1) Children[1].Evaluate();
        else Children[2].Evaluate();

        return 0;
    }
}

class While : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public While(List<INode> children, char value = ' ')
    {
        if (children.Count != 2) throw new SemanticException("While: Wrong children amount");
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        while (Children[0].Evaluate() == 1)
        {
            Children[1].Evaluate();
        }

        return 0;
    }
}