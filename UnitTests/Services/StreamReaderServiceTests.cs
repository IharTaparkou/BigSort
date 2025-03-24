using Logic.Services;

namespace UnitTests.Services;

public class StreamReaderServiceTests
{
    [Fact]
    public void GetCurrentWhenNotEndOfStream()
    {
        var reader = CreateStreamReader("Line 1\nLine 2\nLine 3");

        Assert.True(reader.MoveNext());
        Assert.Equal("Line 1", reader.Current);

        Assert.True(reader.MoveNext());
        Assert.Equal("Line 2", reader.Current);

        Assert.True(reader.MoveNext());
        Assert.Equal("Line 3", reader.Current);
    }

    [Fact]
    public void GetCurrentThrowsExceptionWhenEndOfStream()
    {
        var reader = CreateStreamReader("Line 1");
        reader.MoveNext();

        var result = reader.MoveNext();

        Assert.False(result);
        Assert.Throws<InvalidOperationException>(() => reader.Current);
    }

    [Fact]
    public void GetCurrentThrowsExceptionWhenNotMovedNext()
    {
        var fileReader = CreateStreamReader("Line 1");

        Assert.Throws<InvalidOperationException>(() => fileReader.Current);
    }

    [Fact]
    public void GetCurrentAfterReset()
    {
        var reader = CreateStreamReader("Line 1\nLine 2");
        reader.MoveNext();

        reader.Reset();
        reader.MoveNext();

        Assert.Equal("Line 1", reader.Current);
    }

    [Fact]
    public void MoveNextAfterDispose()
    {
        var reader = CreateStreamReader("Line 1\nLine 2");

        reader.Dispose();

        Assert.Throws<ObjectDisposedException>(() => reader.MoveNext());
    }

    [Fact]
    public void CompareTo()
    {
        var readers = new[]
        {
            CreateStreamReader("415. Apple"),
            CreateStreamReader("2.Banana is yellow"),
            CreateStreamReader("1.Apple")
        };

        var orderedValues = readers
            .Where(_ => _.MoveNext())
            .OrderByDescending(_ => _)
            .Select(_ => _.Current)
            .ToArray();

        string[] expected =
        [
            "1.Apple",
            "415. Apple",
            "2.Banana is yellow",
        ];

        Assert.Equal(expected, orderedValues);
    }

    private static StreamReaderService CreateStreamReader(string fileContent)
    {
        var memoryStream = new MemoryStream();
        var writer = new StreamWriter(memoryStream);
        writer.Write(fileContent);
        writer.Flush();
        memoryStream.Position = 0;
        return new StreamReaderService(memoryStream);
    }
}