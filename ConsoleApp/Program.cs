using System.Diagnostics;
using System.Text;
using Logic.Services;

internal class Program
{
    private static void Main(string[] _)
    {
        var blockSize = 400_000;
        var fileSize = 1;
        var workDirectory = "D:\\BigSortTest";
        var sourceFilePath = Path.Combine(workDirectory, $"source{fileSize}.txt");
        var resultFilePath = Path.Combine(workDirectory, $"result.txt");
        var tempDirectoryPath = Path.Combine(workDirectory, "Temp");

        if (Directory.Exists(workDirectory) == false)
        {
            Directory.CreateDirectory(workDirectory);
        }

        GenerateSourceFileIfNecessary(sourceFilePath, fileSize);

        if (Directory.Exists(tempDirectoryPath) == false)
        {
            Directory.CreateDirectory(tempDirectoryPath);
        }

        var sw = Stopwatch.StartNew();
        Console.WriteLine("Start: {0}()", nameof(FileService.SplitIntoSortedParts));
        var sortedFiles = FileService
            .SplitIntoSortedParts(sourceFilePath, tempDirectoryPath, blockSize, CancellationToken.None)
            .GetAwaiter()
            .GetResult();

        Console.WriteLine("Start: {0}()", nameof(FileService.MergeParts));
        FileService.MergeParts(sortedFiles, resultFilePath, CancellationToken.None);
        Console.WriteLine("Result: {0}", sw.Elapsed);

        sw.Stop();

        if (Directory.Exists(tempDirectoryPath))
        {
            Directory.Delete(tempDirectoryPath, recursive: true);
        }

        Console.ReadKey();
    }

    static void GenerateSourceFileIfNecessary(string filePath, long targetSizeGigabytes = 50)
    {
        if (File.Exists(filePath))
        {
            return;
        }

        var targetSize = ConvertGigabytesToBytes(targetSizeGigabytes);
        Random random = new();
        string[] phrases =
        [
            "Apple",
            "Banana is yellow",
            "Cherry is the best",
            "Something something something",
            "Orange is juicy"
        ];

        using StreamWriter sWriter = new(filePath, false, Encoding.UTF8);
        long currentSize = 0;

        while (currentSize < targetSize)
        {
            int number = random.Next();
            string text = phrases[random.Next(phrases.Length)];
            string line = $"{number}. {text}";

            sWriter.WriteLine(line);
            currentSize += Encoding.UTF8.GetByteCount(line);
        }
    }

    private static long ConvertGigabytesToBytes(double gigabytes)
    {
        const long bytesInGigabyte = 1024L * 1024 * 1024;
        return (long)(gigabytes * bytesInGigabyte);
    }
}