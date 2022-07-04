using System.Text.Json;

using Fmbm.Text;

// var chars = ID.Chars.Hex;
// var id = "1234";
// var num = ID.StringToNum(id, chars);
// var id2 = ID.NumToString(num, chars);
// Console.WriteLine(id);
// Console.WriteLine(num);
// Console.WriteLine(id2);

var id = new ID("", ID.CharSets.Digits, true);

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(id.Next());
}

var json = JsonSerializer.Serialize(id);
Console.WriteLine(json);

id = JsonSerializer.Deserialize<ID>(json)!;

for (int i = 0; i < 10; i++)
{
    Console.WriteLine(id.Next());
}
