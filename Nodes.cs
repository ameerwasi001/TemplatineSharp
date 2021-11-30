public class StrNode : Node
{
    public string str;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public StrNode(string val, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        str = val;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "\"" + this.str + "\"";
    }
}


public class NumNode : Node
{
    public double num;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}
    public NumNode(double val, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        num = val;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return this.num.ToString();
    }
}

public class VarAccessNode : Node
{
    public string ident;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}
    public VarAccessNode(string name, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        ident = name;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return this.ident.ToString();
    }
}

public class RenderNode : Node
{
    public Node renderNode;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public RenderNode(Node node, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        renderNode = node;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "{{" + this.renderNode.ToString() + "}}";
    }
}

public class BinOpNode : Node
{
    public Token op;
    public Node right;
    public Node left;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public BinOpNode(Node leftNode, Token opValue, Node rightNode, Position start, Position end)
    {
        left = leftNode;
        op = opValue;
        right = rightNode;
        posStart = start;
        posEnd = end;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "(" + left.ToString() + " " + op.tokType + " " + right.ToString() + ")";
    }
}