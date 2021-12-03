using System.Collections.Generic;
using System.Linq;

public class Template {
    private List<Node> nodes;

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
        var toks = new TemplateLexer(template).Lex();
        var renderNodes = new TemplateParser(toks).Parse();
        return new Template(renderNodes);
    }
}