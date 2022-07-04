using System.Text.Json;

using Fmbm.Text;

// var chars = ID.Chars.Hex;
// var id = "1234";
// var num = ID.StringToNum(id, chars);
// var id2 = ID.NumToString(num, chars);
// Console.WriteLine(id);
// Console.WriteLine(num);
// Console.WriteLine(id2);

var id = new ID("", "10", ZeroChar.True);

for (int i = 0; i < 13; i++)
{
    Console.WriteLine(id.Next());
}

var json = JsonSerializer.Serialize(id);
Console.WriteLine(json);

var id2 = JsonSerializer.Deserialize<ID>(json)!;

for (int i = 0; i < 6; i++)
{
    Console.WriteLine(id2.Next());
}

var x = 0;
var y = 1;
Console.WriteLine(x.CompareTo(y));
Console.WriteLine(id2.Compare(x.ToString(), y.ToString()));
