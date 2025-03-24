using System.Collections;
using Logic.Extensions;

namespace Logic.Services;

class StreamReaderService : IEnumerator<string>, IComparable<StreamReaderService>, IDisposable
{
    private readonly StreamReader streamReader;
    private string? current;
    private string? textPart;
    private int numberPart;

    public StreamReaderService(string filePath)
    {
        var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        streamReader = new(fileStream);
    }

    public StreamReaderService(Stream stream)
    {
        streamReader = new StreamReader(stream);
    }

    object IEnumerator.Current => Current;

    public string Current => current ?? throw new InvalidOperationException();

    public bool MoveNext()
    {
        if (streamReader.EndOfStream)
        {
            ResetValues();
            return false;
        }

        (current, textPart, numberPart) = streamReader.ReadLine()!.SplitToParts();
        return true;
    }

    public void Reset()
    {
        ResetValues();
        streamReader.BaseStream.Seek(0, SeekOrigin.Begin);
        streamReader.DiscardBufferedData();
    }

    public void ResetValues()
    {
        current = null;
        textPart = null;
        numberPart = 0;
    }

    public int CompareTo(StreamReaderService? other)
    {
        int textComparison = string.Compare(other!.textPart, textPart, StringComparison.OrdinalIgnoreCase);
        if (textComparison != 0)
        {
            return textComparison;
        }

        return other.numberPart.CompareTo(numberPart);
    }

    #region Dispose
    private bool disposedValue;
    protected virtual void Dispose(bool disposing)
    {
        if (disposedValue == false)
        {
            if (disposing)
            {
                streamReader.Close();
                streamReader.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
    #endregion
}
