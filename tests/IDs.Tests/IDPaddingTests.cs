using Xunit;

namespace Fmbm.Text.Tests;

public class IDPaddingTests
{
    [Theory]
    [InlineData("000", "0123456789", HasZero.False, "001")]
    [InlineData("000", "0123456789", HasZero.True, "001")]
    [InlineData("009", "0123456789", HasZero.False, "010")]
    [InlineData("009", "0123456789", HasZero.True, "010")]
    [InlineData("999", "0123456789", HasZero.False, "0000")]
    [InlineData("999", "0123456789", HasZero.True, "1000")]
    public void LeftPaddingIsKept(
        string last, string chars, HasZero zeroChar, string next)
    {
        var id = new ID(last, chars, zeroChar);
        Assert.Equal(next, id.Next());
    }

    [Fact]
    public void PaddedAreEqualIfZeroChar()
    {
        var id = new ID("", IDChars.Digits, HasZero.True);
        Assert.False(id.Equals("010123", "123"));
        Assert.True(id.Equals("000123", "123"));
    }

    [Fact]
    public void PaddedNotEqualIfNoZeroChar()
    {
        var id = new ID("", IDChars.Digits, HasZero.False);
        Assert.True(id.Equals("123", "123"));
        Assert.False(id.Equals("000123", "123"));
    }
}