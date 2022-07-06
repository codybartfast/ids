using Fmbm.Text;

var id = new ID("0007");
for(int i = 0; i < 5; i++){
    Console.WriteLine(id.Next());
}

// Output:
// 0008
// 0009
// 0010
// 0011
// 0012
