using System;
using System.Collections.Generic;
using System.Linq;

public class FindExtensionReference
{
    public string Visit(List<Node> nodes)
    {
        foreach(var node in nodes) {
            if(node is ExtendsNode) return ((ExtendsNode)node).extension;
        }
        return null;
    }
}