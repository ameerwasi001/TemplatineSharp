using System.Collections.Generic;
using System.Linq;

public class Interpreter : IVisitor<Value>
{
    public Value Visit(NumNode node, Context ctx)
    {
        return new Number(node.num, node.posStart, node.posEnd, ctx);
    }

    public Value Visit(BoolNode node, Context ctx)
    {
        return new Bool(node.boolean, node.posStart, node.posEnd, ctx);
    }

    public Value Visit(RenderNode node, Context ctx)
    {
        return node.renderNode.Accept(this, ctx);
    }

    public Value Visit(StrNode node, Context ctx)
    {
        return new Str(node.str, node.posStart, node.posEnd, ctx);
    }

    public Value Visit(ListNode node, Context ctx)
    {
        return new IteratorValue(node.list.Select(a => a.Accept(this, ctx)), node.posStart, node.posEnd, ctx);
    }

    public Value Visit(ObjectNode node, Context ctx)
    {
        var dict = node.keyValueList.ToDictionary(kv => kv.Item1.Accept(this, ctx), kv => kv.Item2.Accept(this, ctx));
        return new ObjectValue(dict, node.posStart, node.posEnd, ctx);
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
        } else if (node.op.tokType == Token.TT_AND)
        {
            return left & right;
        } else if (node.op.tokType == Token.TT_GT)
        {
            return left > right;
        } else if (node.op.tokType == Token.TT_GTE)
        {
            return left >= right;
        } else if (node.op.tokType == Token.TT_LT)
        {
            return left < right;
        } else if (node.op.tokType == Token.TT_LTE)
        {
            return left <= right;
        } else if (node.op.tokType == Token.TT_EE)
        {
            return left.ee(right);
        } else if (node.op.tokType == Token.TT_NE)
        {
            return left.ne(right);
        } else if (node.op.tokType == Token.TT_OR)
        {
            return left | right;
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
            return new BlockValue(outputs, forNode.posStart, forNode.posEnd, ctx);
        } else throw new RuntimeError(forNode.posStart, forNode.posEnd, "Cannot iterate over " + possiblyIter.ToString());
    }

    public Value Visit(IfNode ifNode, Context ctx)
    {
        // System.Console.WriteLine(ifNode);
        foreach(var content in ifNode.blocks)
        {
            var cond = content.Item1;
            var block = content.Item2;
            if (cond.Accept(this, ctx).IsTrue())
            {
                var newCtx = new Context(new SymbolTable(ctx.symbolTable), "<if-stmnt>", ctx, cond.posStart);
                return new BlockValue(block.Select(a => a.Accept(this, newCtx)), cond.posStart, cond.posEnd, newCtx);
            }
        }
        var elseCtx = new Context(new SymbolTable(ctx.symbolTable), "<else-stmnt>", ctx, ifNode.posStart);
        return new BlockValue(ifNode.elseCase.Select(a => a.Accept(this, elseCtx)), ifNode.posStart, ifNode.posEnd, elseCtx);;
    }
}