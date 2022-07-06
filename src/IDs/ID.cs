using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;
namespace Fmbm.Text;

public partial class ID : IComparer<string>, IEqualityComparer<string>
{
    string current;
    private int[] indexes = new int[4];
    private int length;
    public string Last => current;

    readonly string chars;
    public string Chars => chars;

    readonly bool numeric;
    public bool Numeric => numeric;

    readonly IImmutableDictionary<char, int> charIndexDict;
    readonly object lockObj = new object();

    public ID(
        string last = "", string chars = IDChars.Decimal, bool? numeric = null)
        : this(last, chars, numeric ?? chars[0] == '0') { }

    [JsonConstructor]
    public ID(string last, string chars, bool numeric)
    {
        if (chars.Length < 2)
        {
            throw new IDException($"'chars' must have at least 2 members");
        }

        if (chars.Length > chars.Distinct().Count())
        {
            throw new IDException($"'chars' contains duplicate characters");
        }

        foreach (var c in last)
        {
            if (!chars.Contains(c))
            {
                throw new IDException($"Unexpected character '{c}' in 'last'.");
            }
        }

        this.current = last;
        this.chars = chars;
        this.numeric = numeric;

        charIndexDict =
            chars
            .Select((c, i) => new KeyValuePair<char, int>(c, i))
            .ToImmutableDictionary();

        SetIndexesFromCurrent();
    }

    void EnlargeIndexes()
    {
        Array.Resize(ref indexes, length * 2);
    }

    public string Next()
    {
        lock (lockObj)
        {
            for (var i = 0; i < length; i++)
            {
                if (indexes[i] < chars.Length - 1)
                {
                    indexes[i] += 1;
                    return SetCurrentFromIndexes();
                }
                indexes[i] = 0;
            }
            if (length == indexes.Length)
            {
                EnlargeIndexes();
            }
            indexes[length] = (numeric && length > 0) ? 1 : 0;
            length++;
            return SetCurrentFromIndexes();
        }
    }

    void SetIndexesFromCurrent()
    {
        length = current.Length;
        while (current.Length > indexes.Length)
        {
            EnlargeIndexes();
        }
        for (
            int currentCharsIdx = 0, indexesIdx = length - 1;
            currentCharsIdx < length;
            currentCharsIdx++, indexesIdx--)
        {
            indexes[indexesIdx] = charIndexDict[current[currentCharsIdx]];
        }
    }

    string SetCurrentFromIndexes()
    {
        var currentChars = new char[length];
        for (
            int currentCharsIdx = 0, indexesIdx = length - 1;
            currentCharsIdx < length;
            currentCharsIdx++, indexesIdx--)
        {
            currentChars[currentCharsIdx] = chars[indexes[indexesIdx]];
        }
        return (current = new string(currentChars));
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
