using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Template {
    private List<Node> nodes;
    private HashSet<string> requiredEnv;

    public Template(List<Node> list, HashSet<string> env)
    {
        nodes = list;
        requiredEnv = env;
    }

    public string Execute(Dictionary<string, Value> model = null)
    {
        var interpreter = new Interpreter();
        if(model == null) model = new Dictionary<string, Value>();
        var modelKeys = model.Keys;
        if(requiredEnv.Except(modelKeys).Count() != 0) throw new ModelError(requiredEnv, new HashSet<string>(modelKeys));
        var ctx = new Context(model);
        return string.Concat(this.nodes.Select(a => a.Accept(interpreter, ctx)).Select(a => a.ToString()));
    }

    public void Compile(string name, string path)
    {
        if(path == null) path = string.Format("./{0}.cs", name);
        var codeGenerator = new CSharpGenerator();
        var newNodes = new RenderAggregator().Visit(nodes);
        var code = codeGenerator.Generate(name, requiredEnv, newNodes);
        File.WriteAllText(path, code);
    }

    public string Compile(string name)
    {
        var codeGenerator = new CSharpGenerator();
        var newNodes = new RenderAggregator().Visit(nodes);
        return codeGenerator.Generate(name, requiredEnv, newNodes);
    }

    override public string ToString()
    {
        return string.Join("\n", this.nodes.Select(a => a.ToString()));
    }
}

class TemplateBuilder
{
   public Template Build(string template)
    {
        var toks = new TemplateLexer(template).Lex();
        var renderNodes = new TemplateParser(toks).Parse();
        var pipeEliminator = new PipeEliminator();
        var env = new HashSet<string>();
        var envGenerator = new EnvironmentGenerator();
        renderNodes = renderNodes.Select(a => a.Accept(pipeEliminator, true)).ToList();
        foreach(var node in renderNodes) node.Accept(envGenerator, env);
        return new Template(renderNodes, env);
    }
}