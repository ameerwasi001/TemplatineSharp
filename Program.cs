using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {
        static void Main(string[] args)
        {
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
            var files = new List<string>{
                "parent.tempSh",
                "child.tempSh",
            };
            var templates = new TemplateBuilder().Build(files);
            templates.Compile(new List<string>{
                "parent.tempSh",
                "child.tempSh",
            });
            var executed = ChildTemplate.Execute(env);
            System.Console.WriteLine(executed);
        }
    }
}
