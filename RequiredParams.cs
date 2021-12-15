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
        node.list.Select(a => a.Accept(this, ctx)).ToList();
        return null;
    }

    public object Visit(ObjectNode node, ContextValidator ctx)
    {
        node.keyValueList.Select(kv => Tuple.Create(kv.Item1.Accept(this, ctx), kv.Item2.Accept(this, ctx))).ToList();
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
        forNode.nodes.Select(a => a.Accept(this, newCtx)).ToList();
        var idents = new ContextValidator(forNode.idents.Select(a => a.value));
        var ls = idents.Except(newCtx);
        foreach(var str in ls) ctx.Add(str);
        return null;
    }

    public object Visit(CallNode callNode, ContextValidator ctx)
    {
        callNode.callee.Accept(this, ctx);
        callNode.args.Select(a => a.Accept(this, ctx)).ToList();
        return null;
    }

    public object Visit(IfNode ifNode, ContextValidator ctx)
    {
        ifNode.blocks.Select(ab => Tuple.Create(ab.Item1.Accept(this, ctx), ab.Item2.Select(a => a.Accept(this, ctx)).ToList())).ToList();
        ifNode.elseCase.Select(a => a.Accept(this, ctx)).ToList();
        return null;
    }
}