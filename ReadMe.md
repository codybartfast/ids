# IDs

Sequential string ID generator.

Features:

* Customizable character sets
* Numeric and non-numeric styles
* Comparer for custom character sets
* Sequential Ids
* Serializable
* Thread-safe.

Not suitable for:

* Distributed ID generation
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

&nbsp;

Basic Usage
-----------

Without any constructor arguments, ID uses the digits 0-9.  Its behaviour is
similar to incrementing an integer except it returns `string`s and is thread
safe.  The initial value of `Last` is an empty string and the first value
created by `Next()` will be `"0"`.

```csharp
using Fmbm.Text;

ID id = new ID();
for (int i = 0; i < 3; i++)
{
    string last = id.Last;
    string next = id.Next();
    Console.WriteLine($"Last:{last}, Next:{next}");
}

// Output
// Last:, Next:0
// Last:0, Next:1
// Last:1, Next:2
```

&nbsp;

Continuing a sequence
---------------------

`ID` can be initialized with the last (highest) value to continue generating
from that value.  E.g., if the last value generated was `"37"` then:

```csharp
var id = new ID("37");
Console.WriteLine($"Last:{id.Last}, Next:{id.Next()}");

// Output
// Last:37, Next:38
```

An `ID` can also be serialized and deserialized to restore the previous state:

```csharp
var originalID = new ID("37");
var jsonText = JsonSerializer.Serialize(originalID);
Console.WriteLine(jsonText);
var newID = JsonSerializer.Deserialize<ID>(jsonText);
Console.WriteLine($"Last:{originalID.Last}, Next:{originalID.Next()}");

// Output
// {"Last":"37","Chars":"0123456789","Numeric":1}
// Last:37, Next:38
```

&nbsp;

Custom Characters
-----------------

A custom set of characters can be specified by providing a string containing
those characters.  E.g. to just use the characters `'D'`, `'o'`, `'g'` and
`'!'`:

```csharp
var id = new ID(chars: "Dog!");
for (int k = 0; k < 9; k++)
{
    Console.WriteLine(id.Next());
}

// Output
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
                Base64: ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123
                        456789-_
 AsciiPrintableNoSpace: !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWX
                        YZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
        AsciiPrintable:  !"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVW
                        XYZ[\]^_`abcdefghijklmnopqrstuvwxyz{|}~
        LessAmbiguous:  2345679abcdefghjkmnpqrstuvwxyz

```

&nbsp;

Length of IDs
-------------

More charactes will enable shorter ids, but may be less readable.  As examples,
here is the 50,000,000th id produced by diffent sets.

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

&nbsp;

Numeric IDs
-----------

With Base64 characters the first two char id is `"AA"`.  However, with
decimal characters the first two char id is `"10"` not `"00"`.  If we consider
id strings as representing numbers then `"0"`, `"00"` and `"000"` represent the
same number, `0`.  To avoid ambiguity or duplication we typically do not want to
generate `"00"` and `"000"` as ids.

But `"A"`, `"AA"` and `"AAA"` are typically consdered distinct ids (as with
spreadsheet columns) and we do want to generate them.

IDs calls this  _numeric_ or _nonnumeric_ behaviour.  Numeric behaviour treats
the first character in `chars` as a zero and it will only be the leftmost
character for the first id.  Nonnumeric behaviour makes no distinction for the
first character.

If the first char in the character set is `"0"` then numeric behaviour is used,
otherwise nonnumeric behaviour is used.  E.g., `Binary`, `Decimal` and
`HexUpper` ("01", "0123456789" and "0123456789ABCDEF") all start with `0` and so
the behaviour is numeric. `Base64` (`"ABC..."`) starts with `"A"` so the
behaviour is nonnumeric.

The automatic behaviour can be overriden by specifying a numeric argument of
`True` or `False`. To specify nonnumeric behaviour:

```csharp
var id = new ID(last: "7", chars: IDChars.Decimal, numeric: IDNumeric.False);

for(int i = 0; i < 4; i++){
    Console.WriteLine(id.Next());
}

// Output:
// 8
// 9
// 00  <-- "9" is followed by "00" insted of "10"
// 01
```

Similarly letters can be treated numerically so that `A` now acts like a zero.

```csharp
var id = new ID(last: "X", chars: IDChars.Upper, numeric: IDNumeric.True);
for(int i = 0; i < 4; i++){
    Console.WriteLine(id.Next());
}

// Output:
// Y
// Z
// BA  <-- Z is followed by "BA" instead of  "AA"
// BB
```

&nbsp;

Comparing IDs
-------------
