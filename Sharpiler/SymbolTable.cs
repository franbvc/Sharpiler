﻿namespace Sharpiler;

// Stack position table
public static class SymbolTable
{
    public static int StackPos { get; private set; } = 4;
    private static readonly IDictionary<string, int> SymbolDictionary =
        new Dictionary<string, int>();

    public static void Add(string key)
    {
        WriteAsm.Write("PUSH DWORD 0");
        SymbolDictionary[key] = StackPos;
        StackPos += 4;
    }

    public static int Get(string key)
    {
        return SymbolDictionary[key];
    }

    public static bool Contains(string key)
    {
        return SymbolDictionary.ContainsKey(key);
    }
}

public static class ValueSymbolTable
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
