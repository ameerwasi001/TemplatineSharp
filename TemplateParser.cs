using System;
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
        else if (currentTok.tokType == TemplateTokenType.IfCue) return this.ParseIfChain();
        throw new InvalidSyntaxError(currentTok.posStart, currentTok.posEnd, string.Format("Expected either an expression or a statement cue got {0}", currentTok.tokType.ToString()));
    }

    private Node ParseRenderToken()
    {
        var node = currentTok.GetRenderNode();
        this.Advance();
        return new RenderNode(node, node.posStart, node.posEnd);
    }

    private Node ParseForLoop()
    {
        var forLoopContent = currentTok.GetForCueContent();
        var ident = forLoopContent.Item1;
        var iter = forLoopContent.Item2;
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

    private Node ParseIfChain()
    {
        var firstCond = currentTok.GetIfCueCond();
        var nodes = new List<Tuple<Node, List<Node>>>();
        var firstNodes = new List<Node>();
        var elseCase = new List<Node>();
        var posStart = currentTok.posStart;
        this.Advance();
        while(
            currentTok.tokType != TemplateTokenType.ElifCue 
            && currentTok.tokType != TemplateTokenType.ElseCue 
            && currentTok.tokType != TemplateTokenType.EndIfCue 
            && currentTok.tokType != TemplateTokenType.EOF
        ) firstNodes.Add(this.ParseToken());
        if (currentTok.tokType == TemplateTokenType.EOF) throw new InvalidSyntaxError(posStart, currentTok.posEnd.Copy(), "Expected elif or an endif cue");
        nodes.Add(Tuple.Create<Node, List<Node>>(firstCond, firstNodes));
        while (currentTok.tokType == TemplateTokenType.ElifCue)
        {
            var currentNodes = new List<Node>();
            var cond = currentTok.GetElifCueCond();
            this.Advance();
            while (
                currentTok.tokType != TemplateTokenType.ElifCue 
                && currentTok.tokType != TemplateTokenType.EndIfCue 
                && currentTok.tokType != TemplateTokenType.EOF 
                && currentTok.tokType != TemplateTokenType.ElseCue
            ) currentNodes.Add(this.ParseToken());
            if (currentTok.tokType == TemplateTokenType.EOF) throw new InvalidSyntaxError(posStart, currentTok.posEnd.Copy(), "Expected elif or an endif cue");
            nodes.Add(Tuple.Create<Node, List<Node>>(cond, currentNodes));
        }
        if(currentTok.tokType == TemplateTokenType.ElseCue)
        {
            this.Advance();
            while (currentTok.tokType != TemplateTokenType.EndIfCue && currentTok.tokType != TemplateTokenType.EndIfCue) elseCase.Add(this.ParseToken());
        }
        if (currentTok.tokType != TemplateTokenType.EndIfCue) throw new InvalidSyntaxError(posStart, currentTok.posEnd.Copy(), "Expected an endif cue");
        this.Advance();
        return new IfNode(nodes, elseCase, posStart, currentTok.posEnd.Copy());
    }
}