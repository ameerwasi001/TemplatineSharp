# TemplatineSharp
TemplatineSharp is fast, simple, and secure templating engine that supports inheritance, whitespace control, filters, functions, loops, and complex conditionals out of the box. Here we are going to explain each of those concepts and how to interface with them from your favorite dotnet language.

## Building and running
In order to run a TemplatineSharp template, you must first build it. For single templates use
```
Template template = new TemplateBuilder().Build("fileName.ext", fileSrc)
```
and for templates wth complex relations use
```
TemplateSystem templates = new TemplateBuilder().Build(new Dictionary<string, string>{
    {"parent.txt", str},
    {"child.txt", childStr},
});
```
both of which can then either be ran by stating
```
var executed = templates["child.txt"].Execute(env);
```
for a TemplateSystem and
```
template.Execute(env)
```
for templates. One can also compile these templates to CSharp by stating the following
```
templates.Compile(new Dictionary<string, string>{
    {"parent.txt", "ParentTemplate"},
    {"child.txt", "ChildTemplate"},
});
```
for a TemplatingSystem where "ParentTemplate" is the name of the class it wll compile your "parent.txt" template to. When one uses a Template, they may write the following instead
```
template.Compile(src, path_to_compile_to)
```
The compiled templates can then be ran using an Execute function identical to the one that you may use to execute Templates.

## Loops
The first construct you should familiarize yourself with, is a for-loop in TemplatineSharp. Here's how you write a for-loop in this templating engine
```
{% for item in food %}
    {{item}}
{% endfor %}
```
which would, when given the input `new Dictionary<string, Value>{"food": new List<Value>(){"Chicken", "Broccoli", "Peanuts"}}` would output
```
    Chicken
    Broccoli
    Peanuts
```
One may use whitespace control to change spacing like such
```
{% for item in food -%}
    {{item}}
{% endfor %}
```
which would output
```
Chicken
Broccoli
Peanuts
```

## If-Statements
If statements are what you would expect them to be, this is their syntax
```
{% if x == 5 %}
Exact
{% elif x < 5 %}
Small
{% else %}
Big
{% endif %}
```
This would work like a traditional if statement and render depending of the value of x. Following are all the logical, relational, and arithmetic operators respectively. `&`, `|`, `>=`, `>`, `==`, `<`, `<=`, `!=`, `+`, `-`, `*`, and `/`.

## Pipes and Functions
Functions can be called using by using `f(a1, a1)` syntax as well as `a2 |> f(a1)`. Although if a function returns a function then one might want to consider the curry pipe for function calls lke `f(x)(y)` which may be expressed as `y |>> f(x)`.

## Containers
The available containers are list, and dictionary such as the following `[1, 2, 3, 4, 5]`, and `{"a": 4, "b": "m"}`. Dictionaries and lists can contain an item of any type but the keys of dictionaries must be, at least for now, primitves like strings, integers, and booleans.

## Blocks & Inheritance
Blocks are parts of the template that can be overridden via inheritance, so we will discuss the concepts together. Following is how you would define a block in `parent.txt`
```
Header
{% block container %}Default{% endblock %}
Footer
```
which one can inherit from by saying
```
{% extends \"parent.txt\" %}
Header
{% block container %}Overridden{% endblock %}
Footer
```
Then the parent would output
```
Header
Default
Footer
```
while the child outputs
```
Header
Overridden
Footer
```
You may also consider that inheritance is a compile-time operation and since has no effect on runtime performance and is therefore constrained to the limitations of static replacement.
