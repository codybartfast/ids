using Fmbm.Text;

var id = new ID(chars: IDChars.LessAmbiguous);
for(int i = 0; i < 50_000_000; i++){
    id.Next();
}
Console.WriteLine($"{id.Last}");

// Output:
// 10111110101111000001111111