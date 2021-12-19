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

    public Node Copy()
    {
        return new StrNode(str, posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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

    public Node Copy()
    {
        return new BoolNode(boolean, posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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

    public Node Copy()
    {
        return new ListNode(list.Select(a => a.Copy()).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "[" + string.Join(", ", list.Select(a => a.ToString())) + "]";
    }
}

public class BatchRenderNode : Node
{
    public List<Node> batch;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public BatchRenderNode(List<Node> nodes, Position start = null, Position end = null)
    {
        if(start == null) start = Position.Nothing();
        if(end == null) end = Position.Nothing();
        posStart = start;
        posEnd = end;
        batch = nodes;
    }

    public Node Copy()
    {
        return new ListNode(batch.Select(a => a.Copy()).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return string.Join(" + ", batch.Select(a => a.ToString()));
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

    public Node Copy()
    {
        return new ObjectNode(keyValueList.Select(ab => Tuple.Create(ab.Item1.Copy(), ab.Item2.Copy())).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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

    public Node Copy()
    {
        return new NumNode(num, posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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

    public Node Copy()
    {
        return new VarAccessNode(ident, posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return this.ident.ToString();
    }
}

public class AccessProperty : Node
{
    public List<string> accessors;
    public Node node;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}
    public AccessProperty(Node given, List<string> accs, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        accessors = accs;
        node = given;
    }

    public Node Copy()
    {
        return new AccessProperty(node.Copy(), accessors.Select(a => a).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return string.Format("{0}.{1}", node.ToString(), string.Join(".", this.accessors));
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

    public Node Copy()
    {
        return new RenderNode(renderNode.Copy(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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

    public Node Copy()
    {
        return new BinOpNode(left.Copy(), op, right.Copy(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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
    public List<Token> idents;
    public Node iterNode;
    public List<Node> nodes;
    public Position posStart {set; get;}
    public Position posEnd {set; get;}

    public ForNode(List<Token> id, Node iter, List<Node> ns, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        nodes = ns;
        iterNode = iter;
        idents = id;
    }

    public Node Copy()
    {
        return new ForNode(idents, iterNode.Copy(), nodes.Select(a => a.Copy()).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
    {
        return visitor.Visit(this, ctx);
    }

    public override string ToString()
    {
        return "for " + string.Join(", ", idents.Select(x => x.ToString())) + " in " + iterNode.ToString() + " {\n" + string.Join(";\n", this.nodes.Select(node => "\t" + node.ToString())) + "\n}";
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

    public Node Copy()
    {
        return new IfNode(blocks.Select(ab => Tuple.Create(ab.Item1.Copy(), ab.Item2.Select(a => a.Copy()).ToList())).ToList(), elseCase.Select(a => a.Copy()).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
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

public class CallNode : Node
{
    public Position posStart {set; get;}
    public Position posEnd {set; get;}
    public Node callee;
    public List<Node> args;

    public CallNode(Node node, List<Node> givenArgs, Position start, Position end)
    {
        posStart = start;
        posEnd = end;
        callee = node;
        args = givenArgs;
    }

    public Node Copy()
    {
        return new CallNode(callee.Copy(), args.Select(a => a.Copy()).ToList(), posStart.Copy(), posEnd.Copy());
    }

    public T Accept<T, C>(IVisitor<T, C> visitor, C ctx)
    {
        return visitor.Visit(this, ctx);
    }

    override public string ToString()
    {
        var str = args.Count == 0 ? "" : args.Select(a => a.ToString()).Aggregate((str, obj) => str + "," + obj.ToString());;
        return string.Format("\n{0}({1})", callee.ToString(), str);
    }
}
