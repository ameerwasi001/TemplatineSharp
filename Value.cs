public class Value 
{
    public Position posStart;
    public Position posEnd;
    public Context context;

    protected Value(Position start, Position end, Context ctx)
    {
        posStart = start;
        posEnd = end;
        context = ctx;
    }

    public virtual Value add(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot add {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value sub(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot subtract {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value mul(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot multiply {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value div(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot divide {0} and {1}", this.GetType(), other.GetType()));
    }

    public static Value operator+ (Value a, Value b) {
        return a.add(b);
    }

    public static Value operator- (Value a, Value b) {
        return a.sub(b);
    }

    public static Value operator* (Value a, Value b) {
        return a.mul(b);
    }

    public static Value operator/ (Value a, Value b) {
        return a.div(b);
    }
}

public class Number : Value
{
    private double num;

    public Number(double given, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        num = given;
    }

    public override string ToString()
    {
        return num.ToString();
    }

    override public Value add(Value other)
    {
        if (!(other is Number)) base.add(other);
        return new Number(this.num + ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value sub(Value other)
    {
        if (!(other is Number)) base.sub(other);
        return new Number(this.num - ((Number)other).num, this.posStart, other.posEnd, other.context);    
    }

    override public Value mul(Value other)
    {
        if (!(other is Number)) base.sub(other);
        return new Number(this.num * ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value div(Value other)
    {
        if (!(other is Number)) base.div(other);
        if (((Number)other).num == 0) throw new RuntimeError(other.posStart, other.posEnd, "Division by zero is undefined");
        return new Number(this.num / ((Number)other).num, this.posStart, other.posEnd, other.context);
    }
}

public class Str : Value
{
    public string str;

    public Str(string val, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        str = val;
    }

    public override string ToString()
    {
        return str;
    }
}