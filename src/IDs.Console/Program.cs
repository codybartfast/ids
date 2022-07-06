using System.Diagnostics;
using Fmbm.Text;

var id = new ID("");

var sw = new Stopwatch();
sw.Start();

for (int i = 0; i < 19_000_000; i++)
{
    id.Next();
}

Console.WriteLine(sw.Elapsed);
