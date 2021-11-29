using System;
using System.Text;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var template = new TemplateBuilder().Build("Hello {{1+4.2*(2-3)}}, what's up?\nHow are you {{9/3}}?");
            var str = template.Execute();
            System.Console.WriteLine(str);
        }
    }
}
