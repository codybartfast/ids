namespace Fmbm.Text;

public static class IDChars
{
    public const string Binary = "01";
    public const string Decimal = Binary + "23456789";
    public const string HexLower = Decimal + "abcdef";
    public const string HexUpper = Decimal + "ABCDEF";
    public const string Lower = "abcdefghijklmnopqrstuvwxyz";
    public const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public const string DigitsAndLower = Decimal + Lower;
    public const string DigitsAndUpper = Decimal + Upper;
    public const string DigitsAndLowerAndUpper = DigitsAndLower + Upper;
    public const string Base64 = Upper + Lower + Decimal + "-_";
    public const string AsciiPrintableNoSpace =
        "!\"#$%&'()*+,-./" + Decimal + ":;<=>?@" +
        Upper + "[\\]^_`" + Lower + "{|}~";
    public const string AsciiPrintable = " " + AsciiPrintableNoSpace;
    public const string LessAmbiguous = "2345679abcdefghjkmnpqrstuvwxyz";
}
