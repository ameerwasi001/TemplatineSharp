using System;
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

class RuntimeError : Exception
{
    public RuntimeError(Position start, Position end, string err) : base(
        String.Concat(new[]{String.Format("RuntimeError: {0}\n", err), "File ", start.fn, " in line number ", (start.ln + 1).ToString()})
    ){}
}