using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var template = new TemplateBuilder()
                .Build("The number is {{x+4.2*(2-3)}}, what's up?\nHow are you {{name}}?");
            var str = template.Execute(new Dictionary<string, Value>{
                {"x", Number.Construct(7)},
                {"name", Str.Construct("Ameer")}
            });
            System.Console.WriteLine(str);
        }
    }
}
