using Xunit;

namespace Fmbm.Text.Tests;

public class IDPaddingTests
{
    [Theory]
    [InlineData("000", "0123456789", IDNumeric.False, "001")]
    [InlineData("000", "0123456789", IDNumeric.True, "001")]
    [InlineData("009", "0123456789", IDNumeric.False, "010")]
    [InlineData("009", "0123456789", IDNumeric.True, "010")]
    [InlineData("999", "0123456789", IDNumeric.False, "0000")]
    [InlineData("999", "0123456789", IDNumeric.True, "1000")]
    public void LeftPaddingIsKept(
        string last, string chars, IDNumeric numeric, string next)
    {
        var id = new ID(last, chars, numeric);
        Assert.Equal(next, id.Next());
    }

    [Fact]
    public void PaddedAreEqualIfNumeric()
    {
        var id = new ID("", IDChars.Decimal, IDNumeric.True);
        Assert.False(id.Equals("010123", "123"));
        Assert.True(id.Equals("000123", "123"));
    }

    [Fact]
    public void PaddedNotEqualIfNonNumeric()
    {
        var id = new ID("", IDChars.Decimal, IDNumeric.False);
        Assert.True(id.Equals("123", "123"));
        Assert.False(id.Equals("000123", "123"));
    }
}