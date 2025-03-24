namespace Logic.Extensions;

static class StreamExtensions
{
    internal static IEnumerable<IEnumerable<string>> ReadLineParts(this Stream stream, int partSize, CancellationToken token)
    {
        if (partSize <= 0)
        {
            throw new ArgumentException("Wrong size", nameof(partSize));
        }

        List<string> lines = new(partSize);
        using StreamReader reader = new(stream);
        while (reader.EndOfStream == false)
        {
            token.ThrowIfCancellationRequested();

            if (lines.Count >= partSize)
            {
                yield return lines;
                lines = new(partSize);
            }

            lines.Add(reader.ReadLine()!);
        }

        if (lines.Count > 0)
        {
            yield return lines;
        }
    }
}
