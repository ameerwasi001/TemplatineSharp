public class Position {
    public int idx {get; private set;} = -1;
    public int ln {get; private set;} = 0;
    public int col {get; private set;} = -1;
    public string fn {get; private set;}
    public string ftxt {get; private set;}

    public Position(int idxV, int lnV, int colV, string fnV, string ftxtV){
        idx = idxV;
        ln = lnV;
        col = colV;
        fn = fnV;
        ftxt = ftxtV;
    }

    public Position(string fnV, string ftxtV){
        idx = -1;
        ln = 0;
        col = -1;
        fn = fnV;
        ftxt = ftxtV;
    }

    public void ResetIndexing()
    {
        idx = -1;
    }

    public Position Advance(string currentChar = "")
    {
        this.idx += 1;
        this.col += 1;
        if (currentChar == "\n")
        {
            this.ln += 1;
            this.col = 0;
        }
        return this;
    }

    public Position Copy()
    {
        return new Position(this.idx, this.ln, this.col, this.fn, this.ftxt);
    }
}