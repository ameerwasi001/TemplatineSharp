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
    private int forloopNestCount = 0;
    private static Dictionary<string, Func<String, String, String>> operatorTable = new Dictionary<string, Func<String, String, String>>{
        {Token.TT_PLUS, (a, b) => a + "+" + b},
        {Token.TT_MINUS, (a, b) => a + "-" + b},
        {Token.TT_MUL, (a, b) => a + "*" + b},
        {Token.TT_DIV, (a, b) => a + "/" + b},
        {Token.TT_AND, (a, b) => a + "&" + b},
        {Token.TT_OR, (a, b) => a + "|" + b},
        {Token.TT_GT, (a, b) => a + ">" + b},
        {Token.TT_GTE, (a, b) => a + ">=" + b},
        {Token.TT_LT, (a, b) => a + "<" + b},
        {Token.TT_LTE, (a, b) => a + "<=" + b},
        {Token.TT_EE, (a, b) => a + ".ee(" + b + ")"},
        {Token.TT_NE, (a, b) => a + ".ne(" + b + ")"},
    };

    private static string Indent(string str)
    {
        return string.Join("\n", str.Split("\n").Select(a => "\t" + a));
    }

    public string Visit(ExtendsNode node, object ctx)
    {
        return "";
    }

    public string Visit(NumNode node, object ctx)
    {
        return string.Format("Value.Construct({0})", node.num.ToString());
    }

    public string Visit(BoolNode node, object _)
    {
        return string.Format("Value.Construct({0})", node.ToString());
    }

    public string Visit(BlockNode node, object ctx)
    {
        return string.Join("\n", node.block.Select(a => a.Accept(this, ctx)));
    }

    public string Visit(BatchRenderNode node, object ctx)
    {
        return "__generatedString += " + string.Join(" + ", node.batch.Select(a => a.Accept(this, ctx) + ".ToString()")) + ";";
    }

    public string Visit(RenderNode node, object ctx)
    {
        return node.renderNode.Accept(this, ctx);
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
        return node.node.Accept(this, ctx) + string.Concat(node.accessors.Select(a => "[\"" + a.ToString() + "\"]"));
    }

    public string Visit(VarAccessNode node, object ctx)
    {
        return node.ident;
    }

    public string Visit(BinOpNode node, object ctx)
    {
        return "(" + operatorTable[node.op.tokType](node.left.Accept(this, ctx), node.right.Accept(this, ctx)) + ")";
    }

    public string Visit(ForNode forNode, object ctx)
    {
        var iter = forNode.iterNode.Accept(this, ctx).ToString();
        var lsName = "_ls" + forloopNestCount.ToString();
        var assigns = string.Concat(forNode.idents.Select((a, i) => "var " + a.value + " = " + lsName + ".ElementAt(" + i.ToString() + ");"));
        forloopNestCount++;
        var block = "{\n" + Indent(assigns + "\n" + string.Join("\n", forNode.nodes.Select(a => a.Accept(this, ctx).ToString()))) + "\n}";
        forloopNestCount--;
        return string.Format("foreach(var {0} in {1}.GetIterator()){2}", lsName, iter, block);
    }

    public string Visit(CallNode callNode, object ctx)
    {
        return string.Format("{0}.Call(new List<Value>(){1})",  callNode.callee.Accept(this, ctx), "{" + string.Join(", ", callNode.args.Select(a => a.Accept(this, ctx).ToString())) + "}");
    }

    public string Visit(IfNode ifNode, object ctx)
    {
        var i = 0;
        var arr = new List<string>();
        foreach(var nodeBlock in ifNode.blocks)
        {
            var keyword = i == 0 ? "if" : "else if";
            var cond = keyword + "((" + nodeBlock.Item1.Accept(this, ctx).ToString() + ").IsTrue())";
            var branchCode = string.Join("\n", nodeBlock.Item2.Select(a => a.Accept(this, ctx).ToString()));
            var block = "{\n" + Indent(branchCode) + "\n}";
            arr.Add(cond + block);
            i += 1;
        }
        var elseCode = string.Join("\n", ifNode.elseCase.Select(a => a.Accept(this, ctx).ToString()));
        return string.Concat(arr) + "else{\n" + Indent(elseCode) + "\n}";
    }

    public string Generate(string name, HashSet<string> names, List<Node> nodes)
    {
        var codeList = nodes.Select(a => a.Accept(this, null) + "\n").Select(a => a.ToString());
        var code = string.Join("\n", string.Concat(codeList).Split("\n").Where(a => !a.All(Char.IsWhiteSpace)));
        var boilerPlate = "using System;";
        boilerPlate += "\nusing System.Linq;";
        boilerPlate += "\nusing System.Collections.Generic;";
        boilerPlate += "\n" + string.Format("class {0}", name) + "{";
        boilerPlate += "\n\tstatic public string Execute(Dictionary<string, Value> _context = null){";
        boilerPlate += "\n\t\tif(_context == null) _context = new Dictionary<string, Value>();";
        boilerPlate += string.Format("\n\t\tif(new HashSet<string>{0}.Except(_context.Keys).Count() != 0) throw new ModelError(new HashSet<string>{0}, new HashSet<string>(_context.Keys));", "{" + string.Join(", ", names.Select(a => "\"" + a.ToString() + "\"")) + "}");
        boilerPlate += "\n\t\tvar __generatedString = \"\";";
        foreach(var str in names) boilerPlate += string.Format("\n\t\tvar {0} = _context[\"{0}\"];", str);
        boilerPlate += "\n" + Indent(Indent(code));
        boilerPlate += "\n\t\treturn __generatedString;";
        boilerPlate += "\n\t}";
        boilerPlate += "\n}";
        return boilerPlate;
    }
}