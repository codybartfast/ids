using Fmbm.Text;

var id = new ID("00", IDChars.Digits, HasZero.False);
for (int i = 0; i < 1020; i++)
{
    id.Next();
}
Console.WriteLine(id.Last);
