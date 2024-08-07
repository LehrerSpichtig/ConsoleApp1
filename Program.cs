// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq.Expressions;

ExpressionTreeCSharp a = new();

/*
The following program generates several binary algreaic expression trees from latex expressions.
*/
Console.WriteLine();
Console.WriteLine();

a.PrintTree(a.LatexToExpressiontree(@"\sqrt{2.42+3.43* 5}* {{(10.0504-2.72)}^{3}}"),0);

Console.WriteLine();
Console.WriteLine();

Console.WriteLine();
Console.WriteLine();
a.PrintTree(a.LatexToExpressiontree(@"\frac{2.0+2.1*3.21}{4.1+5}+{{{3.5}^{5.6}}}^{2}}"),0);
Console.WriteLine();
Console.WriteLine();
a.PrintTree(a.LatexToExpressiontree(@"\frac{2.0+2.1*3.21}{4.1+5}+3.5*2"),0);
Console.WriteLine();
Console.WriteLine();
a.PrintTree2(a.LatexToExpressiontree(@"\frac{2+3*4}{6+7}+2*3"));
Console.WriteLine();
Console.WriteLine();
// the following example illustrates that the step in the method tokenize (i.e. first translation of the latex expression)
// could be evaded.
a.PrintTree(a.LatexToExpressiontree("(2 + 3 * 4) / (6 + 7) + 2 ^ 3"),0);
Console.WriteLine();
Console.WriteLine();
a.PrintTree(a.LatexToExpressiontree("3*{{2}^{4}}"),0);
Console.WriteLine();
Console.WriteLine();
Console.WriteLine(a.measuredTerm(a.LatexToExpressiontree("3*{{2}^{4}}")));
Console.WriteLine();
Console.WriteLine();
Console.WriteLine(a.measuredTerm(a.LatexToExpressiontree(@"\frac{2+3*4}{6+7}+2*3")));
Console.WriteLine();
Console.WriteLine();

string stringData = "2.3 3 + 2 3 4 * 3 4 4 / 1 6 3 + 2 7 3 + 0 2 2 * 1 3 2";
TermGenerator b = new TermGenerator();
b.FirstLoop(stringData);
b.SingleLoop();
b.FirstLoop(a.measuredTerm(a.LatexToExpressiontree(@"\frac{2.0+2.1*3.21}{4.1+5}+{{{3.5}^{5.6}}}^{2}}")));
b.SingleLoop();
stringData = "10 1 + 0 2 2 * 1 3 2 + 0 3 1";
b.FirstLoop(stringData);
b.SingleLoop();
stringData = "10 1 + 0 2 1";
b.FirstLoop(stringData);
b.SingleLoop();

    
    

