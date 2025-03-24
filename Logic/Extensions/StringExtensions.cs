namespace Logic.Extensions;

static class StringExtensions
{
    internal static (string Original, string TextPart, int NumberPart) SplitToParts(this string str, char sepatator = '.')
    {
        int dotIndex = str.IndexOf(sepatator);
        if (dotIndex > 0 && int.TryParse(str.AsSpan(0, dotIndex), out int number))
        {
            return (str, str[(dotIndex + 1)..].TrimStart(), number);
        }

        return (str, str, 0);
    }
}
