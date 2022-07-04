using System.Text.Json;

using Fmbm.Text;

// var id = new ID("", "10", ZeroChar.True);

// for (int i = 0; i < 13; i++)
// {
//     Console.WriteLine(id.Next());
// }

// var json = JsonSerializer.Serialize(id);
// Console.WriteLine(json);

// var id2 = JsonSerializer.Deserialize<ID>(json)!;

// for (int i = 0; i < 6; i++)
// {
//     Console.WriteLine(id2.Next());
// }

var id = new ID("00", IDChars.Digits, HasZero.False);
for(int i = 0; i < 1020; i++){
    id.Next();
}
Console.WriteLine(id.Last);
