using System.Text.Json;
using Xunit;

namespace Fmbm.Text.Tests;

public class IDSerializationTests
{
    [Fact]
    public void PublicPropertiesUnchanged()
    {
        ID id1, id2;
        id1 = new ID("123", IDChars.Decimal, true);
        id2 = JsonSerializer.Deserialize<ID>(JsonSerializer.Serialize(id1))!;
        Assert.Equal(id1.Last, id2.Last);
        Assert.Equal(id1.Chars, id2.Chars);
        Assert.Equal(id1.Numeric, id2.Numeric);

        id1 = new ID("ABC", "ABCD", false);
        id2 = JsonSerializer.Deserialize<ID>(JsonSerializer.Serialize(id1))!;
        Assert.Equal(id1.Last, id2.Last);
        Assert.Equal(id1.Chars, id2.Chars);
        Assert.Equal(id1.Numeric, id2.Numeric);
    }

    string DeserializedRestartsAsExpected(bool numeric)
    {
        var id = new ID("00", IDChars.Decimal, numeric);
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
    public void DeserializedRestartsAsExpectedNumeric()
    {
        var last = DeserializedRestartsAsExpected(true);
        Assert.Equal("1020", last);
    }

    [Fact]
    public void DeserializedRestartsAsExpectedNonNumeric()
    {
        var last = DeserializedRestartsAsExpected(false);
        Assert.Equal("920", last);
    }
}
