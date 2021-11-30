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
            if(matching[matchIndex] == currentChar[0]) this.Adavnce();
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

    public List<string> Lex()
    {
        while (currentChar != "")
        {
            if (this.Matches("{{")) this.BeginningToken("{{");
            else if (this.Matches("}}")) this.EndingToken("}}");
            else 
            {
                toks[toks.Count - 1] += currentChar;
                this.Adavnce();
            }
        }
        return toks;
    }
}