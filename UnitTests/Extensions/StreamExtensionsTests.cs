using System.Text;
using Logic.Extensions;

namespace UnitTests.Extensions;

public class StreamExtensionsTests
{
    [Fact]
    public void ReadLinePartsWhenTwoFull()
    {
        string content = "Line1\nLine2\nLine3\nLine4";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var result = StreamExtensions.ReadLineParts(stream, partSize: 2, CancellationToken.None).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(["Line1", "Line2"], result[0]);
        Assert.Equal(["Line3", "Line4"], result[1]);
    }

    [Fact]
    public void ReadLinePartsWhenTwoNotFull()
    {
        string content = "Line1\nLine2\nLine3";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var result = StreamExtensions.ReadLineParts(stream, partSize: 2, CancellationToken.None).ToArray();

        Assert.Equal(2, result.Length);
        Assert.Equal(["Line1", "Line2"], result[0]);
        Assert.Equal(["Line3"], result[1]);
    }

    [Fact]
    public void ReadLinePartsWhenIsEmpty()
    {
        string content = string.Empty;
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));

        var result = StreamExtensions.ReadLineParts(stream, partSize: 2, CancellationToken.None);

        Assert.Empty(result);
    }

    [Fact]
    public void ReadLinePartsThrowArgumentException()
    {
        using var stream = new MemoryStream();
        Assert.Throws<ArgumentException>(() =>
        {
            StreamExtensions.ReadLineParts(stream, partSize: 0, CancellationToken.None).ToArray();
        });
    }

    [Fact]
    public void ReadLinePartsThrowOperationCanceledException()
    {
        string content = "Line1";
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        Assert.Throws<OperationCanceledException>(() =>
        {
            StreamExtensions.ReadLineParts(stream, partSize: 1, cts.Token).ToArray();
        });
    }
}
