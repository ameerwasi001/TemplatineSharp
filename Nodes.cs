using System;
using System.Collections.Generic;
using System.Linq;

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

public class BoolNode : Node
{
    public bool boolean;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public BoolNode(bool val, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        boolean = val;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return boolean ? "true" : "false";
    }
}

public class ListNode : Node
{
    public List<Node> list;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public ListNode(List<Node> val, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        list = val;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "[" + string.Join(", ", list.Select(a => a.ToString())) + "]";
    }
}

public class ObjectNode : Node
{
    public List<Tuple<Node, Node>> keyValueList;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public ObjectNode(List<Tuple<Node, Node>> val, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        keyValueList = val;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "{" + string.Join(", ", keyValueList.Select(a => a.Item1.ToString() + ": " + a.Item2.ToString())) + "}";
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

public class ForNode : Node
{
    public Token ident;
    public Node iterNode;
    public List<Node> nodes;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public ForNode(Token id, Node iter, List<Node> ns, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        nodes = ns;
        iterNode = iter;
        ident = id;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "for " + ident.ToString() + " in " + iterNode.ToString() + " {\n" + string.Join(";\n", this.nodes.Select(node => "\t" + node.ToString())) + "\n}";
    }
}

public class IfNode : Node
{
    public Position posStart {set; get;}
    public Position posEnd {set; get;}
    public List<Tuple<Node, List<Node>>> blocks;
    public List<Node> elseCase;

    public IfNode(List<Tuple<Node, List<Node>>> nodes, List<Node> finalCase, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        blocks = nodes;
        elseCase = finalCase;
    }

    public T Accept<T>(IVisitor<T> visitor, Context ctx)
    {
        return visitor.Visit(this, ctx);
    }

    override public string ToString()
    {
        var strs = new List<string>();
        foreach(var contents in blocks)
        {
            var cond = contents.Item1.ToString();
            var block = string.Concat(contents.Item2.Select(a => a.ToString()));
            strs.Add(string.Format("case {0}: {1}", cond, block));
        }
        return string.Join("\n", strs) + string.Format("\nelse: {0}", string.Concat(elseCase.Select(a => a.ToString())));
    }
}