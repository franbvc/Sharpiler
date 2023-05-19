namespace Sharpiler;

public interface INode
{
    dynamic Value { get; set; }
    List<INode> Children { get; set; }
    dynamic Evaluate(ISymbolTable? localSt = null);
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

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        if (Value == '-') return -Children[0].Evaluate(localSt);
        if (Value == '!') return (Children[0].Evaluate(localSt) == 1) ? 0 : 1;

        return Children[0].Evaluate(localSt);
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

    public dynamic Evaluate(ISymbolTable? localSt)
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

    public dynamic Evaluate(ISymbolTable? localSt)
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

    public dynamic Evaluate(ISymbolTable? localStArg)
    {
        ISymbolTable localSt = localStArg ?? new SymbolTable();
        
        dynamic leftNode = Children[0].Evaluate(localSt);
        dynamic rightNode = Children[1].Evaluate(localSt);

        if (Value == ".") return leftNode.ToString() + rightNode.ToString();

        if (leftNode is string || rightNode is string)
        {
            if (Value == ">")
                return string.Compare(leftNode.ToString(), rightNode.ToString()) >= 0 ? 1 : 0;
            if (Value == "<")
                return string.Compare(leftNode.ToString(), rightNode.ToString()) >= 0 ? 0 : 1;
            if (Value == "==")
                return leftNode.ToString() == rightNode.ToString() ? 1 : 0;
        }

        if (leftNode is int && rightNode is int)
            return Value switch
            {
                "+" => Children[0].Evaluate(localSt) + Children[1].Evaluate(localSt),
                "-" => Children[0].Evaluate(localSt) - Children[1].Evaluate(localSt),
                "*" => Children[0].Evaluate(localSt) * Children[1].Evaluate(localSt),
                "/" => Children[0].Evaluate(localSt) / Children[1].Evaluate(localSt),
                "&&" => (Children[0].Evaluate(localSt) == 1) && (Children[1].Evaluate(localSt) == 1) ? 1 : 0,
                "||" => (Children[0].Evaluate(localSt) == 1) || (Children[1].Evaluate(localSt) == 1) ? 1 : 0,
                "==" => (Children[0].Evaluate(localSt) == Children[1].Evaluate(localSt)) ? 1 : 0,
                ">" => (Children[0].Evaluate(localSt) > Children[1].Evaluate(localSt)) ? 1 : 0,
                "<" => (Children[0].Evaluate(localSt) < Children[1].Evaluate(localSt)) ? 1 : 0,
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

    public dynamic Evaluate(ISymbolTable? localSt)
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

    public dynamic Evaluate(ISymbolTable? localStArg)
    {
        ISymbolTable localSt = localStArg ?? new SymbolTable();
        var (retType, retVal) = ((string,string))localSt.Get(Value);

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

    public dynamic Evaluate(ISymbolTable? localStArg)
    {
        ISymbolTable localSt = localStArg ?? new SymbolTable();
        
        string key = Children[0].Value;
        var val = Children[1].Evaluate(localSt);

        string type = localSt.GetType(key);

        if (val is string && type != "String" ||
            val is int && type != "Int")
            throw new SemanticException($"Assignment: Invalid type '{type}' for '{key}'");

        localSt.Set(key, val.ToString(), type);

        return 0;
    }
}

class VariableDeclaration : INode
{
    /*
     * Children[0] = Identifier
     * Children[1] = Type
     * Value = Type
     */
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

    public dynamic Evaluate(ISymbolTable? localStArg)
    {
        // Adiciona o valor da esquerda ao dict
        ISymbolTable localSt = localStArg ?? new SymbolTable();
        
        if (localSt.Contains(Children[0].Value))
            throw new SemanticException($"VarDec: '{Children[0].Value}' type already declared in ST");

        if (Children.Count == 1)
        {
            localSt.Set(Children[0].Value, "_", Value);
            return 0;
        }

        var val = Children[1].Evaluate();

        switch (val)
        {
            case int:
                localSt.Set(Children[0].Value, val.ToString(), "Int");
                return 0;
            case string:
                localSt.Set(Children[0].Value, val, "String");
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

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        Console.WriteLine(Children[0].Evaluate(localSt));
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

    public dynamic Evaluate(ISymbolTable? localSt)
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
        Value = value;
        Children = children;
    }

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        if (Children.Count == 0) return 0;
        dynamic? retVal = null;

        foreach (INode child in Children)
        {
            try
            {
                retVal = child.Evaluate(localSt);
            }
            catch (ReturnException returnException)
            {
                retVal = returnException.Value;
                throw;
            }

            if (child is Return)
                throw new ReturnException(child.Evaluate(localSt));
        }

        return retVal == null ? 0 : retVal;
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

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        if (Children[0].Evaluate(localSt) == 1) Children[1].Evaluate(localSt);
        else Children[2].Evaluate(localSt);

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

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        while (Children[0].Evaluate(localSt) == 1)
        {
            Children[1].Evaluate(localSt);
        }

        return 0;
    }
}

class FunctionDeclaration : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public FunctionDeclaration(List<INode> children, string value)
    {
        if (children.Count < 2) throw new SemanticException("FuncDec: Wrong children amount");
        if (value != "String" && value != "Int")
            throw new SemanticException($"FuncDec: Invalid type '{value}'");

        Value = value;
        Children = children;
    }

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        if (FuncTable.Contains(Children[0].Value))
            throw new SemanticException($"FuncDec: '{Children[0].Value}' function already declared in FT");

        FuncTable.Set(Children[0].Value, this);
        return 0;
    }

    public List<INode> GetParams()
    {
        return Children.Skip(1).Take(Children.Count - 2).ToList();
    }
}

class FunctionCall : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public FunctionCall(List<INode> children, string value)
    {
        Value = value;
        Children = children;
    }

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        if (!FuncTable.Contains(Value))
            throw new SemanticException($"FuncCall: '{Value}' function not declared in FT");

        FunctionDeclaration funcDec = FuncTable.Get(Value);

        if (Children.Count != funcDec.Children.Count - 2)
            throw new SemanticException($"FuncCall: '{Value}' function wrong parameters amount");

        LocalSymbolTable funcSt = new LocalSymbolTable();
        List<INode> funcParams = funcDec.GetParams();
        for (int i = 0; i < funcParams.Count; i++)
        {
            var callParam = Children[i].Evaluate();

            switch (funcParams[i].Value)
            {
                case "Int" when callParam is int:
                    funcSt.Set(funcParams[i].Children[0].Value, callParam.ToString(), "Int");
                    continue;

                case "String" when callParam is string:
                    funcSt.Set(funcParams[i].Children[0].Value, callParam, "String");
                    continue;

                default:
                    throw new SemanticException($"FuncCall: '{Value}' function wrong parameter type");
            }
        }

        try
        {
            funcDec.Children[^1].Evaluate(funcSt);
        }
        catch (ReturnException returnException)
        {
            return returnException.Value;
        }

        return 0;
    }
}

class Return : INode
{
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Return(List<INode> children, char value = ' ')
    {
        if (children.Count != 1) throw new SemanticException("Return: Wrong children amount");
        Value = value;
        Children = children;
    }

    public dynamic Evaluate(ISymbolTable? localSt)
    {
        return Children[0].Evaluate(localSt);
    }
}