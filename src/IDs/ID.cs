using System.Diagnostics.CodeAnalysis;
using System.Numerics;

namespace Fmbm.Text;

public enum HasZero
{
    Auto = 0,
    True = 1,
    False = 2
}

public partial class ID : IComparer<string>, IEqualityComparer<string>
{
    static Dictionary<char, int> MakeDict(string chars)
    {
        var dict = new Dictionary<char, int>();
        for (var i = 0; i < chars.Length; i++)
        {
            dict[chars[i]] = i;
        }
        return dict;
    }

    string chars;
    public string Chars => chars;

    string last;
    public string Last => last;


    readonly List<int> indexes;
    readonly object lockObj = new object();

    readonly bool hasZero;
    public HasZero HasZero => hasZero ? HasZero.True : HasZero.False;

    public ID(string last, string? chars, HasZero hasZero = HasZero.Auto)
    {
        chars = chars ?? IDChars.Digits;
        if (chars.Length < 2)
        {
            throw new IDException($"'chars' must have at least members");
        }
        if (chars.Length > chars.Distinct().Count())
        {
            throw new IDException($"'chars' contains duplicate characters");
        }
        this.chars = chars;
        this.hasZero = hasZero switch
        {
            HasZero.Auto => chars[0] == '0',
            HasZero.True => true,
            HasZero.False => false,
            _ => throw new IDException($"Unexpected ZeroChar value: {hasZero}")
        };
        this.indexes = new List<int>(last.Select(c => chars.IndexOf(c)));
        foreach (var c in last)
        {
            if (!this.chars.Contains(c))
            {
                throw new IDException($"Unexpected character '{c}' in last.");
            }
        }
        this.last = last;
        this.CharIndexDict = MakeDict(chars);
    }

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
            indexes.Insert(0, hasZero && indexes.Count > 0 ? 1 : 0);
            return SetLastFromIndexes();
        }
    }

    string SetLastFromIndexes()
    {

        var cs = indexes.Select(i => chars[i]);
        last = new string(cs.ToArray());
        return last;
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
            int xInd = 0, yInd = 0;
            if (hasZero)
            {
                while (xInd < x.Length - 1 && x[xInd] == chars[0])
                {
                    xInd++;
                }
                while (yInd < y.Length - 1 && y[yInd] == chars[0])
                {
                    yInd++;
                }
            }
            int xSig = x.Length - xInd, ySig = y.Length - yInd;
            if (xSig != ySig)
            {
                return xSig - ySig;
            }
            else if (xSig == 0)
            {
                return 0;
            }
            else
            {
                while (x[xInd] == y[yInd] && xInd < x.Length - 1)
                {
                    xInd++; yInd++;
                }
                return CharIndexDict[x[xInd]] - CharIndexDict[y[yInd]];
            }
        }
    }

    public bool Equals(string? x, string? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] string obj)
    {
        return obj.GetHashCode();
    }
}
