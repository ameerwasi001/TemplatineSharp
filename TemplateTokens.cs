using System;

public enum TemplateTokenType 
{
    ForCue,
    EndForCue,
    Render,
    EOF,
}

public class TemplateToken
{
    public Position posStart;
    public TemplateTokenType tokType;
    public Position posEnd;

    public TemplateToken(TemplateTokenType type_, Position start, Position end)
    {
        tokType = type_;
        posStart = start;
        posEnd = end;
    }

    public static TemplateToken EOFToken()
    {
        return new TemplateToken(TemplateTokenType.EOF, Position.Nothing(), Position.Nothing());
    }

    public virtual Tuple<Token, Node> GetForCueContent()
    {
        throw new Exception("This is not a for cue...");
    }

    public virtual Node GetRenderNode()
    {
        throw new Exception("This is not a render node...");
    }
}

public class RenderToken : TemplateToken 
{
    public Node node;

    public RenderToken(Node n, Position start, Position end) : base(TemplateTokenType.Render, start, end)
    {
        node = n;
    }

    override public Node GetRenderNode()
    {
        return node;
    }
}

public class ForCue : TemplateToken 
{
    public Node iteratorNode;
    public Token identifier;

    public ForCue(Token ident, Node node, Position start, Position end) : base(TemplateTokenType.ForCue, start, end)
    {
        iteratorNode = node;
        identifier = ident;
    }

    override public Tuple<Token, Node> GetForCueContent()
    {
        return Tuple.Create<Token, Node>(identifier, iteratorNode);
    }
}
