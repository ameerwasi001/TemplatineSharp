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

    public SymbolTable(SymbolTable givenParent)
    {
        parent = givenParent;
        table = new Dictionary<string, Value>();
    }
}

public class Context
{
    public string name;
    public SymbolTable symbolTable;
    public Context parent;
    public Position parentEntryPos;

    public Context(SymbolTable table, Position pos, string givenName = "", Context givenParent = null)
    {
        name = givenName;
        symbolTable = table;
        parent = givenParent;
        parentEntryPos = pos;
    }
}