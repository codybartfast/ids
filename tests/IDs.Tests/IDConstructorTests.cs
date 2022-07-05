using System;
using Xunit;

namespace Fmbm.Text.Tests;

public class IDConstructorTests
{
    [Theory]
    [InlineData("", "0")]
    [InlineData("0", "1")]
    [InlineData("0000", "0001")]
    [InlineData("1", "2")]
    [InlineData("9", "10")]
    [InlineData("99", "100")]
    [InlineData("1234", "1235")]
    public void LastAsExpected(string last, string next)
    {
        var idT = new ID(last, IDChars.Decimal, Numeric.True);
        Assert.Equal(last, idT.Last);
        Assert.Equal(next, idT.Next());
        Assert.Equal(next, idT.Last);
        var idF = new ID(last, IDChars.Decimal, Numeric.False);
        Assert.Equal(last, idF.Last);
    }

    [Theory]
    [InlineData("0123456789", "999")]
    [InlineData("0123456789abcdef", "3e7")]
    [InlineData("01", "1111100111")]
    [InlineData("10", "0000011000")]
    public void CharsArgumentIsUsed(string chars, string last)
    {
        string GetLast(string last, string chars)
        {
            var id = new ID(last, chars, Numeric.True);
            for (var i = 0; i < 1000; i++)
            {
                id.Next();
            }
            return id.Last;
        }
        Assert.Equal(last, GetLast("", chars));
    }

    [Theory]
    [InlineData("", "0123456789", Numeric.Auto, "0")]
    [InlineData("", "0123456789", Numeric.False, "0")]
    [InlineData("", "0123456789", Numeric.True, "0")]
    [InlineData("9", "0123456789", Numeric.Auto, "10")]
    [InlineData("9", "0123456789", Numeric.False, "00")]
    [InlineData("9", "0123456789", Numeric.True, "10")]
    [InlineData("9", "Z123456789", Numeric.Auto, "ZZ")]
    [InlineData("9", "Z123456789", Numeric.False, "ZZ")]
    [InlineData("9", "Z123456789", Numeric.True, "1Z")]
    public void NumericArgumentUsed(
        string last, string chars, Numeric numeric, string next)
    {
        var id = new ID(last, chars, numeric);
        Assert.Equal(next, id.Next());
    }

    [Fact]
    public void LastMustOnlyUseKnownChars()
    {
        Action uknownChar = () => new ID("Z123", IDChars.Decimal);
        Assert.Throws<IDException>(uknownChar);
    }

    [Fact]
    public void CharsMustBeUnique()
    {
        Action duplicateChar = () => new ID("", "1223");
        Assert.Throws<IDException>(duplicateChar);
    }

    [Fact]
    public void MustBeAtLeastTwoChars()
    {
        Action twoChars = () => new ID("", "1");
        Assert.Throws<IDException>(twoChars);
    }
}
