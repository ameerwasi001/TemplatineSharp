using System;
using System.Collections.Generic;
using System.Linq;

using ContextValidator = System.Collections.Generic.HashSet<string>;

public class EnvironmentGenerator : IVisitor<object, ContextValidator>
{
    public object Visit(NumNode node, ContextValidator ctx)
    {
        return null;
    }

    public object Visit(BoolNode node, ContextValidator _)
    {
        return null;
    }

    public object Visit(RenderNode node, ContextValidator ctx)
    {
        return node.renderNode.Accept(this, ctx);
    }

    public object Visit(StrNode node, ContextValidator ctx)
    {
        return null;
    }

    public object Visit(ListNode node, ContextValidator ctx)
    {
        foreach(var item in node.list) item.Accept(this, ctx);
        return null;
    }

    public object Visit(ObjectNode node, ContextValidator ctx)
    {
        foreach(var kv in node.keyValueList)
        {
            kv.Item1.Accept(this, ctx);
            kv.Item2.Accept(this, ctx);
        }
        return null;
    }

    public object Visit(AccessProperty node, ContextValidator ctx)
    {
        node.node.Accept(this, ctx);
        return null;
    }

    public object Visit(VarAccessNode node, ContextValidator ctx)
    {
        ctx.Add(node.ident);
        return null;
    }

    public object Visit(BinOpNode node, ContextValidator ctx)
    {
        node.left.Accept(this, ctx);
        node.right.Accept(this, ctx);
        return null;
    }

    public object Visit(ForNode forNode, ContextValidator ctx)
    {
        forNode.iterNode.Accept(this, ctx);
        var newCtx = new HashSet<string>();
        foreach(var node in forNode.nodes) node.Accept(this, newCtx);
        var idents = new ContextValidator(forNode.idents.Select(a => a.value));
        var ls = idents.Except(newCtx);
        foreach(var str in ls) ctx.Add(str);
        return null;
    }

    public object Visit(CallNode callNode, ContextValidator ctx)
    {
        callNode.callee.Accept(this, ctx);
        foreach(var arg in callNode.args) arg.Accept(this, ctx);
        return null;
    }

    public object Visit(IfNode ifNode, ContextValidator ctx)
    {
        foreach(var condBlock in ifNode.blocks)
        {
            condBlock.Item1.Accept(this, ctx);
            foreach(var stmnt in condBlock.Item2) stmnt.Accept(this, ctx);
        }
        foreach(var stmnt in ifNode.elseCase) stmnt.Accept(this, ctx);
        return null;
    }
}