using Xunit;

namespace Fmbm.Text.Tests;

public class IDCharsTests
{
    [Fact]
    public void ExpectedNumberOfPrintable()
    {
        Assert.Equal(95, IDChars.AsciiPrintable.Length);
    }

    [Fact]
    public void CorrectPrintables()
    {
        for (int i = 0; i < 95; i++){
            Assert.Equal<char>((char)(i + 32),IDChars.AsciiPrintable[i]);
        }
    }
}