using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var str = "";
            str += "The number is {{x+4.2*(2-3)}}, what's up?";
            str += "\nWhere is {{name + \"'s shirt with print {{obj}}\"}}?";
            str += "\nI could go for any of the following";
            str += "\n{% for meal in meals %}";
            str += "\n  {% for ingred in meal %}";
            str += "\n  - I could go for a meal with {{ingred}}";
            str += "\n  {% endfor %}";
            str += "\n{% endfor %}";

            var template = new TemplateBuilder().Build(str);
            var executed = template.Execute(new Dictionary<string, Value>{
                {"x", Value.Construct(7)},
                {"name", Value.Construct("Ameer")},
                {"meals", Value.Construct(new List<Value>(){
                    Value.Construct(new List<Value>(){Value.Construct("Tuna"), Value.Construct("Salmon")}),
                    Value.Construct(new List<Value>(){Value.Construct("Chicken"), Value.Construct("Crab")}),
                })},
            });
            System.Console.WriteLine(executed);
        }
    }
}
