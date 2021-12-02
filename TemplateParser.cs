using System.Collections.Generic;

public class TemplateParser
{
    public List<TemplateToken> toks;
    public int tokIdx = -1;
    public TemplateToken currentTok;
    public List<Node> nodes = new List<Node>(){};

    public TemplateParser(List<TemplateToken> tokens)
    {
        toks = tokens;
        this.Advance();
    }

    public void Advance()
    {
        tokIdx += 1;
        currentTok = tokIdx < toks.Count ? toks[tokIdx] : TemplateToken.EOFToken();
    }

    public List<Node> Parse()
    {
        while(currentTok.tokType != TemplateTokenType.EOF)
        {
            nodes.Add(this.ParseToken());
        }
        return nodes;
    }

    private Node ParseToken()
    {
        if(currentTok.tokType == TemplateTokenType.Render) return this.ParseRenderToken();
        else if (currentTok.tokType == TemplateTokenType.ForCue) return this.ParseForLoop();
        throw new InvalidSyntaxError(currentTok.posStart, currentTok.posEnd, "Expected either an expression or a statement cue");
    }

    private Node ParseRenderToken()
    {
        var node = currentTok.GetRenderNode();
        this.Advance();
        return node;
    }

    private Node ParseForLoop()
    {
        var iter = currentTok.GetIterNode();
        var ident = currentTok.GetIterIdent();
        var posStart = currentTok.posStart;
        this.Advance();
        var currentNodes = new List<Node>();

        while(currentTok.tokType != TemplateTokenType.EndForCue && currentTok.tokType != TemplateTokenType.EOF)
        {
            var currentNode = this.ParseToken();
            currentNodes.Add(currentNode);
        }

        if(currentTok.tokType != TemplateTokenType.EndForCue) throw new InvalidSyntaxError(posStart, currentTok.posEnd.Copy(), "Expected an endfor cue");
        this.Advance();
        return new ForNode(ident, iter, currentNodes, posStart, currentTok.posEnd);
    }
}