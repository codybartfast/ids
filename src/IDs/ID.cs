using System.Numerics;

namespace Fmbm.Text;

public enum ZeroChar
{
    Auto = 0,
    True = 1,
    False = 2
}

public partial class ID : IComparer<string>
{
    internal static Dictionary<char, int> MakeDict(string chars)
    {
        var dict = new Dictionary<char, int>();
        for (var i = 0; i < chars.Length; i++)
        {
            dict[chars[i]] = i;
        }
        return dict;
    }

    internal static string NumToString(BigInteger num, string chars)
    {
        var id = new List<char>();
        while (num > 0)
        {
            int n = (int)(num % chars.Length);
            id.Add(chars[n]);
            num /= chars.Length;
        }
        id.Reverse();
        return new string(id.ToArray());
    }

    internal static BigInteger StringToNum(
        string id, string chars)
    {
        return StringToNum(id, MakeDict(chars));
    }

    internal static BigInteger StringToNum(
        string id, Dictionary<char, int> charDict)
    {
        BigInteger step = 1;
        BigInteger num = 0;
        for (int i = id.Length - 1; i >= 0; i--, step *= charDict.Count)
        {
            num += step * charDict[id[i]];
        }
        return num;
    }

    readonly string chars;
    readonly bool haveZeroChar;


    readonly List<int> indexes
    ;
    readonly object lockObj = new object();

    public ID(string last, string chars, ZeroChar zeroChar = ZeroChar.Auto)
    {
        // last only contains chars
        // at least two chars
        // zeroChar is char
        // last no leading zero
        this.chars = chars;
        this.haveZeroChar = zeroChar switch
        {
            ZeroChar.Auto => chars[0] == '0',
            ZeroChar.True => true,
            ZeroChar.False => false,
            _ => throw new Exception($"Unexpected ZeroChar value: {zeroChar}") // XXX
        };
        this.indexes = new List<int>(last.Select(c => chars.IndexOf(c)));
        this.Last = last;
        this.CharIndexDict = MakeDict(chars);
    }

    public string Last { get; private set; }
    public string Chars => chars;
    public ZeroChar ZeroChar => haveZeroChar ? ZeroChar.True : ZeroChar.False;
    Dictionary<char, int> CharIndexDict { get; }

    public string Next()
    {
        return Increment();
    }

    internal string Increment()
    {
        lock (lockObj)
        {
            for (var i = indexes
            .Count - 1; i >= 0; i--)
            {
                if (indexes
                [i] < chars.Length - 1)
                {
                    indexes[i] += 1;
                    return SetLastFromIndexes();
                }
                indexes[i] = 0;
            }
            indexes.Insert(0, haveZeroChar && indexes.Count > 0 ? 1 : 0);
            return SetLastFromIndexes();
        }
    }

    string SetLastFromIndexes()
    {

        var cs = indexes.Select(i =>
        {
            // Console.WriteLine(i);
            return chars[i];
        });
        Last = new string(cs.ToArray());
        return Last;
    }

    public int Compare(string? x, string? y)
    {
        return (x, y) switch
        {
            (null, null) => 0,
            (_, null) => 1,
            (null, _) => -1,
            (_, _) => CompareNonNull(x, y)
        };

        int CompareNonNull(string x, string y)
        {
            var lengthDiff = x.Length - y.Length;
            if (lengthDiff != 0)
            {
                if (!haveZeroChar)
                {
                    return lengthDiff;
                }
                var longer = lengthDiff > 0 ? x : y;
                for (var i = longer.Length - lengthDiff; i < longer.Length; i++)
                {
                    if (longer[i] != chars[0])
                    {
                        return lengthDiff;
                    }
                }
            }
            for (var i = Math.Min(x.Length, y.Length) - 1; i >= 0; i++)
            {
                if (x[i] != y[i])
                {
                    return CharIndexDict[x[i]] - CharIndexDict[y[i]];

                }
            }
            return 0;
        }
    }
}
