public interface Node
{
    T Accept<T, C>(IVisitor<T, C> visitor, C ctx);
    Position posStart {set; get;}
    Position posEnd {set; get;}
    Node Copy();
}

public interface IVisitor<T, C>
{
    T Visit(BinOpNode node, C ctx);
    T Visit(NumNode node, C ctx);
    T Visit(BoolNode node, C ctx);
    T Visit(StrNode node, C ctx);
    T Visit(ListNode node, C ctx);
    T Visit(ObjectNode node, C ctx);
    T Visit(AccessProperty node, C ctx);
    T Visit(VarAccessNode node, C ctx);
    T Visit(RenderNode node, C ctx);
    T Visit(ForNode node, C ctx);
    T Visit(IfNode node, C ctx);
    T Visit(CallNode node, C ctx);
}