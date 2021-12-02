using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var str = "";
            str += "The number is {{x+4.2*(2-3)}}, what's up?\nWhere is {{name + \"'s shirt with print {{obj}}\"}}?";
            // str += "I could go for any of the following";
            // str += "\n{% for meal in meals %}";
            // str += "\n  I could go for a {{meal}}";
            // str += "\n{% endfor %}";

            var template = new TemplateBuilder().Build(str);
            var executed = template.Execute(new Dictionary<string, Value>{
                {"x", Value.Construct(7)},
                {"name", Value.Construct("Ameer")}
            });
            System.Console.WriteLine(executed);
        }
    }
}
