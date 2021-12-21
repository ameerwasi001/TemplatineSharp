using System;
using System.Collections.Generic;

public enum TemplateTokenType 
{
    ForCue,
    EndForCue,
    IfCue,
    ElifCue,
    ElseCue,
    EndIfCue,
    BlockCue,
    EndBlockCue,
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

    public virtual Tuple<List<Token>, Node> GetForCueContent()
    {
        throw new Exception("This is not a for cue...");
    }

    public virtual Node GetRenderNode()
    {
        throw new Exception("This is not a render node...");
    }

    public virtual string GetBlockString()
    {
        throw new Exception("This is not a block node...");
    }

    public virtual Node GetIfCueCond()
    {
        throw new Exception("This is not an IfCue...");
    }

    public virtual Node GetElifCueCond()
    {
        throw new Exception("This is not an IfCue...");
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
    public List<Token> identifiers;

    public ForCue(List<Token> idents, Node node, Position start, Position end) : base(TemplateTokenType.ForCue, start, end)
    {
        iteratorNode = node;
        identifiers = idents;
    }

    override public Tuple<List<Token>, Node> GetForCueContent()
    {
        return Tuple.Create<List<Token>, Node>(identifiers, iteratorNode);
    }
}

public class BlockCue : TemplateToken 
{
    public string name;

    public BlockCue(string str, Position start, Position end) : base(TemplateTokenType.BlockCue, start, end)
    {
        name = str;
    }

    override public string GetBlockString()
    {
        return name;
    }
}

public class IfCue : TemplateToken 
{
    public Node cond;

    public IfCue(Node c, Position start, Position end) : base(TemplateTokenType.IfCue, start, end)
    {
        cond = c;
    }

    override public Node GetIfCueCond()
    {
        return cond;
    }
}

public class ElifCue : TemplateToken 
{
    public Node cond;

    public ElifCue(Node c, Position start, Position end) : base(TemplateTokenType.ElifCue, start, end)
    {
        cond = c;
    }

    override public Node GetElifCueCond()
    {
        return cond;
    }
}