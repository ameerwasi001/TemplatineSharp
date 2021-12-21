using System;
using System.Collections.Generic;
using System.Linq;

using BlockArguments = System.Collections.Generic.Dictionary<string, System.Collections.Generic.List<Node>>;

public class PatchBlocks : IVisitor<Node, BlockArguments>
{
    public Node Visit(NumNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(BoolNode node, BlockArguments _)
    {
        return node.Copy();
    }

    public Node Visit(RenderNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(StrNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(ListNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(ObjectNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(AccessProperty node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(VarAccessNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(BinOpNode node, BlockArguments ctx)
    {
        return node.Copy();
    }

    public Node Visit(CallNode callNode, BlockArguments ctx)
    {
        return callNode.Copy();
    }

    public Node Visit(BlockNode node, BlockArguments ctx)
    {
        var name = node.name;
        return new BlockNode(
            name,
            ctx[name].Select(a => a.Accept(this, ctx)).ToList(), 
            node.posStart.Copy(), 
            node.posEnd.Copy()
        );
    }

    public Node Visit(ForNode forNode, BlockArguments ctx)
    {
        return new ForNode(
            forNode.idents.Select(a => a).ToList(), 
            forNode.iterNode.Accept(this, ctx), 
            forNode.nodes.Select(a => a.Accept(this, ctx)).ToList(), 
            forNode.posStart.Copy(), 
            forNode.posEnd.Copy()
        );
    }

    public Node Visit(BatchRenderNode node, BlockArguments ctx)
    {
        return new BatchRenderNode(node.batch.Select(a => a.Accept(this, ctx)).ToList(), node.posStart.Copy(), node.posEnd.Copy());
    }

    public Node Visit(IfNode ifNode, BlockArguments ctx)
    {
        return new IfNode(
            ifNode.blocks.Select(ab => Tuple.Create(ab.Item1.Accept(this, ctx), ab.Item2.Select(a => a.Accept(this, ctx)).ToList())).ToList(), 
            ifNode.elseCase.Select(a => a.Accept(this, ctx)).ToList(), 
            ifNode.posStart.Copy(), 
            ifNode.posStart.Copy()
        );    
    }

    public List<Node> Visit(List<Node> nodes, Dictionary<string, List<Node>> blockArgs = null)
    {
        var args = blockArgs == null ? new BlockArguments() : blockArgs;
        return nodes.Select(a => a.Accept(this, args)).ToList();
    }
}