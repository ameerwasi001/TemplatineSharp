using System.Collections.Generic;
using System.Linq;

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

    public Value Visit(VarAccessNode node, Context ctx)
    {
        var val = ctx.Get(node.ident);
        if (val == null) throw new RuntimeError(node.posStart, node.posEnd, string.Format("Undefined variable {0}", node.ident));
        return val.setPos(node.posStart, node.posEnd).setContext(ctx);
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
            throw new RuntimeError(node.posStart, node.posEnd, string.Format("{0} not a valid operator", node.op.tokType));
        }
    }

    public Value Visit(ForNode forNode, Context ctx)
    {
        var nodes = forNode.nodes;
        var possiblyIter = forNode.iterNode.Accept(this, ctx);
        if(possiblyIter is IterableValue) {
            var iter = (IterableValue)possiblyIter;
            var outputs = new List<Value>();
            foreach(var value in iter.GetIter())
            {
                var newCtx = new Context(new SymbolTable(ctx.symbolTable), "<for-loop>", ctx, forNode.posStart);
                newCtx.Set(forNode.ident.value, value);
                foreach(var node in nodes) outputs.Add(node.Accept(this, newCtx));
            }
            return new ForLoopValue(outputs, forNode.posStart, forNode.posEnd, ctx);
        } else throw new RuntimeError(forNode.posStart, forNode.posEnd, "Cannot iterate over " + possiblyIter.ToString());
    }
}