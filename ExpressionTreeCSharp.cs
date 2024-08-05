using System;
using System.Text.RegularExpressions;

/*
The classe ExpressionTreeCSharp is adopted from a python program proposed by chatGPT.

The class contains methods that in combination can transform a latex-term into an binary algebraic expression tree.
Central is the transformation from infix notation to postfix notation (reverse Polish notation, method InfixToPostfix) using the Shunting Yard algorithm.
The postfix notation allow the generation of the binary algreaic expression tree.

Limitations: 
- The scope of usable latex-terms is limited: Allowed are only fractions, square roots, exponentiation and the basic operations (addition,
substraction, multiplication and division). The current version deviates from standard latex-Notation in that it uses * instead of \cdot
as the multiplication symbol. 
- Variables are not yet handled (but that is a quick fix).
- When handling variables is included then the following should be implemented:
     xy => x * y
	 3x => 3 * x
- The method Tokenize transforms the latex-term into a more general form according to the following example:
\frac{2+3*4}{6+7}+2*3  => (2 + 3 * 4) / (6 + 7) + 2 * 3
Elements of latex-notation like \frac{}{} and \sqrt{} are detected using regex. This approach works fine with simpler terms but might fail 
with complex terms like nested fractions. The following regex-expressions illustrate the limitation of this approach: 
\\frac{[^{}]+}{[^{}]+}
\\frac{.*}{.*}
The second regex-expression is the greedy version while the first expression is the lazy version (source: realpython.com/regex-python). Both versions are expected to fail by essentially grasping the wrong denominator in nested fractions.
A solution is to circumvent latex-notation by writing the terms directly in the form illustrated by (2 + 3 * 4) / (6 + 7) + 2 * 3 into the method LatexToExpressionTree
- The methods are not yet save with regards to falsely written latex-terms


*/
//Sources
// https://itdranik.com/en/math-expressions-shunting-yard-algorithm-en/
// https://learn.microsoft.com/en-us/dotnet/csharp/tour-of-csharp/tutorials/list-collection?tutorial-step=4
// https://www.tutorialsteacher.com/regex/regex-in-csharp
// 
public class ExpressionTreeCSharp
{

		
	
	
	/* Class containing left and right
	child of current node and key value*/
	//source: https://debug.to/3253/tree-in-data-structure-using-c
	
	public class Node<T>
	{
		public T key;
		public Node<T> left, right;

		public Node(T item)
		{
		key = item;
		left = null;//new("");
		right = null;//new("");
		}
	}
	
	// transforms the the latex expression into a more general mathematical notation
    // e.g. \frac{2+3*4}{6+7}+2*3  => (2 + 3 * 4) / (6 + 7) + 2 * 3   
	private static List<string> Tokenize(string latex_expr)
	{
		string regexPattern = @"\\frac{[^{}]+}{[^{}]+}|\\sqrt{[^{}]+}|{{.*}\^{.*}}|\\[a-zA-Z]+|[0-9]+(\.[0-9]+)?|[+\-*/^(){}]";//@"\\frac{[^{}]+}{[^{}]+}|\\sqrt{[^{}]+}|{{[^{}]+}\^{[^{}]+}}|\\[a-zA-Z]+|[0-9]+(\.[0-9]+)?|[+\-*/^(){}]";
		Regex regex = new(regexPattern);
		
		Regex regexHelp = new(@"{{.*}\^{[^{}]+}");//new(@"{{[^{}]+}\^{[^{}]+}}");
		//string inputString //@"\sqrt{2.4+3 * 5.412} * {(10.1-x)}^{3.32}";
		
		MatchCollection tokens = regex.Matches(latex_expr);
		var processed_tokens = new List<string> {};
		
 		foreach (Match match in tokens)
		{
			string token = match.ToString();
			//Console.WriteLine(token);
			
		
			if (token.StartsWith("\\frac") ){
				processed_tokens.Add("(");
				//ACHTUNG BEI SQRT IST ES 6 und HIER IST ES 5
				var numerator = Tokenize(token.Substring(6,token.IndexOf("}")-6));
				processed_tokens.AddRange(numerator);
				processed_tokens.Add(")");
				processed_tokens.Add("/");
				processed_tokens.Add("(");
				var denominator = Tokenize(token.Substring(token.IndexOf("}")+2,token.Length-token.IndexOf("}")-3));
				processed_tokens.AddRange(denominator);
				
				processed_tokens.Add(")");
			
			}
			else if (token.StartsWith("\\sqrt")){
				processed_tokens.Add("(");
				
				var inner_expr = Tokenize(token.Substring(6,token.IndexOf("}")-6));
				processed_tokens.AddRange(inner_expr);
				
				processed_tokens.Add(")");
				processed_tokens.Add("^");
				processed_tokens.Add("0.5");
				
			}
			else if (regexHelp.IsMatch(token)){
				processed_tokens.Add("(");


				string ie = token.Substring(2,token.LastIndexOf("}^{")-2);
				if (ie.Contains("}^{")){
					ie = "{"+ie+"}";
					
				}

				var inner_expr = Tokenize(ie);

				processed_tokens.AddRange(inner_expr);

				processed_tokens.Add(")");
				processed_tokens.Add("^");
				string exponent = token.Substring(token.LastIndexOf("}^{")+3);
				string exponentClear = exponent.Remove(exponent.IndexOf("}"));
				processed_tokens.Add(exponentClear);
			}
			else {
				processed_tokens.Add(token);
			}
		} 
		return processed_tokens;
	}
	

