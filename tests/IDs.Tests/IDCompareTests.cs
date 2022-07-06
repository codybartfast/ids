using Xunit;

namespace Fmbm.Text.Tests;

public class IDCompareTests
{

    bool IsNeg(int n) { return n < 0; }
    bool IsZero(int n) { return n == 0; }
    bool IsPos(int n) { return n > 0; }

    [Fact]
    public void NumericIdIsWellOrdered()
    {
        var id = new ID(string.Empty, IDChars.AsciiPrintable, true);
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
    public void NonNumericIdIsWellOrdered()
    {
        var id = new ID(string.Empty, IDChars.AsciiPrintable, false);
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
    public void NumericIdIsWellOrderedWhenPadded()
    {
        var id = new ID("010", IDChars.Decimal, true);
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
    public void NonNumericIdIsWellOrderedWhenPadded()
    {
        var id = new ID("010", IDChars.AsciiPrintable, false);
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
