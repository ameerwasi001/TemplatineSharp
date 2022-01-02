using System;
using System.Collections.Generic;

public class NestedExtendsError : IVisitor<object, object>
{
    public int nestedExtendsCount = 0;

    public object Visit(ExtendsNode node, object ctx)
    {
        if(nestedExtendsCount > 0) throw new NestedExtensionError(node.posStart, node.posEnd);
        return null;
    }

    public object Visit(NumNode node, object ctx)
    {
        return null;
    }

    public object Visit(BoolNode node, object _)
    {
        return null;
    }

    public object Visit(BlockNode node, object ctx)
    {
        nestedExtendsCount += 1;
        foreach(var item in node.block) item.Accept(this, ctx);
        nestedExtendsCount -= 1;
        return null;
    }

    public object Visit(BatchRenderNode node, object ctx)
    {
        foreach(var item in node.batch) item.Accept(this, ctx);
        return null;
    }

    public object Visit(RenderNode node, object ctx)
    {
        return node.renderNode.Accept(this, ctx);
    }

    public object Visit(StrNode node, object ctx)
    {
        return null;
    }

    public object Visit(ListNode node, object ctx)
    {
        foreach(var item in node.list) item.Accept(this, ctx);
        return null;
    }

    public object Visit(ObjectNode node, object ctx)
    {
        foreach(var kv in node.keyValueList)
        {
            kv.Item1.Accept(this, ctx);
            kv.Item2.Accept(this, ctx);
        }
        return null;
    }

    public object Visit(AccessProperty node, object ctx)
    {
        node.node.Accept(this, ctx);
        return null;
    }

    public object Visit(VarAccessNode node, object ctx)
    {
        return null;
    }

    public object Visit(BinOpNode node, object ctx)
    {
        node.left.Accept(this, ctx);
        node.right.Accept(this, ctx);
        return null;
    }

    public object Visit(ForNode forNode, object ctx)
    {
        forNode.iterNode.Accept(this, ctx);
        var newCtx = new HashSet<string>();
        nestedExtendsCount += 1;
        foreach(var node in forNode.nodes) node.Accept(this, newCtx);
        nestedExtendsCount -= 1;
        return null;
    }

    public object Visit(CallNode callNode, object ctx)
    {
        callNode.callee.Accept(this, ctx);
        foreach(var arg in callNode.args) arg.Accept(this, ctx);
        return null;
    }

    public object Visit(IfNode ifNode, object ctx)
    {
        foreach(var condBlock in ifNode.blocks)
        {
            condBlock.Item1.Accept(this, ctx);
            nestedExtendsCount += 1;
            foreach(var stmnt in condBlock.Item2) stmnt.Accept(this, ctx);
            nestedExtendsCount -= 1;
        }
        nestedExtendsCount += 1;
        foreach(var stmnt in ifNode.elseCase) stmnt.Accept(this, ctx);
        nestedExtendsCount -= 1;
        return null;
    }

    public object Visit(List<Node> nodes)
    {
        foreach(var node in nodes) node.Accept(this, null);
        return null;
    }
}