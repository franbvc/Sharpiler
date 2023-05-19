namespace Sharpiler;

public interface ISymbolTable
{
    void Set(string key, string value, string type);
    (string, string) Get(string key);
    bool Contains(string key);
    string GetType(string key);
}

public class SymbolTable : ISymbolTable
{
    private static readonly IDictionary<string, (string, string)> SymbolDictionary =
        new Dictionary<string, (string, string)>();
    
    public void Set(string key, string value, string type) => SymbolDictionary[key] = (type, value);
    public  (string, string) Get(string key) => SymbolDictionary[key];
    public bool Contains(string key) => SymbolDictionary.ContainsKey(key);
    
    public string GetType(string key)
    {
        if (!SymbolDictionary.ContainsKey(key))
            throw new SemanticException($"SymbolTable: '{key}' is not defined");
        return SymbolDictionary[key].Item1;
    }
}

public class LocalSymbolTable : ISymbolTable
{
    private readonly IDictionary<string, (string, string)> _symbolDictionary =
        new Dictionary<string, (string, string)>();
    
    public void Set(string key, string value, string type) => _symbolDictionary[key] = (type, value);
    public (string, string) Get(string key) => _symbolDictionary[key];
    public bool Contains(string key) => _symbolDictionary.ContainsKey(key);
    
    public string GetType(string key)
    {
        if (!_symbolDictionary.ContainsKey(key))
            throw new SemanticException($"LocalSymbolTable: '{key}' is not defined");
        return _symbolDictionary[key].Item1;
    } 
    
}

public static class FuncTable
{
    private static readonly IDictionary<string, INode> FuncDictionary =
        new Dictionary<string, INode>();

    public static void Set(string key, INode funcDecl)
    {
        FuncDictionary[key] = funcDecl;
    }

    public static INode Get(string key)
    {
        return FuncDictionary[key];
    }
    
    public static bool Contains(string key)
    {
        return FuncDictionary.ContainsKey(key);
    }
}