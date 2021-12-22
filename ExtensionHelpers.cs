using System;
using System.Collections.Generic;
using System.Linq;

public class ExtensionHelpers
{
    static public string Find(List<Node> nodes)
    {
        foreach(var node in nodes) {
            if(node is ExtendsNode) return ((ExtendsNode)node).extension;
        }
        return null;
    }

    static public List<Node> Eliminate(List<Node> nodes)
    {
        return nodes.Where(n => !(n is ExtendsNode)).ToList();
    }
}