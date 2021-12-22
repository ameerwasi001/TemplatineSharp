using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            var str = "";
            str += "Title: {% block title %}Ingredients{% endblock %}";
            str += "\nThe number is {{ {\"val\": cos(x*6+4.2*(2-3)) |>> sin |> pow(2)} }} and the book is written by {{index(book.author, \"firstName\")}} {{book.author.lastName}}, what's up?";
            str += "\nWhere is {{name + \"'s shirt with print {{obj}}\"}}?";
            str += "\nI could go for any of the following";
            str += "\n  {% for meal in meals %}";
            str += "\n  {%- for ingred in meal -%}";
            str += "\n  {%- if x < 3 %}";
            str += "\n  - I could go for a meal with {{ingred}}";
            str += "\n  {%- elif x < 5 %}";
            str += "\n  - I would like to have a meal with {{ingred}}";
            str += "\n  {%- elif x < 10 %}";
            str += "\n  - I would love a meal with {{ingred}}";
            str += "\n  {%- else %}";
            str += "\n  {%- if name == \"Ameer\" %}";
            str += "\n  - Ameer would kill for a meal with {{ingred}}";
            str += "\n  {%- else -%}";
            str += "\n  {%- block outOfRange %}";
            str += "\n  - I would kill for a meal with {{ingred}}";
            str += "\n  {%- endblock -%}";
            str += "\n  {%- endif -%}";
            str += "\n  {% endif -%}";
            str += "\n  {%- endfor -%}";
            str += "\n  {%- endfor -%}";

            var childStr = "";
            childStr += "\n{% extends \"parent.txt\" %}";
            childStr += "\n{%- block title %}Meal for {{name}}{% endblock %}";
            childStr += "\n{%- block outOfRange %}";
            childStr += "\n  - I would like for Ameer to have a meal";
            childStr += "\n{%- endblock -%}";

            var env = new Dictionary<string, Value>{
                {"x", Value.Construct(11)},
                {"name", "Ameer"},
                {"book", new Dictionary<Value, Value>{
                    {"length", 240},
                    {"author", new Dictionary<Value, Value>{
                        {"firstName", "Frank"},
                        {"lastName", "Herbert"},
                    }}
                }},
                {"meals", new List<Value>(){
                    new List<Value>(){"Tuna", "Salmon"},
                    new List<Value>(){"Chicken", "Broccoli"},
                }},
                {"sin", Value.Construct(arr => System.Math.Sin(arr[0]))},
                {"cos", Value.Construct(arr => System.Math.Cos(arr[0]))},
                {"pow", Value.Construct(arr => System.Math.Pow(arr[0], arr[1]))},
                {"index", Value.Construct(arr => arr[0][arr[1]])},
            };

            var templates = new TemplateBuilder().Build(new Dictionary<string, string>{
                {"parent.txt", str},
                {"child.txt", childStr},
            });
            templates.Compile(new Dictionary<string, string>{
                {"parent.txt", "ParentTemplate"},
                {"child.txt", "ChildTemplate"},
            });
            var executed = ChildTemplate.Execute(env);
            // var executed = templates["child.txt"].Execute(env);
            System.Console.WriteLine(executed);
        }
    }
}
