namespace Fmbm.Text;


public static class IDChars
{
    public static string Digits { get; } = "0123456789";
    public static string HexLower { get; } = "0123456789abcdef";
    public static string HexUpper { get; } = "0123456789abcdef";
    public static string Lower { get; } = "abcdefghijklmnopqrstuvwxyz";
    public static string Upper { get; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    public static string DigitsAndLower { get; } = Digits + Lower;
    public static string DigitsAndUpper { get; } = Digits + Upper;
    public static string DigitsAndLowerAndUpper { get; } = DigitsAndLower + Upper;
    public static string Base64 { get; } = Upper + Lower + Digits + "+/";
    public static string AsciiPrintableNoSpace { get; } =
        "!\"#$%&'()*+,-./" + Digits + ":;<=>?@" +
        Upper + "[\\]^_`" + Lower + "{|}~";
    public static string AsciiPrintable { get; } = " " + AsciiPrintableNoSpace;
    public static string LessAmbiguous { get; } =
        "23456789abcdefghjkmnpqrstuvwxyz";
}