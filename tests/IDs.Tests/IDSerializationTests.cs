using System.Text.Json;
using Xunit;

namespace Fmbm.Text.Tests;

public class IDSerializationTests
{
    [Fact]
    public void PublicPropertiesUnchanged()
    {
        ID id1, id2;
        id1 = new ID("123", IDChars.Digits, HasZero.True);
        id2 = JsonSerializer.Deserialize<ID>(JsonSerializer.Serialize(id1))!;
        Assert.Equal(id1.Last, id2.Last);
        Assert.Equal(id1.Chars, id2.Chars);
        Assert.Equal(id1.HasZero, id2.HasZero);

        id1 = new ID("ABC", "ABCD", HasZero.False);
        id2 = JsonSerializer.Deserialize<ID>(JsonSerializer.Serialize(id1))!;
        Assert.Equal(id1.Last, id2.Last);
        Assert.Equal(id1.Chars, id2.Chars);
        Assert.Equal(id1.HasZero, id2.HasZero);
    }

    string DeserializedRestartsAsExpected(HasZero zeroChar)
    {
        var id = new ID("00", IDChars.Digits, zeroChar);
        for (int i = 0; i < 1020; i++)
        {
            var previous = id.Next();
            string json = JsonSerializer.Serialize(id);
            id = JsonSerializer.Deserialize<ID>(json)!;
            Assert.Equal(previous, id.Last);
        }
        return id.Last;
    }

    [Fact]
    public void DeserializedRestartsAsExpectedZero()
    {
        var last = DeserializedRestartsAsExpected(HasZero.True);
        Assert.Equal("1020", last);
    }

    [Fact]
    public void DeserializedRestartsAsExpectedNoZero()
    {
        var last = DeserializedRestartsAsExpected(HasZero.False);
        Assert.Equal("920", last);
    }
}
