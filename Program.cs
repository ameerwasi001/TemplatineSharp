using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var template = new TemplateBuilder()
                .Build("The number is {{x+4.2*(2-3)}}, what's up?\nWhere is {{name + \"'s shirt with print {{obj}}\"}}?");
            var str = template.Execute(new Dictionary<string, Value>{
                {"x", Value.Construct(7)},
                {"name", Value.Construct("Ameer")}
            });
            System.Console.WriteLine(str);
        }
    }
}
