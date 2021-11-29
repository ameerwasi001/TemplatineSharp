using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

public class Template {
    private List<Node> nodes;

    public Template(List<Node> list)
    {
        nodes = list;
    }

    public string Execute()
    {
        var interpreter = new Interpreter();
        return string.Concat(this.nodes.Select(a => a.Accept(interpreter, new Context(new SymbolTable(), null))).Select(a => a.ToString()));
    }
}

class TemplateBuilder
{
    public static string REGEX = @"(?s)({{.*?}}|{%.*?%}|{#.*?#})";

    public Template Build(string template)
    {
        var splittenString = Regex.Split(template, REGEX);
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