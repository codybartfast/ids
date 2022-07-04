using System.Numerics;

namespace Fmbm.Text;

public partial class ID
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
    readonly bool zeroChar;


    readonly List<int> lastIndexes;
    readonly object lockObj = new object();

    public ID(string last, string chars, bool zeroChar)
    {
        // last only contains chars
        // at least two chars
        // zeroChar is char
        // last no leading zero
        this.chars = ID.CharSets.Digits;
        this.zeroChar = true;
        this.lastIndexes = new List<int>(
            last.Reverse<char>().Select(c => chars.IndexOf(c)));
        this.Last = last;
    }

    // public ID(string last, string chars)
    //     : this(last, chars, chars[0] == '0') { }

    public string Last { get; private set; }
    public string Chars => chars;
    public bool ZeroChar => zeroChar;


    public string Next()
    {
        return Increment();
    }

    internal string Increment()
    {
        lock (lockObj)
        {
            for (var i = 0; i < lastIndexes.Count; i++)
            {
                if (lastIndexes[i] < chars.Length - 1)
                {
                    lastIndexes[i] += 1;
                    return SetLastFromIndexes();
                }
                lastIndexes[i] = 0;
            }
            lastIndexes.Add(zeroChar && lastIndexes.Count > 0 ? 1 : 0);
            return SetLastFromIndexes();
        }
    }

    string SetLastFromIndexes()
    {
        var cs = lastIndexes.Reverse<int>().Select(i => chars[i]);
        Last = new string(cs.ToArray());
        return Last;
    }
}
