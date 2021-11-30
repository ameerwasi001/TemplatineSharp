using System.Collections.Generic;

public class SymbolTable 
{
    public SymbolTable parent;
    private Dictionary<string, Value> table;

    public SymbolTable()
    {
        parent = null;
        table = new Dictionary<string, Value>();
    }

    public SymbolTable(Dictionary<string, Value> dict)
    {
        parent = null;
        table = dict;
    }

    public SymbolTable(SymbolTable givenParent)
    {
        parent = givenParent;
        table = new Dictionary<string, Value>();
    }

    public Value Get(string name)
    {
        if (this.table.ContainsKey(name)) return this.table[name];
        if (this.parent == null) return null;
        return this.parent.Get(name);
    }

    public Value Set(string name, Value value)
    {
        this.table[name] = value;
        return value;
    }
}

public class Context
{
    public string name;
    public SymbolTable symbolTable;
    public Context parent;
    public Position parentEntryPos;

    public Context(SymbolTable table = null, string givenName = "<module>", Context givenParent = null, Position pos = null)
    {
        name = givenName;
        symbolTable = table == null ? new SymbolTable(givenParent == null ? null : givenParent.symbolTable) : table;
        parent = givenParent;
        parentEntryPos = pos;
    }

    public Context(Dictionary<string, Value> dict) : this(new SymbolTable(dict)) {}

    public Value Get(string name)
    {
        return this.symbolTable.Get(name);
    }

    public Value Set(string name, Value value)
    {
        return this.symbolTable.Set(name, value);
    }
}