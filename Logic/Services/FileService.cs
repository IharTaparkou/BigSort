using Logic.Extensions;

namespace Logic.Services;

public class FileService
{
    public static async Task<string[]> SplitIntoSortedParts(string sourceFilePath, string tempDirectoryPath, int blockSize, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sourceFilePath, nameof(sourceFilePath));
        ArgumentException.ThrowIfNullOrWhiteSpace(tempDirectoryPath, nameof(tempDirectoryPath));

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
        using FileStream fileStream = new(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read);

        var parts = fileStream.ReadLineParts(blockSize, token);

        var sortedLines = parts
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .WithCancellation(token)
            .Select(SortLines);

        var writingTasks = sortedLines.Select(part => WriteAllBlocksAsync(part, tempDirectoryPath, token));

        return await Task.WhenAll(writingTasks);
    }

    public static void MergeParts(IEnumerable<string> filesPath, string outputFilePath, CancellationToken token)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputFilePath, nameof(outputFilePath));

        var fileReaders = filesPath
            .Where(File.Exists)
            .Select(_ => new StreamReaderService(_))
            .Where(_ => _.MoveNext())
            .OrderBy(_ => _)
            .ToList();

        using StreamWriter writer = new(outputFilePath);
        while (fileReaders.Count > 0)
        {
            token.ThrowIfCancellationRequested();

            int lastIndex = fileReaders.Count - 1;
            StreamReaderService reader = fileReaders[lastIndex];

            writer.WriteLine(reader.Current);

            if (reader.MoveNext() == false)
            {
                fileReaders.RemoveAt(lastIndex);
                reader.Dispose();
                continue;
            }

            var newIndex = fileReaders.BinarySearch(reader);
            if (newIndex < 0)
            {
                newIndex = ~newIndex;
            }

            if (newIndex < lastIndex)
            {
                fileReaders.RemoveAt(lastIndex);
                fileReaders.Insert(newIndex, reader);
            }
        }
    }

    private static IEnumerable<string> SortLines(IEnumerable<string> lines)
    {
        return lines
            .Select(line => line.SplitToParts())
            .OrderBy(part => part.TextPart)
            .ThenBy(part => part.NumberPart)
            .Select(part => part.Original);
    }

    private static async Task<string> WriteAllBlocksAsync(IEnumerable<string> lines, string tempDirectoryPath, CancellationToken token)
    {
        string tempFilePath = Path.Combine(tempDirectoryPath, Guid.NewGuid().ToString("N"));
        await File.WriteAllLinesAsync(tempFilePath, lines, token);
        return tempFilePath;
    }
}
