using System.Collections.Generic;

public class Token 
{
    public static string TT_NUMBER = "NUMBER";
    public static string TT_IDENT = "IDENT";
    public static string TT_KEYWORD = "KEYWORD";
    public static string TT_GT = "GT";
    public static string TT_GTE = "GTE";
    public static string TT_LT = "LT";
    public static string TT_LTE = "LTE";
    public static string TT_STRING = "STRING";
    public static string TT_PLUS = "PLUS";
    public static string TT_MINUS = "MINUS";
    public static string TT_MUL = "MUL";
    public static string TT_DIV = "DIV";
    public static string TT_AND = "AND";
    public static string TT_OR = "OR";
    public static string TT_RPAREN = "RPAREN";
    public static string TT_LPAREN = "LPAREN";
    public static string TT_RSQUARE = "RSQUARE";
    public static string TT_LSQUARE = "LSQUARE";
    public static string TT_COMMA = "COMMA";
    public static string TT_PIPE = "PIPE";
    public static string TT_END = "END";

    public string tokType;
    public string value;
    public Position posStart; 
    public Position posEnd; 
    public static HashSet <string> KEYWORDS = new HashSet <string> {  
        "for",
        "in",
        "endfor",
        "if",
        "elif",
        "endif",
        "true",
        "false"
    };


    public Token(string typ, string val, Position start, Position end)
    {
        tokType = typ;
        value = val;
        posStart = start;
        posEnd = end;
    }

    public Token(string typ, Position start, Position end)
    {
        tokType = typ;
        value = "";
        posStart = start;
        posEnd = end;
    }

    public bool Matches(string name, string value)
    {
        return this.tokType == name && this.value == value;
    }

    public override string ToString()
    {
        if (this.value == "") return this.tokType;
        return string.Concat(new[]{"[", this.tokType, ":", this.value, "]"});
    }
}