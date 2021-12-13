using System;
using System.Collections.Generic;
using System.Linq;

public class PipeEliminator : IVisitor<Node>
{
    public Node Visit(NumNode node, Context _)
    {
        return node.Copy();
    }

    public Node Visit(BoolNode node, Context _)
    {
        return node.Copy();
    }

    public Node Visit(RenderNode node, Context ctx)
    {
        return new RenderNode(node.renderNode.Accept(this, ctx), node.posStart.Copy(), node.posEnd.Copy());
    }

    public Node Visit(StrNode node, Context ctx)
    {
        return node.Copy();
    }

    public Node Visit(ListNode node, Context ctx)
    {
        return new ListNode(node.list.Select(a => a.Accept(this, ctx)).ToList(), node.posStart.Copy(), node.posEnd.Copy());
    }

    public Node Visit(ObjectNode node, Context ctx)
    {
        var dict = node.keyValueList.Select(kv => Tuple.Create(kv.Item1.Accept(this, ctx), kv.Item2.Accept(this, ctx))).ToList();
        return new ObjectNode(dict, node.posStart, node.posEnd);
    }

    public Node Visit(AccessProperty node, Context ctx)
    {
        return new AccessProperty(node.node.Accept(this, ctx), node.accessors.Select(a => a).ToList(), node.posStart, node.posEnd);
    }

    public Node Visit(VarAccessNode node, Context ctx)
    {
        return node.Copy();
    }

    public Node Visit(BinOpNode node, Context ctx)
    {
        if(node.op.tokType == Token.TT_PIPE) {
            var leftNode = node.left;
            var rightNode = node.right;
            if(node.right is CallNode)
            {
                var callNode = (CallNode)rightNode.Accept(this, ctx);
                callNode.args.Add(leftNode.Accept(this, ctx));
                return callNode.Accept(this, ctx);
            }
            return new CallNode(rightNode, new List<Node>(){leftNode}, node.posStart, node.posEnd).Accept(this, ctx);
        }
        return new BinOpNode(node.left.Accept(this, ctx), node.op, node.right.Accept(this, ctx), node.posStart.Copy(), node.posEnd.Copy());
    }

    public Node Visit(ForNode forNode, Context ctx)
    {
        return new ForNode(
            forNode.idents.Select(a => a).ToList(), 
            forNode.iterNode.Accept(this, ctx), 
            forNode.nodes.Select(a => a.Accept(this, ctx)).ToList(), 
            forNode.posStart.Copy(), 
            forNode.posEnd.Copy()
        );
    }

    public Node Visit(CallNode callNode, Context ctx)
    {
        return new CallNode(
            callNode.callee.Accept(this, ctx), 
            callNode.args.Select(a => a.Accept(this, ctx)).ToList(), 
            callNode.posStart.Copy(), 
            callNode.posEnd.Copy()
        );
    }

    public Node Visit(IfNode ifNode, Context ctx)
    {
        return new IfNode(
            ifNode.blocks.Select(ab => Tuple.Create(ab.Item1.Accept(this, ctx), ab.Item2.Select(a => a.Accept(this, ctx)).ToList())).ToList(), 
            ifNode.elseCase.Select(a => a.Accept(this, ctx)).ToList(), 
            ifNode.posStart.Copy(), 
            ifNode.posStart.Copy()
        );
    }
}