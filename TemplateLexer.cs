using System.Collections.Generic;

class TemplateLexer {
    private List<string> toks = new List<string>(){""};
    private int index = 0;
    private string contents;
    private string currentChar = "";

    public TemplateLexer(string str)
    {
        contents = str;
        currentChar = index < contents.Length ? contents[index].ToString() : "";
    }

    private void Adavnce()
    {
        index += 1;
        currentChar = index < contents.Length ? contents[index].ToString() : "";
    }

    private void Revert(int num)
    {
        index = num;
        currentChar = index < contents.Length ? contents[index].ToString() : "";
    }

    private bool Matches(string matching)
    {
        var frozenIndex = index;
        var matchIndex = 0;
        var matchLength = frozenIndex + matching.Length;
        while(index < matchLength)
        {
            if(matching[matchIndex].ToString() == currentChar.ToString()) this.Adavnce();
            else
            {
                this.Revert(frozenIndex);
                return false;
            }
            matchIndex += 1;
        }
        this.Revert(frozenIndex);
        return true;
    }

    private void BeginningToken(string str)
    {
        toks.Add("");
        foreach(var _ in str) this.Adavnce();
        toks[toks.Count - 1] += str;
    }

    private void EndingToken(string str)
    {
        toks[toks.Count - 1] += str;
        foreach(var _ in str) this.Adavnce();
        toks.Add("");
    }

    public void SkipString()
    {
        var str = "";
        var esacapeChar = false;
        var escapeChars = new Dictionary<string, string>(){
            {"n", "\n"},
            {"t", "\t"}
        };
        this.Adavnce();
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
            this.Adavnce();
        }
        this.Adavnce();
        toks[toks.Count - 1] += "\"" + str + "\"";
    }

    private List<TemplateToken> ToTemplateTokens(List<string> toks)
    {
        var templateToks = new List<TemplateToken>();

        var pos = new Position("<module>", contents);
        pos.Advance();
        foreach(var tok in toks) {
            var posStart = pos.Copy();
            if (tok.StartsWith("{{") && tok.EndsWith("}}"))
            {
                pos.Advance();
                pos.Advance();
                var subStr = tok.Substring(2);
                var finalStr = subStr.Substring(0, subStr.Length-2);
                var toksForExprParser = new Lexer("<module>", finalStr, pos).Lex();
                var node = new Parser(toksForExprParser).Parse();
                pos.Advance();
                pos.Advance();
                var currentPos = pos.Copy();
                var renderTok = new RenderToken(node, posStart, currentPos);
                templateToks.Add(renderTok);
            } else if (tok.StartsWith("{%") && tok.EndsWith("%}"))
            {
                pos.Advance();
                pos.Advance();
                var subStr = tok.Substring(2);
                var finalStr = subStr.Substring(0, subStr.Length-2);
                var toksForExprParser = new Lexer("<module>", finalStr, pos).Lex();
                var beginSmntCue = new Parser(toksForExprParser).ParseCue();
                pos.Advance();
                pos.Advance();
                templateToks.Add(beginSmntCue);
            } else {
                foreach(var ch in tok) pos.Advance(ch.ToString());
                var currentPos = pos.Copy();
                templateToks.Add(new RenderToken(new StrNode(tok, posStart, currentPos), posStart, currentPos));
            }
        }

        return templateToks;
    }

    public List<TemplateToken> Lex()
    {
        while (currentChar != "")
        {
            if (this.Matches("{{")) this.BeginningToken("{{");
            else if (this.Matches("}}")) this.EndingToken("}}");
            else if (this.Matches("{%")) this.BeginningToken("{%");
            else if (this.Matches("%}")) this.EndingToken("%}");
            else if (currentChar == "\"") this.SkipString();
            else 
            {
                toks[toks.Count - 1] += currentChar;
                this.Adavnce();
            }
        }

        return this.ToTemplateTokens(toks);
    }
}