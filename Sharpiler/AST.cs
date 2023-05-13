using System.Diagnostics;

namespace Sharpiler;

public interface INode
{
    int Id { get; }
    dynamic Value { get; set; }
    List<INode> Children { get; set; }
    dynamic Evaluate();
}

public static class NodeIdManager
{
    private static int _id = 0;
    
    public static int GetId()
    {
        return _id++;
    }
}

class UnOp : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public UnOp(char value, List<INode> children)
    {
        if (children.Count != 1) throw new SemanticException("UnOp: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }


    public dynamic Evaluate()
    {
        Children[0].Evaluate();

        switch (Value)
        {
            case '-':
                WriteAsm.Write("NEG EBX");
                break;
            case '!':
                WriteAsm.Write("AND EBX, 1");
                WriteAsm.Write("XOR EBX, 1");
                break;
            default:
                throw new SemanticException($"UnOp: Invalid operator {Value} (Should never happen)");
        }
        
        return 0;
    }
}

class IntVal : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }

    public List<INode> Children { get; set; }

    public IntVal(int value, List<INode> children = null!)
    {
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children ?? new List<INode>();
    }

    public dynamic Evaluate()
    {
        //return Value;
        WriteAsm.Write($"MOV EBX, {Value}");
        return 0;
    }
}

class StrVal : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }

    public List<INode> Children { get; set; }

    public StrVal(string value, List<INode> children = null!)
    {
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children ?? new List<INode>();
    }

    public dynamic Evaluate()
    {
        //return Value;
        throw new NotImplementedException("StrVal: Not implemented yet");
    }
}

partial class BinOp : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public BinOp(string value, List<INode> children)
    {
        if (children.Count != 2) throw new SemanticException("BinOp: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        if (!_operatorAction.ContainsKey(Value)) 
            throw new SemanticException($"Invalid Binary Operator {Value}");
        
        Children[0].Evaluate();
        WriteAsm.Write("PUSH EBX");
        
        Children[1].Evaluate();
        WriteAsm.Write("POP EAX");
        
        _operatorAction[Value]();

        WriteAsm.Write("MOV EBX, EAX");
        return 0;
    }
}

class NoOp : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public NoOp(char value = ' ', List<INode> children = null!)
    {
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children ?? new List<INode>();
    }

    public dynamic Evaluate()
    {
        WriteAsm.Write("NOP");
        return 0;
    }
}

class Identifier : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }
    
    public Identifier(string value, List<INode> children = null!)
    {
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children ?? new List<INode>();
    }


    public dynamic Evaluate()
    {
        string key = Value;
        if (!SymbolTable.Contains(key))
            throw new SemanticException($"Identifier: key {key} was not declared");
        
        WriteAsm.Write($"MOV EBX, [EBP-{SymbolTable.Get(key)}]");
        return 0;
    }
}

class Assignment : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Assignment(List<INode> children, char value = ' ')
    {
        if (children.Count != 2) throw new SemanticException("Assignment: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }


    public dynamic Evaluate()
    {
        string key = Children[0].Value;
        if (!SymbolTable.Contains(key))
            SymbolTable.Add(key);
            
        Children[1].Evaluate();
        WriteAsm.Write($"MOV [EBP-{SymbolTable.Get(key)}], EBX");

        return 0;
    }
}

class VariableDeclaration : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public VariableDeclaration(List<INode> children, string value)
    {
        if (value != "Int" && value != "String")
            throw new SemanticException($"VariableDeclaration: Invalid type '{value}'");
        if (children.Count < 1) throw new SemanticException("VariableDeclaration: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        string key = Children[0].Value;
        
        if (SymbolTable.Contains(key))
            throw new SemanticException($"VarDec: '{key}' type already declared in ST");

        SymbolTable.Add(key);

        if (Children.Count == 1) return 0;

        Children[1].Evaluate();
        WriteAsm.Write($"MOV [EBP-{SymbolTable.Get(key)}], EBX");

        return 0;
    }

}

class Print : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Print(List<INode> children, char value = ' ')
    {
        if (children.Count != 1) throw new SemanticException("Print: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }


    public dynamic Evaluate()
    {
        Children[0].Evaluate();
        
        WriteAsm.Write("PUSH EBX");
        WriteAsm.Write("CALL print");
        WriteAsm.Write("POP EBX");
            
        return 0;
    }
}

class Read : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Read(List<INode> children = null!, char value = ' ')
    {
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children ?? new List<INode>();
    }

    public dynamic Evaluate()
    {
        throw new NotImplementedException("Read not implemented yet");
    }
}

class Block : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public Block(List<INode> children, char value = ' ')
    {
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        if (Children.Count == 0) return 0;
        
        foreach (INode child in Children)
        {
            child.Evaluate();
        }

        return 0;
    }
}

class If : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public If(List<INode> children, char value = ' ')
    {
        if (children.Count != 3) throw new SemanticException("If: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }

    public dynamic Evaluate()
    {
        WriteAsm.Write($"IF_{Id}_START:");
        Children[0].Evaluate();
        
        // Condition
        WriteAsm.Write("CMP EBX, 1");
        WriteAsm.Write($"JE IF_{Id}_TRUE");
        
        // Else
        Children[2].Evaluate();
        WriteAsm.Write($"JMP IF_{Id}_END");
        
        // If True
        WriteAsm.Write($"IF_{Id}_TRUE:");
        Children[1].Evaluate();
        
        WriteAsm.Write($"IF_{Id}_END:");
        
        return 0;
    }
}

class While : INode
{
    public int Id { get; private set; }
    public dynamic Value { get; set; }
    public List<INode> Children { get; set; }

    public While(List<INode> children, char value = ' ')
    {
        if (children.Count != 2) throw new SemanticException("While: Wrong children amount");
        Id = NodeIdManager.GetId();
        Value = value;
        Children = children;
    }

    
    public dynamic Evaluate()
    {
        WriteAsm.Write($"LOOP_{Id}_START:");
        
        Children[0].Evaluate();
        
        // Condition
        WriteAsm.Write("CMP EBX, 1");
        WriteAsm.Write($"JE LOOP_{Id}_TRUE");
        
        // End
        WriteAsm.Write($"JMP LOOP_{Id}_END");
        
        // Body
        WriteAsm.Write($"LOOP_{Id}_TRUE:");
        Children[1].Evaluate();
        
        WriteAsm.Write($"JMP LOOP_{Id}_START");
        
        WriteAsm.Write($"LOOP_{Id}_END:");
        
        return 0;
    }
}