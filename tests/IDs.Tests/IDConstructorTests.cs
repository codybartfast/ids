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
        var idT = new ID(last, IDChars.Decimal, true);
        Assert.Equal(last, idT.Last);
        Assert.Equal(next, idT.Next());
        Assert.Equal(next, idT.Last);
        var idF = new ID(last, IDChars.Decimal, false);
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
            var id = new ID(last, chars, true);
            for (var i = 0; i < 1000; i++)
            {
                id.Next();
            }
            return id.Last;
        }
        Assert.Equal(last, GetLast("", chars));
    }

    [Theory]
    [InlineData("", "0123456789", null, "0")]
    [InlineData("", "0123456789", false, "0")]
    [InlineData("", "0123456789", true, "0")]
    [InlineData("9", "0123456789", null, "10")]
    [InlineData("9", "0123456789", false, "00")]
    [InlineData("9", "0123456789", true, "10")]
    [InlineData("9", "Z123456789", null, "ZZ")]
    [InlineData("9", "Z123456789", false, "ZZ")]
    [InlineData("9", "Z123456789", true, "1Z")]
    public void NumericArgumentUsed(
        string last, string chars, bool? numeric, string next)
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

    class Fruit
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
    }

    Fruit[] fruit = new[]{
            new Fruit{Id = "2", Name = "Banana"},
            new Fruit{Id = "1", Name = "Apple"},
            new Fruit{Id = "4", Name = "Durian"},
            new Fruit{Id = "3", Name = "Cherry"},
        };

    [Fact]
    public void FromExistingWithDefaults()
    {
        var id = ID.FromExisting(fruit, f => f.Id!);
        Assert.Equal("5", id.Next());
    }

    [Fact]
    public void FromExistingWithExplicit()
    {
        var id = ID.FromExisting(fruit, f => f.Id!, "01234", false);
        Assert.Equal("00", id.Next());
    }
}
