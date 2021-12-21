using System;
using System.Linq;
using System.Collections.Generic;
class SimpleTemplate{
	public string Execute(Dictionary<string, Value> _context = null){
		if(_context == null) _context = new Dictionary<string, Value>();
		if(new HashSet<string>{"pow", "sin", "cos", "x", "index", "book", "name", "meals"}.Except(_context.Keys).Count() != 0) throw new ModelError(new HashSet<string>{"pow", "sin", "cos", "x", "index", "book", "name", "meals"}, new HashSet<string>(_context.Keys));
		var __generatedString = "";
		var pow = _context["pow"];
		var sin = _context["sin"];
		var cos = _context["cos"];
		var x = _context["x"];
		var index = _context["index"];
		var book = _context["book"];
		var name = _context["name"];
		var meals = _context["meals"];
		__generatedString += Value.Construct("The number is ").ToString() + Value.Construct(new Dictionary<Value, Value>{{Value.Construct("val"), pow.Call(new List<Value>(){Value.Construct(2), sin.Call(new List<Value>(){cos.Call(new List<Value>(){((x * Value.Construct(6)) + (Value.Construct(4.2) * (Value.Construct(2) - Value.Construct(3))))})})})}}).ToString() + Value.Construct(" and the book is written by ").ToString() + index.Call(new List<Value>(){book["author"], Value.Construct("firstName")}).ToString() + Value.Construct(" ").ToString() + book["author"]["lastName"].ToString() + Value.Construct(", what's up?\nWhere is ").ToString() + (name + Value.Construct("'s shirt with print {{obj}}")).ToString() + Value.Construct("?\nI could go for any of the following\n  ").ToString();
		foreach(var _ls0 in meals.GetIterator()){
			var meal = _ls0.ElementAt(0);
			foreach(var _ls1 in meal.GetIterator()){
				var ingred = _ls1.ElementAt(0);
				if((x < Value.Construct(3))){
					__generatedString += Value.Construct("\n  - I could go for a meal with ").ToString() + ingred.ToString();
				}else if((x < Value.Construct(5))){
					__generatedString += Value.Construct("\n  - I would like to have a meal with ").ToString() + ingred.ToString();
				}else if((x < Value.Construct(10))){
					__generatedString += Value.Construct("\n  - I would love a meal with ").ToString() + ingred.ToString();
				}else{
					if((name == Value.Construct("Ameer"))){
						__generatedString += Value.Construct("\n  - Ameer would kill for a meal with ").ToString() + ingred.ToString();
					}else{
						__generatedString += Value.Construct("\n  - I would kill for a meal with ").ToString() + ingred.ToString();
					}
				}
			}
		}
		return __generatedString;
	}
}