using System;
using System.Collections.Generic;

class Lexer 
{
    static HashSet<char> digits = new HashSet<char>{'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.'};
    static HashSet<char> alphabets = new HashSet<char>{
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k',
        'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
        'w', 'x', 'y', 'z'
    };
    static HashSet<char> whitespace = new HashSet<char>{
        '\t', ' ', '\n', '\r',
    };
    public string text = "";
    public string fn = "";
    public string currentChar;
    public Position pos;

    public Lexer(string fname, string txt, Position givenPos = null){
        fn = fname;
        text = txt;
        if(givenPos == null){
            pos = new Position(fname, txt);
        } else {
            givenPos.ResetIndexing();
            pos = givenPos;
        }
    }

    public void Advance()
    {
        pos.Advance(currentChar);
        currentChar = pos.idx < text.Length ? text[pos.idx].ToString() : Token.TT_END;
    }

    public Token MakeNumber()
    {
        var pos_start = pos.Copy();
        var number_str = "";
        var dot_count = 0;
        var e_count = 0;
        while (digits.Contains(currentChar[0]) || currentChar == ".")
        {
            if (currentChar == ".") dot_count+=1;
            if (currentChar == "e") e_count+=1;
            if (dot_count == 2) break;
            if (e_count == 2) break;
            if (dot_count == 1 && e_count == 1) break;
            number_str = String.Concat(number_str, currentChar);
            this.Advance();
        }
        return new Token(Token.TT_NUMBER, number_str, pos_start.Copy(), pos.Copy());
    }

    public Token MakeString()
    {
        var str = "";
        var posStart = pos.Copy();
        var esacapeChar = false;
        var escapeChars = new Dictionary<string, string>(){
            {"n", "\n"},
            {"t", "\t"}
        };
        this.Advance();
        while (currentChar != Token.TT_END && (currentChar != "\"" || esacapeChar))
        {
            if(esacapeChar)
            {
                str += escapeChars[currentChar].ToString();
                esacapeChar = false;
            } else 
            {
                if(currentChar == "\\")
                {
                    esacapeChar = true;
                } else 
                {
                    str += currentChar.ToString();
                }
            }
            this.Advance();
        }
        this.Advance();
        return new Token(Token.TT_STRING, str, posStart.Copy(), pos.Copy());
    }

    public Token MakeIdentifier()
    {
        var pos_start = pos.Copy();
        var id_str = "";
        while (alphabets.Contains(currentChar[0]))
        {
            id_str += currentChar;
            this.Advance();
        }

        return
            Token.KEYWORDS.Contains(id_str) 
                ? new Token(Token.TT_KEYWORD, id_str, pos_start.Copy(), pos.Copy()) 
                : new Token(Token.TT_IDENT, id_str, pos_start.Copy(), pos.Copy());
    }
    public List<Token> Lex()
    {
        this.Advance();
        var tokens = new List<Token>();
        while (currentChar != Token.TT_END)
        {
            if (whitespace.Contains(currentChar[0]))
            {
                this.Advance();
            } else if (currentChar == "+")
            {
                tokens.Add(new Token(Token.TT_PLUS, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (currentChar == "\"")
            {
                tokens.Add(this.MakeString());
            } else if (currentChar == "-")
            {
                tokens.Add(new Token(Token.TT_MINUS, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (currentChar == "*")
            {
                tokens.Add(new Token(Token.TT_MUL, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (currentChar == "/")
            {
                tokens.Add(new Token(Token.TT_DIV, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (currentChar == "(")
            {
                tokens.Add(new Token(Token.TT_RPAREN, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (currentChar == ")")
            {
                tokens.Add(new Token(Token.TT_LPAREN, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (currentChar == "|")
            {
                tokens.Add(new Token(Token.TT_PIPE, pos.Copy(), pos.Copy()));
                this.Advance();
            } else if (digits.Contains(currentChar[0]))
            {
                tokens.Add(this.MakeNumber());
            } else if (alphabets.Contains(currentChar[0]))
            {
                tokens.Add(this.MakeIdentifier());
            } else 
            {
                var posStart = pos.Copy();
                var errorChar = currentChar;
                this.Advance();
                throw new LexerException(posStart, pos.Copy(), errorChar);
            }
        }
        tokens.Add(new Token(Token.TT_END, pos, pos));
        return tokens;
    }
}