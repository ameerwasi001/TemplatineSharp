using System;
using System.Collections.Generic;
using System.Linq;

public class RenderAggregator : IVisitor<Node, object>
{

    private List<Node> AggregateBlock(List<Node> block)
    {
        var newBlock = new List<Node>();
        var renders = new List<Node>();
        foreach(var node in block)
        {
            if(node is RenderNode) renders.Add(node);
            else {
                if(renders.Count != 0) newBlock.Add(new BatchRenderNode(renders));
                renders = new List<Node>();
                newBlock.Add(node);
            }
        }
        if(renders.Count != 0) newBlock.Add(new BatchRenderNode(renders));
        renders = new List<Node>();
        return newBlock;
    }

    public Node Visit(ExtendsNode node, object ctx)
    {
        return node;
    }

    public Node Visit(NumNode node, object ctx)
    {
        return node;
    }

    public Node Visit(BoolNode node, object _)
    {
        return node;
    }

    public Node Visit(BatchRenderNode node, object ctx)
    {
        return node;
    }

    public Node Visit(RenderNode node, object ctx)
    {
        return node;
    }

    public Node Visit(StrNode node, object ctx)
    {
        return node;
    }

    public Node Visit(ListNode node, object ctx)
    {
        return node;
    }

    public Node Visit(ObjectNode node, object ctx)
    {
        return node;
    }

    public Node Visit(AccessProperty node, object ctx)
    {
        return node;
    }

    public Node Visit(VarAccessNode node, object ctx)
    {
        return node;
    }

    public Node Visit(BinOpNode node, object ctx)
    {
        return node;
    }

    public Node Visit(BlockNode node, object ctx)
    {
        return new BlockNode(
            node.name,
            this.AggregateBlock(node.block.Select(a => a.Accept(this, ctx)).ToList()), 
            node.posStart.Copy(), 
            node.posEnd.Copy()
        );
    }

    public Node Visit(ForNode forNode, object ctx)
    {
        return new ForNode(
            forNode.idents.Select(a => a).ToList(), 
            forNode.iterNode.Accept(this, ctx), 
            this.AggregateBlock(forNode.nodes.Select(a => a.Accept(this, ctx)).ToList()), 
            forNode.posStart.Copy(), 
            forNode.posEnd.Copy()
        );
    }

    public Node Visit(CallNode callNode, object ctx)
    {
        return callNode;
    }

    public Node Visit(IfNode ifNode, object ctx)
    {
        return new IfNode(
            ifNode.blocks.Select(ab => Tuple.Create(ab.Item1.Accept(this, ctx), this.AggregateBlock(ab.Item2.Select(a => a.Accept(this, ctx)).ToList()))).ToList(), 
            this.AggregateBlock(ifNode.elseCase.Select(a => a.Accept(this, ctx)).ToList()), 
            ifNode.posStart.Copy(), 
            ifNode.posStart.Copy()
        );    
    }

    public List<Node> Visit(List<Node> nodes)
    {
        return this.AggregateBlock(nodes.Select(a => a.Accept(this, null)).ToList());
    }
}