# IDs

Sequential string ID generator.

Features:

* Customizable character sets
* Numeric and non-numeric styles
* Custom Comparer
* Sequential Ids
* Serializable
* Thread-safe.

Not suitable for:

* Distributed IDs
* Globally unique IDs
* IDs requiring a random element (e.g. for security).

&nbsp;

For Me, By Me (FMBM)
--------------------

FMBM packages are created primarily for use by the author.  They are intended
for getting casual, desktop applications up and running quickly.  They may not
be suitable for production, scalable nor critical applications. The name is
inspired by the [Fubu][Fubu] project, '_For Us, By Us_', but there is no other
connection.

&nbsp;

Contents
--------

* [Basic Usage](#basic-usage)
* [Continuing a Sequence](#continuing-a-sequence)
* [Custom Characters](#custom-characters)
* [Predefined Sets of Characters](#predefined-sets-of-characters)
* [Length of Ids](#length-of-ids)
* [Numeric And Non-numeric Ids](#numeric-and-non-numeric-ids)
* [Comparing And Sorting](#comparing-and-sorting)
* [Leading Zeros](#leading-zeros)

&nbsp;

Basic Usage
-----------

IDs default behaviour is similar to incrementing an integer, except it returns
`String`s and is thread safe.  The initial value of `Last` is an empty string
and the first value created by `Next()` will be `"0"`.

```csharp
using Fmbm.Text;

ID id = new ID();
for (int i = 0; i < 3; i++)
{
    string last = id.Last;
    string next = id.Next();
    Console.WriteLine($"Last:\"{last}\", Next:\"{next}\"");
}

// Output:
// Last:"", Next:"0"
// Last:"0", Next:"1"
// Last:"1", Next:"2"
```

&nbsp;

Continuing a Sequence
---------------------

`ID` can be initialized with the last value to continue generating from that
value.  E.g., if the last value generated was `"37"` then:

```csharp
var id = new ID("37");
Console.WriteLine($"Last:{id.Last}, Next:{id.Next()}");

// Output:
// Last:37, Next:38
```

An `ID` can also be serialized and deserialized to restore the previous state:

```csharp
var originalID = new ID("37");
var jsonText = JsonSerializer.Serialize(originalID);
Console.WriteLine(jsonText);

var newID = JsonSerializer.Deserialize<ID>(jsonText);
Console.WriteLine($"Last:{originalID.Last}, Next:{originalID.Next()}");

// Output:
// {"Last":"37","Chars":"0123456789","Numeric":true}
// Last:37, Next:38
```

&nbsp;

Custom Characters
-----------------

A custom set of characters can be specified by providing a string containing
those characters in order.  E.g. to create ids using `'D'`, `'o'`, `'g'` and
`'!'` use the string `"Dog!"`

```csharp
var id = new ID(chars: "Dog!");
for (int k = 0; k < 9; k++)
{
    Console.WriteLine(id.Next());
}

// Output:
// D
// o
// g
// !
// DD
// Do
// Dg
// D!
// oD
```

&nbsp;

Predefined Sets of Characters
-----------------------------

The class `IDChars` contains the following predefined sets of characters:

```text
                Binary: 01
               Decimal: 0123456789
              HexLower: 0123456789abcdef
              HexUpper: 0123456789ABCDEF
                 Lower: abcdefghijklmnopqrstuvwxyz
                 Upper: ABCDEFGHIJKLMNOPQRSTUVWXYZ
        DigitsAndLower: 0123456789abcdefghijklmnopqrstuvwxyz
        DigitsAndUpper: 0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ
DigitsAndLowerAndUpper: 0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRST
                        UVWXYZ
               Base64*: ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123
                        456789-_
 AsciiPrintableNoSpace: !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWX
                        YZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
        AsciiPrintable:  !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVW
                        XYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
        LessAmbiguous:  2345679abcdefghjkmnpqrstuvwxyz

* Using the Base64 character set does not create meaningful Base64 encoded data.
```

```csharp
var id = new ID(chars: IDChars.Base64);
for (int i = 0; i < 3; i++)
{
    Console.WriteLine(id.Next());
}

// Output:
// A
// B
// C
```

&nbsp;

Length of Ids
-------------

More characters will enable shorter ids, but may be less readable.  As examples,
here is the 50,000,000th id produced by different sets.

```csharp
var id = new ID(chars: IDChars.Binary);
for(int i = 0; i < 50_000_000; i++){
    id.Next();
}
Console.WriteLine($"{id.Last}");

// Output:
// 10111110101111000001111111
```

```text
Chars           50,000,000th ID
=====           ===============
Binary:         10111110101111000001111111
"Dog!":         oggoogog!Dgg! 
Decimal:        49999999
Hex:            2FAF07F
LessAmbiguous:  32qujp
Base64:         B9uA_
AsciiPrintable: Y=.j
```

There is no predefined limit on the size of an id:

```csharp
var id = new ID(
  "9999999999999999999999999999999999999999999999999999999999999999999999999999"
);

Console.WriteLine(id.Next());

// Output:
// 10000000000000000000000000000000000000000000000000000000000000000000000000000
```

&nbsp;

Numeric And Non-numeric IDs
--------------------------

With Base64 characters the first id with two characters is `"AA"`.  However,
with decimal characters the first id with two characters is `"10"` not `"00"`.
If we consider id strings as representing numbers then `"0"`, `"00"` and `"000"`
represent the same number, `0`.  To avoid ambiguity or duplication we typically
do not want to generate `"00"` and `"000"` as ids.

But `"A"`, `"AA"` and `"AAA"` are typically considered distinct ids (as with
spreadsheet columns) and we _do_ want to generate these.

`IDs` calls these  _numeric_ and _non-numeric_ behaviour.  Numeric behaviour
treats the first character in `chars` as a zero and it will only be the leftmost
character of an id for the first id (zero).  Non-numeric behaviour makes no
distinction for the first character.

If the first char in the character set is `'0'` then numeric behaviour is
automatically used, otherwise non-numeric behaviour is used.  E.g., `Binary`,
`Decimal` and `HexUpper` (`"01"`, `"0123456789"` and `"0123456789ABCDEF"`) all
start with a `'0'` and so the behaviour is numeric. `Base64` (`"ABC..."`) starts
with an `'A'` so the behaviour is non-numeric.

The automatic behaviour can be overridden by specifying a numeric argument of
`true` or `false`. For example, to specify non-numeric behaviour:

```csharp
var id = new ID(last: "7", numeric: false);

for(int i = 0; i < 4; i++){
    Console.WriteLine(id.Next());
}

// Output:
// 8
// 9
// 00  <-- "9" is followed by "00" instead of "10"
// 01
```

Similarly letters can be treated numerically so that `'A'` can act as a zero.

```csharp
var id = new ID(last: "X", chars: IDChars.Upper, numeric: true);
for(int i = 0; i < 4; i++){
    Console.WriteLine(id.Next());
}

// Output:
// Y
// Z
// BA  <-- "Z" is followed by "BA" instead of  "AA"
// BB
```

&nbsp;

Comparing And Sorting
---------------------

ID implements the `Comparerer<String>` and `IEqualityComparer<string>`
interfaces that it can be used to compare the ids that it generates.  Here ids
generated using the chars `"Dog!"` are sorted alphabetically and then returned
to their original order by using `ID` as a string comparer.

```csharp
var id = new ID(chars: "Dog!");
var ids = new List<string>();
for (int k = 0; k < 9; k++)
{
    ids.Add(id.Next());
}

Console.WriteLine(String.Join(", ", ids));

ids.Sort();
Console.WriteLine(String.Join(", ", ids));

ids.Sort(id);
Console.WriteLine(String.Join(", ", ids));

// Output:
// D, o, g, !, DD, Do, Dg, D!, oD
// !, D, D!, DD, Dg, Do, g, o, oD
// D, o, g, !, DD, Do, Dg, D!, oD
```

The comparison respects whether the behaviour is numeric or non-numeric.  Here
is the default numeric behaviour for decimal digits:

```csharp
var id = new ID();
var ids = new List<string> { "1", "2", "00", "8", "9" };
ids.Sort(id);
Console.WriteLine(String.Join(", ", ids));
Console.WriteLine(id.Equals("0", "00"));

// Output:
// 00, 1, 2, 8, 9
// True
```

And the non-numeric behaviour:

```csharp
var id = new ID(numeric: false);
var ids = new List<string> { "1", "2", "00", "8", "9" };
ids.Sort(id);
Console.WriteLine(String.Join(", ", ids));
Console.WriteLine(id.Equals("0", "00"));

// Output:
// 1, 2, 8, 9, 00
// False
```

&nbsp;

Leading Zeros
-------------

Leading zeros are maintained in both numeric and non-numeric `ID`s:

```csharp
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
```
