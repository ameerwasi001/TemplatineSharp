using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Template {
    private List<Node> nodes;
    private HashSet<string> requiredEnv;
    private Dictionary<string, List<Node>> blockArgs;

    public Template(List<Node> list, HashSet<string> env, Dictionary<string, List<Node>> args)
    {
        nodes = list;
        requiredEnv = env;
        blockArgs = args;
    }

    public string Execute(Dictionary<string, Value> model = null)
    {
        var interpreter = new Interpreter();
        if(model == null) model = new Dictionary<string, Value>();
        var modelKeys = model.Keys;
        if(requiredEnv.Except(modelKeys).Count() != 0) throw new ModelError(requiredEnv, new HashSet<string>(modelKeys));
        var ctx = new Context(model);
        var newNodes = new PatchBlocks().Visit(nodes, blockArgs);
        return string.Concat(newNodes.Select(a => a.Accept(interpreter, ctx)).Select(a => a.ToString()));
    }

    public void Compile(string name, string path)
    {
        if(path == null) path = string.Format("./{0}.cs", name);
        var codeGenerator = new CSharpGenerator();
        var newNodes = new PatchBlocks().Visit(nodes, blockArgs);
        newNodes = new RenderAggregator().Visit(newNodes);
        var code = codeGenerator.Generate(name, requiredEnv, newNodes);
        File.WriteAllText(path, code);
    }

    public string Compile(string name)
    {
        var codeGenerator = new CSharpGenerator();
        var newNodes = new PatchBlocks().Visit(nodes, blockArgs);
        newNodes = new RenderAggregator().Visit(newNodes);
        return codeGenerator.Generate(name, requiredEnv, newNodes);
    }

    public Template Extends(Template givenTemplate)
    {
        var parent = givenTemplate.Copy();
        foreach(var (k, v) in blockArgs) parent.blockArgs[k] = v;
        parent.nodes = new PatchBlocks().Visit(parent.nodes, parent.blockArgs);
        var env = new HashSet<string>();
        var envGenerator = new EnvironmentGenerator();
        foreach(var node in parent.nodes) node.Accept(envGenerator, env);
        parent.requiredEnv = env;
        return parent;
    }

    public Template Copy()
    {
        return new Template(
            this.nodes.Select(a => a.Copy()).ToList(),
            this.requiredEnv.Select(a => a).ToHashSet(),
            this.blockArgs.Select(kv => Tuple.Create(kv.Key, kv.Value.Select(a => a.Copy()).ToList())).ToDictionary(ab => ab.Item1, ab => ab.Item2)
        );
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
        var blockArgs = new CollectBlocks(false).Visit(renderNodes).Item2;
        var pipeEliminator = new PipeEliminator();
        var env = new HashSet<string>();
        var envGenerator = new EnvironmentGenerator();
        foreach(var node in renderNodes) node.Accept(envGenerator, env);
        renderNodes = renderNodes.Select(a => a.Accept(pipeEliminator, true)).ToList();
        return new Template(renderNodes, env, blockArgs);
    }
}