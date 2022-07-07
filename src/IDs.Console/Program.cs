using System.Diagnostics;
using System.Numerics;
using System.Text.Json;
using Fmbm.Text;

var id = new ID("0006");
for(int i = 0; i < 5; i++){
    Console.WriteLine(id.Next());
}

// Output:
// 0007
// 0008
// 0009
// 0010
// 0011