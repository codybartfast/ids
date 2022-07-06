using Xunit;

namespace Fmbm.Text.Tests;

public class IDHexTests
{
    [Fact]
    public void MatchesToStrign()
    {
        var id = new ID("", IDChars.HexUpper, IDNumeric.True);
        for (var i = 0; i < 17000; i++)
        {
            Assert.Equal(i.ToString("X"), id.Next());
        }
    }
}