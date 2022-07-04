using Xunit;

namespace Fmbm.Text.Tests;

public class IDCompareTests
{

    bool IsNeg(int n) { return n < 0; }
    bool IsZero(int n) { return n == 0; }
    bool IsPos(int n) { return n > 0; }

    [Fact]
    public void WithZeroIdIsWellOrdered()
    {
        var id = new ID(string.Empty, IDChars.AsciiPrintable, HasZero.True);
        for (int i = 0; i < 10000; i++)
        {
            var previous = id.Last;
            Assert.True(IsNeg(id.Compare(id.Last, id.Next())));
            Assert.True(IsZero(id.Compare(id.Last, id.Last)));
            Assert.True(id.Equals(id.Last, id.Last));
            Assert.True(IsPos(id.Compare(id.Last, previous)));
        }
    }

    [Fact]
    public void WithoutZeroIdIsWellOrdered()
    {
        var id = new ID(string.Empty, IDChars.AsciiPrintable, HasZero.False);
        for (int i = 0; i < 10000; i++)
        {
            var previous = id.Last;
            Assert.True(IsNeg(id.Compare(id.Last, id.Next())));
            Assert.True(IsZero(id.Compare(id.Last, id.Last)));
            Assert.True(id.Equals(id.Last, id.Last));
            Assert.True(IsPos(id.Compare(id.Last, previous)));
        }
    }

    [Fact]
    public void WithZeroIdIsWellOrderedWhenPadded()
    {
        var id = new ID("010", IDChars.Digits, HasZero.True);
        for (int i = 0; i < 10000; i++)
        {
            var previous = id.Last;
            Assert.True(IsNeg(id.Compare(id.Last, id.Next())));
            Assert.True(IsZero(id.Compare(id.Last, id.Last)));
            Assert.True(id.Equals(id.Last, id.Last));
            Assert.True(IsPos(id.Compare(id.Last, previous)));
        }
    }

    [Fact]
    public void WithoutZeroIdIsWellOrderedWhenPadded()
    {
        var id = new ID("010", IDChars.AsciiPrintable, HasZero.False);
        for (int i = 0; i < 10000; i++)
        {
            var previous = id.Last;
            Assert.True(IsNeg(id.Compare(id.Last, id.Next())));
            Assert.True(IsZero(id.Compare(id.Last, id.Last)));
            Assert.True(id.Equals(id.Last, id.Last));
            Assert.True(IsPos(id.Compare(id.Last, previous)));
        }
    }
}