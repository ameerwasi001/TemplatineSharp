using System.Collections.Generic;
using System.Linq;

public interface IterableValue {
    IEnumerable<Value> GetIter();
}

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

    public Value setPos(Position start, Position end)
    {
        this.posStart = start;
        this.posEnd = end;
        return this;
    }

    public static Number Construct(int i)
    {
        return Number.Construct(i);
    }

    public static Str Construct(string s)
    {
        return Str.Construct(s);
    }

    public static Bool Construct(bool s)
    {
        return Bool.Construct(s);
    }

    public static IteratorValue Construct(IEnumerable<Value> vals)
    {
        return IteratorValue.Construct(vals);
    }

    public Value setContext(Context ctx)
    {
        this.context = ctx;
        return this;
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

    public virtual Value ee(Value other)
    {
        return new Bool(false, this.posStart, other.posEnd, context);
    }

    public virtual Value ne(Value other)
    {
        return new Bool(true, this.posStart, other.posEnd, context);
    }

    public virtual Value gte(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot use >= with {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value gt(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot use > with {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value lt(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot use < with {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value lte(Value other)
    {
        throw new RuntimeError(this.posStart, other.posEnd, string.Format("Cannot use <= with {0} and {1}", this.GetType(), other.GetType()));
    }

    public virtual Value anded(Value other)
    {
        return new Bool(this.IsTrue() && other.IsTrue(), this.posStart, other.posEnd, context);
    }

    public virtual Value ored(Value other)
    {
        return new Bool(this.IsTrue() || other.IsTrue(), this.posStart, other.posEnd, context);
    }

    public virtual bool IsTrue()
    {
        throw new RuntimeError(this.posStart, this.posEnd, string.Format("{0} has no intrinsic truth value", this.GetType()));
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

    public static Value operator> (Value a, Value b) {
        return a.gt(b);
    }

    public static Value operator>= (Value a, Value b) {
        return a.gte(b);
    }

    public static Value operator< (Value a, Value b) {
        return a.lt(b);
    }

    public static Value operator<= (Value a, Value b) {
        return a.lte(b);
    }

    public static Value operator& (Value a, Value b) {
        return a.anded(b);
    }

    public static Value operator| (Value a, Value b) {
        return a.ored(b);
    }
}

public class Number : Value
{
    private double num;

    public Number(double given, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        num = given;
    }

    public static Number Construct(double given)
    {
        return new Number(given, Position.Nothing(), Position.Nothing(), new Context());
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

    override public Value gte(Value other)
    {
        if (!(other is Number)) base.gte(other);
        return new Bool(this.num >= ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value ee(Value other)
    {
        if (!(other is Number)) base.ee(other);
        return new Bool(this.num == ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value ne(Value other)
    {
        if (!(other is Number)) base.ne(other);
        return new Bool(this.num != ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value gt(Value other)
    {
        if (!(other is Number)) base.gt(other);
        return new Bool(this.num > ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value lt(Value other)
    {
        if (!(other is Number)) base.lt(other);
        return new Bool(this.num < ((Number)other).num, this.posStart, other.posEnd, other.context);
    }

    override public Value lte(Value other)
    {
        if (!(other is Number)) base.lte(other);
        return new Bool(this.num <= ((Number)other).num, this.posStart, other.posEnd, other.context);
    }
}

public class Str : Value, IterableValue
{
    public string str;

    public Str(string val, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        str = val;
    }

    public IEnumerable<Value> GetIter()
    {
        foreach(var ch in str) yield return new Str(ch.ToString(), posStart, posEnd, context);
    }

    override public Value add(Value other)
    {
        if (!(other is Str)) base.add(other);
        return new Str(this.str + ((Str)other).str, this.posStart, other.posEnd, other.context);
    }

    public new static Str Construct(string given)
    {
        return new Str(given, Position.Nothing(), Position.Nothing(), new Context());
    }

    override public Value ee(Value other)
    {
        if (!(other is Str)) base.ee(other);
        return new Bool(this.str == ((Str)other).str, this.posStart, other.posEnd, other.context);
    }

    override public Value ne(Value other)
    {
        if (!(other is Str)) base.ne(other);
        return new Bool(this.str != ((Str)other).str, this.posStart, other.posEnd, other.context);
    }

    public override string ToString()
    {
        return str;
    }
}

public class Bool : Value
{
    public bool boolean;

    public Bool(bool b, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        boolean = b;
    }

    public new static Bool Construct(bool given)
    {
        return new Bool(given, Position.Nothing(), Position.Nothing(), new Context());
    }

    public override bool IsTrue()
    {
        return boolean;
    }

    public override string ToString()
    {
        return boolean ? "true" : "false";
    }
}

public class IteratorValue : Value, IterableValue
{
    public IEnumerable<Value> elems;

    public IteratorValue(IEnumerable<Value> nodes, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        elems = nodes;
    }

    public IEnumerable<Value> GetIter()
    {
        return elems;
    }

    public new static IteratorValue Construct(IEnumerable<Value> given)
    {
        return new IteratorValue(given, Position.Nothing(), Position.Nothing(), new Context());
    }

    public override string ToString()
    {
        return "[" + string.Join(", ", elems.Select(a => a.ToString())) + "]";
    }
}

public class BlockValue : Value, IterableValue
{
    public IEnumerable<Value> values;

    public BlockValue(IEnumerable<Value> nodes, Position start, Position end, Context ctx) : base(start, end, ctx)
    {
        values = nodes;
    }

    public IEnumerable<Value> GetIter()
    {
        return values;
    }

    public new static BlockValue Construct(IEnumerable<Value> given)
    {
        return new BlockValue(given, Position.Nothing(), Position.Nothing(), new Context());
    }

    public override string ToString()
    {
        return string.Concat(values.Select(a => a.ToString()));
    }
}