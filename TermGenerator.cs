

//using System.Runtime.CompilerServices;

public class TermGenerator
{
    private string stringData = "3 1 * 0 2 2 ^ 1 4 2";
    private string[] symbols;
    private int[] levels;
    private double[] floatValues;
    int maxLevel;
    private int currentLevel;


    private void Generate(string stringData)
    {
        this.stringData = stringData.Replace('.', ',');

        string[] stringDataArray = this.stringData.Split(' ');
        this.symbols = new string[stringDataArray.Length / 2];
        this.levels = new int[stringDataArray.Length / 2];
        this.floatValues = new double[stringDataArray.Length / 2];

        int j = 0;
        this.maxLevel = 0;
        for (int i = 0; i < stringDataArray.Length; i += 2)
        {
            symbols[j] = stringDataArray[i];
            int.TryParse(stringDataArray[i + 1], out int tryParsedInt);
            levels[j] = tryParsedInt;
            if (levels[j] > maxLevel) maxLevel = levels[j];
            j++;
        }
        for (int i = 0; i < symbols.Length; i += 2)
        {
            double.TryParse(symbols[i], out double floatValue);
            floatValues[i] = floatValue;
            if (i + 1 < symbols.Length) floatValues[i + 1] = double.MinValue;
        }

        this.currentLevel = maxLevel;


    }

    private void UpdateTerm()
    {//string[] symbols, int[] levels, float[] floatValues, int currentLevel){

        List<string> Symbols = symbols.ToList<string>();
        List<int> Levels = levels.ToList<int>();
        List<double> FloatValues = floatValues.ToList<double>();



        int i = 0;
        List<int> Update = [];
        bool oneHandled = false;
        List<string> strich = ["+", "-"];
        List<string> punkt = ["*", "/"];
        while (i < Levels.Count & !oneHandled)
        {
            if (Levels[i] == currentLevel)
            {
                Update.Add(i);
                switch (Symbols[i + 1])
                {
                    case "+":
                        FloatValues[i] = FloatValues[i] + FloatValues[i + 2];
                        oneHandled = true;
                        break;
                    case "-":
                        FloatValues[i] = FloatValues[i] - FloatValues[i + 2];
                        oneHandled = true;
                        break;
                    case "*":
                        FloatValues[i] = FloatValues[i] * FloatValues[i + 2];
                        oneHandled = true;
                        break;
                    case "/":
                        FloatValues[i] = FloatValues[i] / FloatValues[i + 2];
                        oneHandled = true;
                        break;
                    case "^":
                        FloatValues[i] = Math.Pow(FloatValues[i], FloatValues[i + 2]);
                        oneHandled = true;
                        break;
                }
                //FloatValues[i] = Math.Round(FloatValues[i], 1);
                Symbols[i] = Math.Round(FloatValues[i], 1).ToString();
                //Levels[i] = Levels[i]-1;            
                i += 3;

            }
            else
            {
                i++;
            }

        }

        for (int j = Update.Count - 1; j >= 0; j--)
        {
            string operation = Symbols[Update[j] + 1];
            Symbols.RemoveAt(Update[j] + 2);
            Symbols.RemoveAt(Update[j] + 1);
            FloatValues.RemoveAt(Update[j] + 2);
            FloatValues.RemoveAt(Update[j] + 1);
            Levels[Update[j]] -= 1;
            Levels.RemoveAt(Update[j] + 2);
            Levels.RemoveAt(Update[j] + 1);
            if (strich.Contains(operation) && Update[j] + 1 < Symbols.Count && strich.Contains(Symbols[Update[j] + 1]) && Levels[Update[j] + 1] == Levels[Update[j]])
            {
                Levels[Update[j]] += 1;
            }
            if (punkt.Contains(operation) && Update[j] + 1 < Symbols.Count && punkt.Contains(Symbols[Update[j] + 1]) && Levels[Update[j] + 1] == Levels[Update[j]])
            {
                Levels[Update[j]] += 1;
            }

        }

        //UNBEDINGT "this" anfügen!!
        if (!Levels.Contains(currentLevel))
        {
            this.currentLevel -= 1;
        }

        this.symbols = Symbols.ToArray();
        this.floatValues = FloatValues.ToArray();
        this.levels = Levels.ToArray();

    }

    public List<int> PrimFaktorzerlegung(int n)
    {
        List<int> tM = new();

        if (n == 1) return tM;
        int t = 2;
        while (t * t <= n)
        {
            while (n % t == 0)
            {
                tM.Add(t);
                n /= t;
            }
            t++;
        }
        tM.Add(n);
        return tM;
    }

    public List<int> TeilerMenge(int n){
        List<int> tM = new();

        if (n == 1) return tM;
        int t = 2;
        while (t * t <= n)
        {
            if (n % t == 0)
            {
                tM.Add(t);
                tM.Add((n/t));
            }
            t++;
        }
        //tM.Add(n);
        tM.Sort();
        
        return tM;
    }

    public void FirstLoop(string stringData)
    {
        Generate(stringData);
        PrintData();
        while (this.currentLevel > 0)
        {
            UpdateTerm();
            PrintData();
        }
        Console.WriteLine();
    }

    public void SingleLoop()
    {
        Generate(stringData);
        PrintData();
        while (this.currentLevel > 0)
        {
            UpdateTerm();
            PrintData();
        }
        Console.WriteLine();
    }

    private void PrintData()
    {
        foreach (string symbol in symbols) Console.Write(symbol + " "); Console.WriteLine();
        foreach (int level in levels) Console.Write(level + " "); Console.WriteLine();
        foreach (double floatValue in floatValues)
        {
            if (floatValue > double.MinValue)
            {
                Console.Write(floatValue + " ");
            }
            else
            {
                Console.Write("a ");
            }
        }
        Console.WriteLine();


    }

}