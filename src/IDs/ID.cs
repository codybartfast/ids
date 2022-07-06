﻿using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Fmbm.Text;

public partial class ID : IComparer<string>, IEqualityComparer<string>
{
    string last;
    public string Last => last;

    readonly string chars;
    public string Chars => chars;

    readonly bool numeric;
    public bool Numeric => numeric;

    readonly List<int> indexes;

    readonly Dictionary<char, int> charIndexDict;
    readonly object lockObj = new object();

    public ID(
        string last = "", string chars = IDChars.Decimal, bool? numeric = null)
        : this(last, chars, numeric ?? chars[0] == '0') { }

    [JsonConstructor]
    public ID(string last, string chars, bool numeric)
    {
        chars = chars ?? IDChars.Decimal;
        if (chars.Length < 2)
        {
            throw new IDException($"'chars' must have at least 2 members");
        }
        if (chars.Length > chars.Distinct().Count())
        {
            throw new IDException($"'chars' contains duplicate characters");
        }
        this.chars = chars;
        this.numeric = numeric;
        this.indexes = new List<int>(last.Select(c => chars.IndexOf(c)));
        foreach (var c in last)
        {
            if (!this.chars.Contains(c))
            {
                throw new IDException($"Unexpected character '{c}' in 'last'.");
            }
        }
        this.last = last;
        this.charIndexDict = MakeDict(chars);

        Dictionary<char, int> MakeDict(string chars)
        {
            var dict = new Dictionary<char, int>();
            for (var i = 0; i < chars.Length; i++)
            {
                dict[chars[i]] = i;
            }
            return dict;
        }
    }

    public string Next()
    {
        lock (lockObj)
        {
            for (var i = indexes.Count - 1; i >= 0; i--)
            {
                if (indexes[i] < chars.Length - 1)
                {
                    indexes[i] += 1;
                    return SetLastFromIndexes();
                }
                indexes[i] = 0;
            }
            indexes.Insert(0, (numeric && indexes.Count > 0) ? 1 : 0);
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
            int firstSignificant(string s)
            {
                int index = 0;
                if (numeric)
                {
                    while (index < s.Length - 1 && s[index] == chars[0])
                    {
                        index++;
                    }
                }
                return index;
            }

            var xInd = firstSignificant(x);
            var yInd = firstSignificant(y);
            var sigLenX = x.Length - xInd;
            var sigLenY = y.Length - yInd;
            if (sigLenX != sigLenY)
            {
                return sigLenX - sigLenY;
            }
            else
            {
                for (; xInd < x.Length; xInd++, yInd++)
                {
                    if (x[xInd] != y[yInd])
                    {
                        return charIndexDict[x[xInd]] - charIndexDict[y[yInd]];
                    }
                }
                return 0;
            }
        }
    }

    public bool Equals(string? x, string? y)
    {
        return Compare(x, y) == 0;
    }

    public int GetHashCode([DisallowNull] string str)
    {
        return str.GetHashCode();
    }
}
