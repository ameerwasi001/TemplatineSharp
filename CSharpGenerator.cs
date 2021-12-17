using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

public class ReplaceString
{
    static readonly IDictionary<string, string> m_replaceDict
        = new Dictionary<string, string>();

    const string ms_regexEscapes = @"[\a\b\f\n\r\t\v\\""]";

    public static string StringLiteral(string i_string)
    {
        return Regex.Replace(i_string, ms_regexEscapes, match);
    }

    private static string match(Match m)
    {
        string match = m.ToString();
        if (m_replaceDict.ContainsKey(match))
        {
            return m_replaceDict[match];
        }

        throw new NotSupportedException();
    }

    static ReplaceString()
    {
        m_replaceDict.Add("\a", @"\a");
        m_replaceDict.Add("\b", @"\b");
        m_replaceDict.Add("\f", @"\f");
        m_replaceDict.Add("\n", @"\n");
        m_replaceDict.Add("\r", @"\r");
        m_replaceDict.Add("\t", @"\t");
        m_replaceDict.Add("\v", @"\v");
        m_replaceDict.Add("\\", @"\\");
        m_replaceDict.Add("\0", @"\0");
        m_replaceDict.Add("\"", "\\\"");
    }
}

public class CSharpGenerator : IVisitor<string, object>
{
    private static Dictionary<string, string> operatorTable = new Dictionary<string, string>{
        {Token.TT_PLUS, "+"},
        {Token.TT_MINUS, "-"},
        {Token.TT_MUL, "*"},
        {Token.TT_DIV, "/"},
        {Token.TT_AND, "&"},
        {Token.TT_OR, "|"},
        {Token.TT_GT, ">"},
        {Token.TT_GTE, ">="},
        {Token.TT_LT, "<"},
        {Token.TT_LTE, "<="},
        {Token.TT_EE, "=="},
        {Token.TT_NE, "!="},
    };

    private static string Indent(string str)
    {
        return string.Join("\n", str.Split("\n").Select(a => "\t" + a));
    }

    public string Visit(NumNode node, object ctx)
    {
        return string.Format("Value.Construct({0})", node.num.ToString());
    }

    public string Visit(BoolNode node, object _)
    {
        return string.Format("Value.Construct({0})", node.ToString());
    }

    public string Visit(RenderNode node, object ctx)
    {
        return string.Format("__generatedList.Add({0});", node.renderNode.Accept(this, ctx));
    }

    public string Visit(StrNode node, object ctx)
    {
        return string.Format("Value.Construct(\"{0}\")", ReplaceString.StringLiteral(node.str));
    }

    public string Visit(ListNode node, object ctx)
    {
        return string.Format("Value.Construct(new List<Value>(){0})", "{" + string.Join(", ", node.list.Select(x => x.Accept(this, ctx))) + "}");
    }

    public string Visit(ObjectNode node, object ctx)
    {
        var ls = string.Join(", ", node.keyValueList.Select(a => "{" + a.Item1.Accept(this, ctx) + ", " + a.Item2.Accept(this, ctx) + "}"));
        return "Value.Construct(new Dictionary<Value, Value>{" + ls + "})";
    }

    public string Visit(AccessProperty node, object ctx)
    {
        return node.node.Accept(this, ctx) + "." + string.Join(".", node.accessors);
    }

    public string Visit(VarAccessNode node, object ctx)
    {
        return node.ident;
    }

    public string Visit(BinOpNode node, object ctx)
    {
        return string.Format("({0} {1} {2})", node.left.Accept(this, ctx), operatorTable[node.op.tokType], node.right.Accept(this, ctx));
    }

    public string Visit(ForNode forNode, object ctx)
    {
        var iter = forNode.iterNode.Accept(this, ctx).ToString();
        var assigns = string.Concat(forNode.idents.Select((a, i) => "var " + a.value + " = _ls.ElementAt(" + i.ToString() + ");"));
        var block = "{\n" + Indent(assigns + "\n" + string.Join("\n", forNode.nodes.Select(a => a.Accept(this, ctx).ToString()))) + "\n}";
        return string.Format("foreach(var _ls in {0}.GetIterator()){1}", iter, block);
    }

    public string Visit(CallNode callNode, object ctx)
    {
        return string.Format("{0}.Call({1})", callNode.callee.Accept(this, ctx), string.Join(", ", callNode.args.Select(a => a.Accept(this, ctx).ToString())));
    }

    public string Visit(IfNode ifNode, object ctx)
    {
        var i = 0;
        var arr = new List<string>();
        foreach(var nodeBlock in ifNode.blocks)
        {
            var keyword = i == 0 ? "if" : "else if";
            var cond = keyword + "(" + nodeBlock.Item1.Accept(this, ctx).ToString() + ")";
            var block = "{\n" + Indent(string.Join("\n", nodeBlock.Item2.Select(a => a.Accept(this, ctx).ToString()))) + "\n}";
            arr.Add(cond + block);
            i += 1;
        }
        return string.Concat(arr) + "else{\n" + Indent(string.Join("\n", ifNode.elseCase.Select(a => a.Accept(this, ctx).ToString()))) + "\n}";
    }
}