	private static int Precedence(string op){
		if (op == "+" || op == "-") return 1;
		if (op == "*" || op == "/") return 2;
		if (op == "^") return 3;
		return 0;
	}

	// transforms the infix notation into a postfix notation (reverse Polish notation) by means of the shunting yard algorithm
    private List<string> InfixToPostfix(List<string> tokens){
		List<string> stack = new List<string>();
		List<string> postfix = new List<string>();

		Regex regex = new Regex(@"^\d+(\.\d+)?$");

		foreach (string token in tokens){
			if (regex.IsMatch(token)){// ATTENTION: Variables are not included yet
				postfix.Add(token);
			}
			else if (token.Equals("(")){
				stack.Add(token);
			}
			else if (token.Equals(")")){
				while (stack.Count > 0 && !stack[stack.Count-1].Equals("(")){
					postfix.Add(stack[stack.Count-1]);
					stack.RemoveAt(stack.Count-1);
					}
				if (stack.Count > 0 && stack[stack.Count-1].Equals("(")){
					stack.RemoveAt(stack.Count-1);
				}
			}
			else {
				while (stack.Count > 0 && Precedence(stack[stack.Count-1])>= Precedence(token)){
					postfix.Add(stack[stack.Count-1]);
					stack.RemoveAt(stack.Count-1);
				}
				stack.Add(token);
			}
		}
		while (stack.Count > 0){
			postfix.Add(stack[stack.Count-1]);
			stack.RemoveAt(stack.Count-1);
		}
		return postfix;
		}

		
	//Constrution of the binary algebraic tree from the postfix notation
	private Node<string> ConstructTree(List<string> postfix_tokens){
		List<Node<string>> stack = new(); // List<Node<string>>();
		Regex regex = new Regex(@"^\d+(\.\d+)?$");
		foreach (string token in postfix_tokens){
			if (regex.IsMatch(token)){
				stack.Add(new Node<string>(token));
			}
			else {
				if (stack.Count < 2){
					Console.WriteLine("Mathematisch inkorrekte Eingabe! Noch sicherstellen");
					return stack[stack.Count-1];
				}
				Node<string> r = stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				Node<string> l = stack[stack.Count-1];
				stack.RemoveAt(stack.Count-1);
				Node<string> node = new Node<string>(token);
				node.left = l;
				node.right = r;
				stack.Add(node);
			}
		}
		if (!(stack.Count == 1)){
			Console.WriteLine("Unbalanced expression");
			return stack[0];
		}

		return stack[stack.Count-1];
	}

	// prints the algebraic tree onto the console
	public void PrintTree<T>(Node<T> node, int level){
		if (!(node == null)){
			PrintTree(node.right, level + 1);
			string shift = "    ";
			for (int i = 0;i<level;i++) shift += "    ";
			shift += "-> " + node.key.ToString();
			Console.WriteLine(shift);
			PrintTree(node.left, level + 1);
		}
	}
	private string s = "";

	public string measuredTerm<T>(Node<T> node){
		this.s = "";
		LevelTree(node,0);
		string g = s;
		return g.Trim();
	}


	private void LevelTree<T>(Node<T> node, int level){
		if (!(node == null)){
			LevelTree(node.left, level + 1);
			s += "" + node.key.ToString() + " " + level+ " ";
			LevelTree(node.right, level + 1);
		}
		
	}
	// an alternative display of the tree (inferior display)
	public void PrintTree2(Node<string> node){
		if (!(node == null)){
			Console.WriteLine(node.key);
			if (node.left != null){
				Console.WriteLine(node.left.key+" left of "+node.key);
				PrintTree2(node.left);
			}
			if (node.right != null){
				Console.WriteLine(node.right.key+" right of "+node.key);
				PrintTree2(node.right);
			}
		}
	}

	// Generates the tree from the latex expression of a term
	public Node<string> LatexToExpressiontree(string latexExpr){
		List<string> tokens = Tokenize(latexExpr);
		List<string> infixToPostfix = InfixToPostfix(tokens);
		string rp = "";
		foreach (string token in infixToPostfix){
			rp += token + " ";
		}
		Console.WriteLine(rp);
		Node<string> tree = ConstructTree(infixToPostfix);
		return tree;
	}

	
}