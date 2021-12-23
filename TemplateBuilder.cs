using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public class Template {
    private List<Node> nodes;
    public Dictionary<string, Template> templates = new Dictionary<string, Template>();
    private HashSet<string> requiredEnv;
    private Dictionary<string, List<Node>> blockArgs;
    public string extension;

    public Template(List<Node> list, HashSet<string> env, string ext, Dictionary<string, List<Node>> args)
    {
        nodes = list;
        requiredEnv = env;
        blockArgs = args;
        extension = ext;
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
        parent.extension = extension;
        return parent;
    }

    public Template Extends()
    {
        if(extension == null) return this;
        else if(!(templates.ContainsKey(extension))) throw new System.Exception();
        else return this.Extends(templates[extension].Extends());
    }

    public Template Copy()
    {
        return new Template(
            this.nodes.Select(a => a.Copy()).ToList(),
            this.requiredEnv.Select(a => a).ToHashSet(),
            this.extension,
            this.blockArgs.Select(kv => Tuple.Create(kv.Key, kv.Value.Select(a => a.Copy()).ToList())).ToDictionary(ab => ab.Item1, ab => ab.Item2)
        );
    }

    public void SetEnv(Dictionary<string, Template> env)
    {
        templates = env;
    }

    override public string ToString()
    {
        return string.Join("\n", this.nodes.Select(a => a.ToString()));
    }
}

class TemplateSystem
{
    private Dictionary<string, Template> system;
    virtual public Template this[string index] => system[index];

    public TemplateSystem(Dictionary<string, Template> dict)
    {
        system = dict;
        CycleDetect(system);
    }

    public void Compile(Dictionary<string, string> mapping)
    {
        foreach(var (key, template) in system) 
        {
            if(!(mapping.ContainsKey(key))) throw new Exception();
            var name = mapping[key];
            template.Compile(name, string.Format("./GeneratedTemplates/{0}.cs", name));
        }
    }

    static public void CycleDetect(Dictionary<string, Template> system)
    {
        var extensionGraph = system
            .Select(kv => Tuple.Create(kv.Key, kv.Value.extension))
            .ToDictionary(ab => ab.Item1, ab => ab.Item2);
        foreach(var (_, v) in extensionGraph)
        {
            if(v != null) CycleDetect(v, extensionGraph, new HashSet<string>());
        }
    }

    static public void CycleDetect(string point, Dictionary<string, string> extensionGraph, HashSet<string> visiting)
    {
        var newPoint = extensionGraph[point];
        if(newPoint == null) return;
        if(visiting.Contains(newPoint)) throw new System.Exception(string.Format("{0} is in a cycle", newPoint));
        visiting.Add(newPoint);
        CycleDetect(newPoint, extensionGraph, visiting);
        visiting.Remove(newPoint);
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
        var extension = ExtensionHelpers.Find(renderNodes);
        renderNodes = ExtensionHelpers.Eliminate(renderNodes);
        return new Template(renderNodes, env, extension, blockArgs);
    }

    public TemplateSystem Build(Dictionary<string, string> dict)
    {
        var templateDict = dict
            .Select(kv => Tuple.Create(kv.Key, Build(kv.Value)))
            .ToDictionary(ab => ab.Item1, ab => ab.Item2);
        foreach(var (_, v) in templateDict) v.SetEnv(templateDict);
        TemplateSystem.CycleDetect(templateDict);
        templateDict = templateDict
            .Select(kv => Tuple.Create(kv.Key, kv.Value.Extends()))
            .ToDictionary(ab => ab.Item1, ab => ab.Item2);
        return new TemplateSystem(templateDict);
    }
}