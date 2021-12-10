using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var str = "";
            str += "The number is {{ {\"val\": sin(cos(x*6+4.2*(2-3)))} }} and the book is written by {{book.author.firstName}} {{book.author.lastName}}, what's up?";
            str += "\nWhere is {{name + \"'s shirt with print {{obj}}\"}}?";
            str += "\nI could go for any of the following";
            str += "\n{% for meal in meals %}";
            str += "\n  {% for ingred in meal %}";
            str += "\n  {% if x < 3 %}";
            str += "\n  - I could go for a meal with {{ingred}}";
            str += "\n  {% elif x < 5 %}";
            str += "\n  - I would like to have a meal with {{ingred}}";
            str += "\n  {% elif x < 10 %}";
            str += "\n  - I would love a meal with {{ingred}}";
            str += "\n  {% else %}";
            str += "  {% if name == \"Ameer\" %}";
            str += "  - Ameer would kill for a meal with {{ingred}}";
            str += "  {% else %}";
            str += "  - I would kill for a meal with {{ingred}}";
            str += "  {% endif %}";
            str += "\n  {% endif %}";
            str += "\n  {% endfor %}";
            str += "\n{% endfor %}";

            var template = new TemplateBuilder().Build(str);
            var executed = template.Execute(new Dictionary<string, Value>{
                {"x", Value.Construct(11)},
                {"name", "Ameer"},
                {"book", Value.Construct(new Dictionary<Value, Value>{
                    {"length", 240},
                    {"author", Value.Construct(new Dictionary<Value, Value>{
                        {"firstName", "Frank"},
                        {"lastName", "Herbert"},
                    })}
                })},
                {"meals", Value.Construct(new List<Value>(){
                    Value.Construct(new List<Value>(){"Tuna", "Salmon"}),
                    Value.Construct(new List<Value>(){"Chicken", "Broccoli"}),
                })},
                {"sin", Value.Construct(arr => System.Math.Sin(arr[0]))},
                {"cos", Value.Construct(arr => System.Math.Cos(arr[0]))}
            });
            System.Console.WriteLine(executed);
        }
    }
}
