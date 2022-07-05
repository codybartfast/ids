using Fmbm.Text;

var id = new ID(last: "X", chars: IDChars.Upper, numeric: Numeric.True);
for(int i = 0; i < 4; i++){
    Console.WriteLine(id.Next());
}

// Output:
// Y
// Z
// BA
// BB