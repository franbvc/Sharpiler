namespace Sharpiler;

public static class SymbolTable
{
    private static readonly IDictionary<string, (string, string)> SymbolDictionary =
        new Dictionary<string, (string, string)>();

    public static void Set(string key, string value, string type)
    {
        SymbolDictionary[key] = (type, value);
    }

    public static (string, string) Get(string key)
    {
        return SymbolDictionary[key];
    }
    
    public static string GetType(string key)
    {
        if (!SymbolDictionary.ContainsKey(key))
            throw new SemanticException($"SymbolTable: '{key}' is not defined");
        return SymbolDictionary[key].Item1;
    }
    
    public static bool Contains(string key)
    {
        return SymbolDictionary.ContainsKey(key);
    }
}