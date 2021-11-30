using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class Template {
    private List<Node> nodes;
    private Dictionary<string, Value> dict;

    public Template(List<Node> list)
    {
        nodes = list;
    }

    public string Execute(Dictionary<string, Value> model = null)
    {
        var interpreter = new Interpreter();
        var ctx = new Context(model == null ? new Dictionary<string, Value>() : model);
        return string.Concat(this.nodes.Select(a => a.Accept(interpreter, ctx)).Select(a => a.ToString()));
    }
}

class TemplateBuilder
{
   public Template Build(string template)
    {
        var splittenString = new TemplateLexer(template).Lex();
        var renderNodes = new List<Node>();
        var pos = new Position("<module>", template);
        pos.Advance();
        foreach(var str in splittenString) {
            var posStart = pos.Copy();
            if (str.StartsWith("{{"))
            {
                pos.Advance();
                pos.Advance();
                var subStr = str.Substring(2);
                var finalStr = subStr.Substring(0, subStr.Length-2);
                var toks = new Lexer("<module>", finalStr, pos).Lex();
                var node = new Parser(toks).Parse();
                pos.Advance();
                pos.Advance();
                var currentPos = pos.Copy();
                var renderNode = new RenderNode(node, posStart, currentPos);
                renderNodes.Add(renderNode);
            } else {
                foreach(var ch in str) pos.Advance(ch.ToString());
                var currentPos = pos.Copy();
                renderNodes.Add(new RenderNode(new StrNode(str, posStart, currentPos), posStart, currentPos));
            }
        }

        return new Template(renderNodes);
    }
}