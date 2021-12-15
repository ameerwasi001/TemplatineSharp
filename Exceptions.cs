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

class RuntimeError : Exception
{
    public RuntimeError(Position start, Position end, string err) : base(
        String.Concat(new[]{String.Format("RuntimeError: {0}\n", err), "File ", start.fn, " in line number ", (start.ln + 1).ToString()})
    ){}
}