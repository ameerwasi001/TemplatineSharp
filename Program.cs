using System.Collections.Generic;

namespace TemplateSharp
{
    class Program
    {

        static void Main(string[] args)
        {
            var str = "";
            str += "The number is {{ {\"val\": cos(x*6+4.2*(2-3)) |>> sin |> pow(2)} }} and the book is written by {{index(book.author, \"firstName\")}} {{book.author.lastName}}, what's up?";
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
            str += "\n  {%- else %}";
            str += "\n  - I would kill for a meal with {{ingred}}";
            str += "\n  {%- endif -%}";
            str += "\n  {% endif -%}";
            str += "\n  {%- endfor -%}";
            str += "\n  {%- endfor -%}";

            var template = new TemplateBuilder().Build(str);

            // template.Compile("SimpleTemplate", string.Format("./GeneratedTemplates/SimpleTemplate.cs"));

            var executed = new SimpleTemplate().Execute(new Dictionary<string, Value>{
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
            });

            // var executed = template.Execute(new Dictionary<string, Value>{
            //     {"x", Value.Construct(11)},
            //     {"name", "Ameer"},
            //     {"book", new Dictionary<Value, Value>{
            //         {"length", 240},
            //         {"author", new Dictionary<Value, Value>{
            //             {"firstName", "Frank"},
            //             {"lastName", "Herbert"},
            //         }}
            //     }},
            //     {"meals", new List<Value>(){
            //         new List<Value>(){"Tuna", "Salmon"},
            //         new List<Value>(){"Chicken", "Broccoli"},
            //     }},
            //     {"sin", Value.Construct(arr => System.Math.Sin(arr[0]))},
            //     {"cos", Value.Construct(arr => System.Math.Cos(arr[0]))},
            //     {"pow", Value.Construct(arr => System.Math.Pow(arr[0], arr[1]))},
            //     {"index", Value.Construct(arr => arr[0][arr[1]])},
            // });

            System.Console.WriteLine(executed);
        }
    }
}
