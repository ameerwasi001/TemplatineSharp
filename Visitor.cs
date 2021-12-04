public interface Node
{
    T Accept<T>(IVisitor<T> visitor, Context ctx);
    Position posStart {set; get;}
    Position posEnd {set; get;}
}

public interface IVisitor<T>
{
    T Visit(BinOpNode node, Context ctx);
    T Visit(NumNode node, Context ctx);
    T Visit(BoolNode node, Context ctx);
    T Visit(StrNode node, Context ctx);
    T Visit(VarAccessNode node, Context ctx);
    T Visit(RenderNode node, Context ctx);
    T Visit(ForNode node, Context ctx);
    T Visit(IfNode node, Context ctx);
}