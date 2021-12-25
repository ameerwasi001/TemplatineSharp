using System;
using System.Linq;
using System.Collections.Generic;

class LexerException : Exception
{
    public LexerException(Position start, Position end, string err) : base(
        String.Concat(new[]{String.Format("LexerException: {0}\n", err), "File ", start.fn, " in line number ", (start.ln + 1).ToString()})
    ){}
  
}

class InvalidSyntaxError : Exception
{
    public InvalidSyntaxError(Position start, Position end, string err) : base(
        String.Concat(new[]{String.Format("InvalidSyntaxError: {0}\n", err), "File ", start.fn, " in line number ", (start.ln + 1).ToString()})
    ){}
}

class ValidationError : Exception
{
    public ValidationError(Position start, Position end, string err) : base(
        String.Concat(new[]{String.Format("ValidationError: {0}\n", err), "File ", start.fn, " in line number ", (start.ln + 1).ToString()})
    ){}
}

class ModelError : ValidationError
{
    public ModelError(HashSet<string> requiredEnv, HashSet<string> modelKeys) : base(
        Position.Nothing(), Position.Nothing(), 
        string.Format("Required keys [{0}] are missing", string.Join(", ", requiredEnv.Except(modelKeys)))
    ){}
}

class CyclicExtensionError : ValidationError
{
    public CyclicExtensionError(string point, IEnumerable<string> cycle) : base(
        Position.Nothing(), Position.Nothing(), 
        string.Format("{0} is a cycle and cyclical inheritance in not allowed", string.Format("{0} -> {1}", string.Join(" -> ", extractCycle(point, cycle)), point))
    ){}

    static private List<string> extractCycle(string point, IEnumerable<string> visiting)
    {
        var cycle = new List<string>();
        var found = false;
        foreach(var vertex in visiting)
        {
            if(found) cycle.Add(vertex);
            else if(point == vertex) {
                found = true;
                cycle.Add(vertex);
            }
        }
        return cycle;
    }
}

class RuntimeError : Exception
{
    public RuntimeError(Position start, Position end, string err) : base(
        String.Concat(new[]{String.Format("RuntimeError: {0}\n", err), "File ", start.fn, " in line number ", (start.ln + 1).ToString()})
    ){}
}