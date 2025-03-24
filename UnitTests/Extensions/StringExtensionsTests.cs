using Logic.Extensions;

namespace UnitTests.Extensions;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("1. Something", "1. Something", "Something", 1)]
    [InlineData("1.Something", "1.Something", "Something", 1)]
    [InlineData("1 .Something", "1 .Something", "Something", 1)]
    [InlineData(".abc", ".abc", ".abc", 0)]
    [InlineData("123", "123", "123", 0)]
    [InlineData("", "", "", 0)]
    public void SplitToParts(string input, string expectedOriginal, string expectedText, int expectedNumber)
    {
        var (originalResult, textPartResult, numberPartResult) = input.SplitToParts();

        Assert.Equal(expectedOriginal, originalResult);
        Assert.Equal(expectedText, textPartResult);
        Assert.Equal(expectedNumber, numberPartResult);
    }
}
