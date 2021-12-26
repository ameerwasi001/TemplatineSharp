using System;
using System.Collections.Generic;
using System.Linq;

public class RenderAggregator : IVisitor<List<Node>, object>
{

    private List<Node> AggregateBatch(List<Node> block)
    {
        var newBlock = new List<Node>();
        var batches = new List<Node>();
        foreach(var node in block)
        {
            if(node is BatchRenderNode) {
                foreach(var iNode in ((BatchRenderNode)node).batch)
                {
                    batches.Add(iNode);
                }
            } else {
                if(batches.Count != 0) newBlock.Add(new BatchRenderNode(batches));
                batches = new List<Node>();
                newBlock.Add(node);
            }
        }
        if(batches.Count != 0) newBlock.Add(new BatchRenderNode(batches));
        batches = new List<Node>();
        return newBlock;
    }

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
        return AggregateBatch(newBlock);
    }

    public List<Node> Visit(ExtendsNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(NumNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(BoolNode node, object _)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(BatchRenderNode node, object ctx)
    {
        return node.batch;
    }

    public List<Node> Visit(RenderNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(StrNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(ListNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(ObjectNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(AccessProperty node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(VarAccessNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(BinOpNode node, object ctx)
    {
        return new List<Node>{node};
    }

    public List<Node> Visit(BlockNode node, object ctx)
    {
        return this.AggregateBlock(node.block.Select(a => a.Accept(this, ctx)).SelectMany(x => x).ToList());
    }

    public List<Node> Visit(ForNode forNode, object ctx)
    {
        return new List<Node>{
            new ForNode(
                forNode.idents.Select(a => a).ToList(), 
                forNode.iterNode,
                this.AggregateBlock(forNode.nodes.Select(a => a.Accept(this, ctx)).SelectMany(x => x).ToList()), 
                forNode.posStart.Copy(), 
                forNode.posEnd.Copy()
            )
        };
    }

    public List<Node> Visit(CallNode callNode, object ctx)
    {
        return new List<Node>{callNode};
    }

    public List<Node> Visit(IfNode ifNode, object ctx)
    {
        return new List<Node>{
            new IfNode(
                ifNode.blocks.Select(ab => Tuple.Create(ab.Item1, this.AggregateBlock(ab.Item2.Select(a => a.Accept(this, ctx)).SelectMany(x => x).ToList()))).ToList(), 
                this.AggregateBlock(ifNode.elseCase.Select(a => a.Accept(this, ctx)).SelectMany(x => x).ToList()), 
                ifNode.posStart.Copy(), 
                ifNode.posStart.Copy()
            )
        };
    }

    public List<Node> Visit(List<Node> nodes)
    {
        return this.AggregateBlock(nodes.Select(a => a.Accept(this, null)).SelectMany(x => x).ToList());
    }
}