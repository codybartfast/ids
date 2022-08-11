using System.Collections.Immutable;
using System.Text.Json.Serialization;

namespace Fmbm.Text;

public partial class ID : IComparer<string>, IEqualityComparer<string>
{
    // 'indexes' represents the last id that was returned.  It contains the
    // index in 'chars' of the charaters that formed the last id, but in reverse
    // order.  If 'chars' is "ABC...XYZ" and the last id was "CAT" then the
    // first three values in 'indexes' are 19, 0, and 2.
    private int[] indexes = new int[4];
    private string last;
    private int length;
    public string Last => last;

    readonly string chars;
    public string Chars => chars;

    readonly bool numeric;
    public bool Numeric => numeric;

    // maps a character to its index in 'chars'
    readonly ImmutableDictionary<char, int> charIndexDict;
    readonly object lockObj = new object();

    // Two constructors because JsonSerializer does not seem to like
    // deserializing  `bool?`
    public ID(
        string? last = null, string? chars = null, bool? numeric = null)
        : this(
            last ?? "",
            chars ?? IDChars.Decimal,
            numeric ?? (chars ?? IDChars.Decimal)[0] == '0')
    { }

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

        this.last = last;
        this.chars = chars;
        this.numeric = numeric;

        charIndexDict =
            chars
            .Select((c, i) => new KeyValuePair<char, int>(c, i))
            .ToImmutableDictionary();

        SetIndexesFromLast();
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
                    return SetLastFromIndexes();
                }
                else
                {
                    indexes[i] = 0;
                }
            }
            // If reach here, need to make the id longer. E.g., "999" -> "1000"
            if (length == indexes.Length)
            {
                EnlargeIndexes();
            }
            indexes[length] = (numeric && length > 0) ? 1 : 0;
            length++;
            return SetLastFromIndexes();
        }
    }

    string SetLastFromIndexes()
    {
        var lastChars = new char[length];
        for (
            int lastCharsIdx = 0, indexesIdx = length - 1;
            lastCharsIdx < length;
            lastCharsIdx++, indexesIdx--)
        {
            lastChars[lastCharsIdx] = chars[indexes[indexesIdx]];
        }
        return (last = new string(lastChars));
    }

    void SetIndexesFromLast()
    {
        length = last.Length;
        while (last.Length > indexes.Length)
        {
            EnlargeIndexes();
        }
        for (
            int lastCharsIdx = 0, indexesIdx = length - 1;
            lastCharsIdx < length;
            lastCharsIdx++, indexesIdx--)
        {
            indexes[indexesIdx] = charIndexDict[last[lastCharsIdx]];
        }
    }

    void EnlargeIndexes()
    {
        Array.Resize(ref indexes, length * 2);
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
                    // if numeric adavance past leading zeros
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

    public int GetHashCode(string x)
    {
        return x.GetHashCode();
    }
}
