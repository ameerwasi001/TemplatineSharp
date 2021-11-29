using System;

public class Interpreter : IVisitor<Value>
{
    public Value Visit(NumNode node, Context ctx)
    {
        return new Number(node.num, node.posStart, node.posEnd, ctx);
    }

    public Value Visit(RenderNode node, Context ctx)
    {
        return node.renderNode.Accept(this, ctx);
    }

    public Value Visit(StrNode node, Context ctx)
    {
        return new Str(node.str, node.posStart, node.posEnd, ctx);
    }
    public Value Visit(BinOpNode node, Context ctx)
    {
        var left = node.left.Accept(this, ctx);
        var right = node.right.Accept(this, ctx);
        if (node.op.tokType == Token.TT_PLUS)
        {
            return left + right;
        } else if (node.op.tokType == Token.TT_MINUS)
        {
            return left - right;
        } else if (node.op.tokType == Token.TT_MUL)
        {
            return left * right;
        } else if (node.op.tokType == Token.TT_DIV)
        {
            return left / right;
        } else {
            throw new RuntimeError(node.posStart, node.posEnd, String.Format("{0} not a valid operator", node.op.tokType));
        }
    }

}