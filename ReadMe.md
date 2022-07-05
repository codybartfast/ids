# IDs

Simple, customizable string ID generator.

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

Without any constructor arguments, ID uses the digits 0-9.  Its behaviour is similar to incrementing an integer except it returns `string`s and is thread safe.  The initial value of `Last` is an empty string and the first value 
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

Length of IDs
-------------

More charactes will enable shorter ids, but may be less readable.  As examples,
here is the 50,000,000th id produced by diffent sets.

```chsharp
var id = new ID(chars: IDChars.Binary);
for(int i = 0; i < 50_000_000; i++){
    id.Next();
}
Console.WriteLine($"{id.Last}");

// Output:
// 10111110101111000001111111
```

```text
Binary:         100110001001011001111111
Dog!:           oggoogog!Dgg! 
Decimal:        49999999
Hex:            2FAF07F
LessAmbiguous:  32qujp
Base64:         B9uA_
AsciiPrintable: Y=.j
```

compare
