using System;
using System.Linq;
using System.Collections.Generic;
class SimpleTemplate{
	public string Execute(Dictionary<string, Value> _context = null){
		if(_context == null) _context = new Dictionary<string, Value>();
		var __generatedList = new List<Value>();
		var pow = _context["pow"];
		var sin = _context["sin"];
		var cos = _context["cos"];
		var x = _context["x"];
		var index = _context["index"];
		var book = _context["book"];
		var name = _context["name"];
		var meals = _context["meals"];
		__generatedList.Add(Value.Construct("The number is "));
		__generatedList.Add(Value.Construct(new Dictionary<Value, Value>{{Value.Construct("val"), pow.Call(new List<Value>(){Value.Construct(2), sin.Call(new List<Value>(){cos.Call(new List<Value>(){((x * Value.Construct(6)) + (Value.Construct(4.2) * (Value.Construct(2) - Value.Construct(3))))})})})}}));
		__generatedList.Add(Value.Construct(" and the book is written by "));
		__generatedList.Add(index.Call(new List<Value>(){book["author"], Value.Construct("firstName")}));
		__generatedList.Add(Value.Construct(" "));
		__generatedList.Add(book["author"]["lastName"]);
		__generatedList.Add(Value.Construct(", what's up?\nWhere is "));
		__generatedList.Add((name + Value.Construct("'s shirt with print {{obj}}")));
		__generatedList.Add(Value.Construct("?\nI could go for any of the following\n  "));
		foreach(var _ls0 in meals.GetIterator()){
			var meal = _ls0.ElementAt(0);
			foreach(var _ls1 in meal.GetIterator()){
				var ingred = _ls1.ElementAt(0);
				if((x < Value.Construct(3))){
					__generatedList.Add(Value.Construct("\n  - I could go for a meal with "));
					__generatedList.Add(ingred);
				}else if((x < Value.Construct(5))){
					__generatedList.Add(Value.Construct("\n  - I would like to have a meal with "));
					__generatedList.Add(ingred);
				}else if((x < Value.Construct(10))){
					__generatedList.Add(Value.Construct("\n  - I would love a meal with "));
					__generatedList.Add(ingred);
				}else{
					if((name == Value.Construct("Ameer"))){
						__generatedList.Add(Value.Construct("\n  - Ameer would kill for a meal with "));
						__generatedList.Add(ingred);
					}else{
						__generatedList.Add(Value.Construct("\n  - I would kill for a meal with "));
						__generatedList.Add(ingred);
					}
				}
			}
		}
		
		return string.Concat(__generatedList);
	}
}